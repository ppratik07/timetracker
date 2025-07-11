using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContractorManagementBackend.Models
{
    public enum ProjectStatus
    {
        Backlog,
        InProgress,
        Completed,
        OnHold,
        Cancelled
    }
   public class Project
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; }
        [Required]
        [StringLength(200)]
        public string ProjectName { get; set; }
        [Required]
        [StringLength(50)]
        public string CostCenter { get; set; }
        [Required]
        [StringLength(100)]
        public string Category { get; set; }
        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal EstimatedHours { get; set; }
        [Column(TypeName = "decimal(8,2)")]
        public decimal HoursWorked { get; set; } = 0;
        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalCost { get; set; } = 0;
        [Column(TypeName = "decimal(8,2)")]
        public decimal? HoursRemaining { get; set; }
        public ProjectStatus Status { get; set; } = ProjectStatus.Backlog;
        public DateTime? Deadline { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}