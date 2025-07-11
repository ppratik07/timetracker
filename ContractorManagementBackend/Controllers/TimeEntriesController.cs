using System.Security.Claims;
using ContractorManagement.Data;
using ContractorManagementBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContractorManagement.Controllers
{
    [Route("api/time-entries")]
    [ApiController]
    public class TimeEntriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TimeEntriesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetTimeEntries()
        {
            var entries = await _context.WeeklyTimeEntries.ToListAsync();
            return Ok(entries);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTimeEntry([FromBody] WeeklyTimeEntry entry)
        {
            var contractor = await _context.Users.FindAsync(entry.ContractorId);
            if (contractor == null || contractor.HourlyRate == null)
                return BadRequest("Invalid contractor or hourly rate");

            entry.Cost = entry.HoursWorked * contractor.HourlyRate.Value;
            _context.WeeklyTimeEntries.Add(entry);

            // Update project hours and cost
            var project = await _context.Projects.FindAsync(entry.ProjectId);
            if (project != null)
            {
                project.HoursWorked += entry.HoursWorked;
                project.TotalCost += entry.Cost;
                project.HoursRemaining = project.EstimatedHours - project.HoursWorked;
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTimeEntry), new { id = entry.Id }, entry);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTimeEntry(Guid id)
        {
            var entry = await _context.WeeklyTimeEntries.FindAsync(id);
            if (entry == null)
                return NotFound();
            return Ok(entry);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTimeEntry(Guid id, [FromBody] WeeklyTimeEntry entry)
        {
            if (id != entry.Id)
                return BadRequest();

            _context.Entry(entry).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimeEntry(Guid id)
        {
            var entry = await _context.WeeklyTimeEntries.FindAsync(id);
            if (entry == null)
                return NotFound();

            _context.WeeklyTimeEntries.Remove(entry);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("contractor/{id}")]
        public async Task<IActionResult> GetContractorTimeEntries(Guid id)
        {
            var entries = await _context.WeeklyTimeEntries
                .Where(te => te.ContractorId == id)
                .ToListAsync();
            return Ok(entries);
        }

        //authorization to ensure only admins can approve/reject
        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveTimeEntry(Guid id)
        {
            var entry = await _context.WeeklyTimeEntries.FindAsync(id);
            if (entry == null)
                return NotFound();

            entry.Status = TimeEntryStatus.Approved;
            entry.ApprovedAt = DateTime.UtcNow;
            entry.ApprovedById = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value?? string.Empty);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectTimeEntry(Guid id)
        {
            var entry = await _context.WeeklyTimeEntries.FindAsync(id);
            if (entry == null)
                return NotFound();

            entry.Status = TimeEntryStatus.Rejected;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}