using ContractorManagement.Models;
using ContractorManagementBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ContractorManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ContractorAssignment> ContractorAssignments { get; set; }
        public DbSet<WeeklyTimeEntry> WeeklyTimeEntries { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<Project>()
                .Property(p => p.Status)
                .HasConversion<string>();

            modelBuilder.Entity<WeeklyTimeEntry>()
                .Property(t => t.Status)
                .HasConversion<string>();
        }
    }
}