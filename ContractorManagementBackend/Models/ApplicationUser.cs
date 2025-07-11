using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ContractorManagementBackend.Models;

namespace ContractorManagementBackend.Models
{
    public enum UserRole
    {
        Admin,
        Contractor,
        Client
    }

    public class ApplicationUser
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string? Username { get; set; }
        [Required]
        [StringLength(255)]
        public string? PasswordHash { get; set; }
        [Required]
        [StringLength(100)]
        public string? FullName { get; set; }
        [Required]
        public UserRole Role { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? HourlyRate { get; set; }
        [StringLength(100)]
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}