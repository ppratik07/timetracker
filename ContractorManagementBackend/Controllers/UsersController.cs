using ContractorManagement.Data;
using ContractorManagementBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ContractorManagement.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userDto)
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = userDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                FullName = userDto.FullName,
                Role = userDto.Role,
                HourlyRate = userDto.HourlyRate,
                Email = userDto.Email
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] ApplicationUser user)
        {
            if (id != user.Id)
                return BadRequest();

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            user.IsActive = false;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("contractors")]
        public async Task<IActionResult> GetContractors()
        {
            var contractors = await _context.Users
                .Where(u => u.Role == UserRole.Contractor)
                .ToListAsync();
            return Ok(contractors);
        }

        public class CreateUserDto
        {
            public string? Username { get; set; }
            public string? Password { get; set; }
            public string? FullName { get; set; }
            public UserRole Role { get; set; }
            public decimal? HourlyRate { get; set; }
            public string? Email { get; set; }
        }
    }
}