using Microsoft.EntityFrameworkCore;
using ClaimManagementSystem.Models;

namespace ClaimManagementSystem.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<ClaimSubmission> ClaimSubmission { get; set; }
        public DbSet<Contractor> Contractors { get; set; }
        public DbSet<Payments> Payments { get; set; }
        public DbSet<ClaimStatus> ClaimStatuses { get; set; }
        public DbSet<ContractorResponse> ContractorResponses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure that ClaimSubmission has a primary key defined
            modelBuilder.Entity<ClaimSubmission>()
                .HasKey(cs => cs.ClaimID);

            // Ensure that ClaimStatus has a primary key defined
            modelBuilder.Entity<ClaimStatus>()
                .HasKey(cs => cs.StatusID);

            // Ensure that ContractorResponse has a primary key defined
            modelBuilder.Entity<ContractorResponse>()
                .HasKey(cs => cs.ResponseID);

            // Ensure that Lecturer has a primary key defined
            modelBuilder.Entity<Lecturer>()
                .HasKey(cs => cs.EmployeeNumber);

            // Ensure that Payment has a primary key defined
            modelBuilder.Entity<Payments>()
                .HasKey(cs => cs.PaymentID);

            // Ensure that ContractorResponse has a primary key defined
            modelBuilder.Entity<Contractor>()
                .HasKey(cs => cs.ContractorID);
        }
    }
}
