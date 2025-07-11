using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContractorManagementBackend.Models
{
    public class ContractorAssignment
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string? ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public Project? Project { get; set; }
        [Required] public Guid ContractorId { get; set; }
        [ForeignKey("ContractorId")]
        public ApplicationUser? Contractor { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}