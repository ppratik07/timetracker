// Controllers/ReportsController.cs
using ContractorManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.IO;
using CsvHelper;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace ContractorManagement.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("project-summary")]
        public async Task<IActionResult> GetProjectSummary()
        {
            var summary = await _context.Projects
                .Select(p => new
                {
                    p.Id,
                    p.ProjectName,
                    p.CostCenter,
                    p.Category,
                    p.EstimatedHours,
                    p.HoursWorked,
                    p.TotalCost,
                    p.HoursRemaining,
                    p.Status
                })
                .ToListAsync();
            return Ok(summary);
        }

        [HttpGet("contractor-hours")]
        public async Task<IActionResult> GetContractorHours()
        {
            var hours = await _context.WeeklyTimeEntries
                .GroupBy(te => te.ContractorId)
                .Select(g => new
                {
                    ContractorId = g.Key,
                    ContractorName = g.First().Contractor != null ? g.First().Contractor.FullName ?? string.Empty : string.Empty,
                    TotalHours = g.Sum(te => te.HoursWorked),
                    TotalCost = g.Sum(te => te.Cost)
                })
                .ToListAsync();
            return Ok(hours);
        }

        [HttpGet("cost-analysis")]
        public async Task<IActionResult> GetCostAnalysis()
        {
            var analysis = await _context.Projects
                .GroupBy(p => p.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalCost = g.Sum(p => p.TotalCost),
                    TotalHours = g.Sum(p => p.HoursWorked)
                })
                .ToListAsync();
            return Ok(analysis);
        }

        [HttpGet("export/csv")]
        public async Task<IActionResult> ExportCsv()
        {
            var projects = await _context.Projects
                .Select(p => new
                {
                    p.Id,
                    p.ProjectName,
                    p.CostCenter,
                    p.Category,
                    p.EstimatedHours,
                    p.HoursWorked,
                    p.TotalCost,
                    p.HoursRemaining,
                    p.Status
                })
                .ToListAsync();

            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(projects);
            await writer.FlushAsync();
            memoryStream.Position = 0;

            return File(memoryStream, "text/csv", "project_summary.csv");
        }

        [HttpGet("export/pdf")]
        public async Task<IActionResult> ExportPdf()
        {
            var projects = await _context.Projects
                .Select(p => new
                {
                    p.Id,
                    p.ProjectName,
                    p.CostCenter,
                    p.Category,
                    p.EstimatedHours,
                    p.HoursWorked,
                    p.TotalCost,
                    p.HoursRemaining,
                    p.Status
                })
                .ToListAsync();

            using var memoryStream = new MemoryStream();
            using var writer = new PdfWriter(memoryStream);
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            document.Add(new Paragraph("Project Summary Report").SetFontSize(20));

            var table = new Table(9);
            table.AddHeaderCell("ID");
            table.AddHeaderCell("Project Name");
            table.AddHeaderCell("Cost Center");
            table.AddHeaderCell("Category");
            table.AddHeaderCell("Estimated Hours");
            table.AddHeaderCell("Hours Worked");
            table.AddHeaderCell("Total Cost");
            table.AddHeaderCell("Hours Remaining");
            table.AddHeaderCell("Status");

            foreach (var project in projects)
            {
                table.AddCell(project.Id);
                table.AddCell(project.ProjectName);
                table.AddCell(project.CostCenter);
                table.AddCell(project.Category);
                table.AddCell(project.EstimatedHours.ToString());
                table.AddCell(project.HoursWorked.ToString());
                table.AddCell(project.TotalCost.ToString("C"));
                table.AddCell(project.HoursRemaining?.ToString() ?? "N/A");
                table.AddCell(project.Status.ToString());
            }

            document.Add(table);
            document.Close();

            return File(memoryStream.ToArray(), "application/pdf", "project_summary.pdf");
        }
    }
}