using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ContractorManagementBackend.Models;

namespace ContractorManagement.Models
{
    public class AuditLog
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
        [Required]
        [StringLength(100)]
        public string? Action { get; set; }
        [StringLength(50)]
        public string? TableName { get; set; }
        [StringLength(100)]
        public string? RecordId { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}