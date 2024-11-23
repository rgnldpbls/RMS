using Microsoft.EntityFrameworkCore;
using ResearchManagementSystem.Areas.CreSys.Models;

namespace ResearchManagementSystem.Areas.CreSys.Data
{
    public class CreDbContext : DbContext
    {
        private readonly IWebHostEnvironment _env;
        public CreDbContext(DbContextOptions<CreDbContext> options, IWebHostEnvironment env)
           : base(options)
        {
            _env = env;
        }

        public DbSet<Chairperson> CRE_Chairperson { get; set; }
        public DbSet<CompletionCertificate> CRE_CompletionCertificates { get; set; }
        public DbSet<CompletionReport> CRE_CompletionReports { get; set; }
        public DbSet<CoProponent> CRE_CoProponents { get; set; }
        public DbSet<DeclinedEthicsEvaluation> CRE_DeclinedEthicsEvaluation { get; set; }
        public DbSet<EthicsApplication> CRE_EthicsApplication { get; set; }
        public DbSet<EthicsApplicationForms> CRE_EthicsApplicationForms { get; set; }
        public DbSet<EthicsApplicationLogs> CRE_EthicsApplicationLogs { get; set; }
        public DbSet<EthicsClearance> CRE_EthicsClearance { get; set; }
        public DbSet<EthicsEvaluation> CRE_EthicsEvaluation { get; set; }
        public DbSet<EthicsEvaluator> CRE_EthicsEvaluator { get; set; }
        public DbSet<EthicsEvaluatorExpertise> CRE_EthicsEvaluatorExpertise { get; set; }
        public DbSet<EthicsForms> CRE_EthicsForms { get; set; }
        public DbSet<EthicsMemoranda> CRE_EthicsMemoranda { get; set; }
        public DbSet<EthicsNotifications> CRE_EthicsNotifications { get; set; }
        public DbSet<EthicsReport> CRE_EthicsReport { get; set; }
        public DbSet<Expertise> CRE_Expertise { get; set; }
        public DbSet<InitialReview> CRE_InitialReview { get; set; }
        public DbSet<NonFundedResearchInfo> CRE_NonFundedResearchInfo { get; set; }
        public DbSet<ReceiptInfo> CRE_ReceiptInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the relationship for EthicsEvaluation to EthicsEvaluator
            modelBuilder.Entity<EthicsEvaluation>()
                .HasOne(e => e.EthicsEvaluator)
                .WithMany(e => e.EthicsEvaluation)
                .HasForeignKey(e => e.EthicsEvaluatorId)  // Now using EthicsEvaluatorId as the foreign key
                .IsRequired(false);  // Make it optional

            // Configure the EthicsEvaluator entity
            modelBuilder.Entity<EthicsEvaluator>()
                .HasKey(e => e.EthicsEvaluatorId);

            // Configure other relationships for EthicsEvaluatorExpertise
            modelBuilder.Entity<EthicsEvaluatorExpertise>()
                .HasKey(e => new { e.EthicsEvaluatorId, e.ExpertiseId });

            modelBuilder.Entity<EthicsEvaluatorExpertise>()
                .HasOne(e => e.EthicsEvaluator)
                .WithMany(e => e.EthicsEvaluatorExpertises)
                .HasForeignKey(e => e.EthicsEvaluatorId);

            modelBuilder.Entity<EthicsEvaluatorExpertise>()
                .HasOne(e => e.Expertise)
                .WithMany(e => e.EthicsEvaluatorExpertise)
                .HasForeignKey(e => e.ExpertiseId);

            // Seed data for Expertise
            modelBuilder.Entity<Expertise>().HasData(
                new Expertise { ExpertiseId = 1, ExpertiseName = "Education" },
                new Expertise { ExpertiseId = 2, ExpertiseName = "Computer Science, Information Systems, and Technology" },
                new Expertise { ExpertiseId = 3, ExpertiseName = "Engineering, Architecture, and Design" },
                new Expertise { ExpertiseId = 4, ExpertiseName = "Humanities, Language, and Communication" },
                new Expertise { ExpertiseId = 5, ExpertiseName = "Business" },
                new Expertise { ExpertiseId = 6, ExpertiseName = "Social Sciences" },
                new Expertise { ExpertiseId = 7, ExpertiseName = "Science, Mathematics, and Statistics" }
            );
        }
    }
}
