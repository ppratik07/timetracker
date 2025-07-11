using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContractorManagementBackend.Models
{
    public enum TimeEntryStatus
    {
        Draft,
        Submitted,
        Approved,
        Rejected
    }
    public class WeeklyTimeEntry
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid ContractorId { get; set; }
        [ForeignKey("ContractorId")]
        public ApplicationUser? Contractor { get; set; }
        [Required]
        [StringLength(50)]
        public string? ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public Project? Project { get; set; }
        [Required]
        public DateTime WeekStartDate { get; set; }
        [Required]
        [Column(TypeName = "decimal(6,2)")]
        public decimal HoursWorked { get; set; }
        public string? Description { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Cost { get; set; }
        public TimeEntryStatus Status { get; set; } = TimeEntryStatus.Draft;
        public DateTime? SubmittedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public Guid? ApprovedById { get; set; }
        [ForeignKey("ApprovedById")]
        public ApplicationUser? ApprovedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}