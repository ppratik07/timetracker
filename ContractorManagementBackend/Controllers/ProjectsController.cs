using ContractorManagement.Data;
using ContractorManagementBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContractorManagement.Controllers
{
    [Route("api/projects")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var projects = await _context.Projects.ToListAsync();
            return Ok(projects);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto projectDto)
        {
            if (projectDto.EstimatedHours < 0)
                return BadRequest("Estimated hours cannot be negative");

            var project = new Project
            {
                Id = projectDto.Id,
                ProjectName = projectDto.ProjectName,
                CostCenter = projectDto.CostCenter,
                Category = projectDto.Category,
                EstimatedHours = projectDto.EstimatedHours,
                Deadline = projectDto.Deadline
            };
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(string id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return NotFound();
            return Ok(project);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(string id, [FromBody] Project project)
        {
            if (id != project.Id)
                return BadRequest();

            _context.Entry(project).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(string id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return NotFound();

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id}/contractors")]
        public async Task<IActionResult> GetProjectContractors(string id)
        {
            var contractors = await _context.ContractorAssignments
                .Where(ca => ca.ProjectId == id && ca.IsActive)
                .Select(ca => ca.Contractor)
                .ToListAsync();
            return Ok(contractors);
        }

        [HttpPost("{id}/assign-contractors")]
        public async Task<IActionResult> AssignContractors(string id, [FromBody] List<Guid> contractorIds)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return NotFound();

            foreach (var contractorId in contractorIds)
            {
                _context.ContractorAssignments.Add(new ContractorAssignment
                {
                    ProjectId = id,
                    ContractorId = contractorId
                });
            }
            await _context.SaveChangesAsync();
            return Ok();
        }
        public class CreateProjectDto
        {
            public string? Id { get; set; }
            public string? ProjectName { get; set; }
            public string? CostCenter { get; set; }
            public string? Category { get; set; }
            public decimal EstimatedHours { get; set; }
            public DateTime? Deadline { get; set; }
        }
    }
}