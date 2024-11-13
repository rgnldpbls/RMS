using CRE.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CRE.Data
{
    public class CreDbContext : DbContext
    {
        public CreDbContext(DbContextOptions<CreDbContext> options) : base(options)
        {

        }

        // DbSets for various models
        public DbSet<Chairperson> Chairperson { get; set; }
        public DbSet<Chief> Chief { get; set; }
        public DbSet<CoProponent> CoProponent { get; set; }
        public DbSet<CompletionCertificate> CompletionCertificate { get; set; }
        public DbSet<CompletionReport> CompletionReport { get; set; }
        public DbSet<EthicsApplication> EthicsApplication { get; set; }
        public DbSet<EthicsApplicationForms> EthicsApplicationForms { get; set; }
        public DbSet<EthicsApplicationLog> EthicsApplicationLog { get; set; }
        public DbSet<EthicsClearance> EthicsClearance { get; set; }
        public DbSet<EthicsEvaluation> EthicsEvaluation { get; set; }
        public DbSet<EthicsEvaluator> EthicsEvaluator { get; set; }
        public DbSet<EthicsEvaluatorExpertise> EthicsEvaluatorExpertise { get; set; }
        public DbSet<EthicsForm> EthicsForm { get; set; }
        public DbSet<Expertise> Expertise { get; set; }
        public DbSet<Faculty> Faculty { get; set; }
        public DbSet<NonFundedResearchInfo> NonFundedResearchInfo { get; set; }
        public DbSet<InitialReview> InitialReview { get; set; }
        public DbSet<ReceiptInfo> ReceiptInfo { get; set; }
        public DbSet<Secretariat> Secretariat { get; set; }
        public DbSet<EvaluationForms> EthicEvaluationForms { get; set; }
        public DbSet<EthicsEvaluationDeclined> EthicsEvaluationDeclined { get; set; }
        public DbSet<Notification> Notification { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define relationships with foreign keys and restrict cascade deletes
            modelBuilder.Entity<NonFundedResearchInfo>()
                .HasOne(g => g.EthicsClearance)
                .WithOne(e => e.NonFundedResearchInfo)
                .HasForeignKey<NonFundedResearchInfo>(g => g.ethicsClearanceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EthicsEvaluatorExpertise>()
                .HasKey(ee => new { ee.ethicsEvaluatorId, ee.expertiseId });

            modelBuilder.Entity<EthicsEvaluation>()
                .HasOne(e => e.EthicsEvaluator)
                .WithMany(ev => ev.EthicsEvaluation)
                .HasForeignKey(e => e.ethicsEvaluatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EthicsEvaluatorExpertise>()
                .HasOne(ee => ee.EthicsEvaluator)
                .WithMany(ev => ev.EthicsEvaluatorExpertise)
                .HasForeignKey(ee => ee.ethicsEvaluatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EthicsEvaluatorExpertise>()
                .HasOne(ee => ee.Expertise)
                .WithMany(ex => ex.EthicsEvaluatorExpertise)
                .HasForeignKey(ee => ee.expertiseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EthicsReport>()
                .HasOne(e => e.Chief)
                .WithMany(c => c.EthicsReport)
                .HasForeignKey(e => e.chiefId)
                .HasPrincipalKey(c => c.chiefId); // Configure the principal key

            // Additional relationships


            modelBuilder.Entity<EthicsApplicationLog>()
                .HasOne(e => e.EthicsApplication)
                .WithMany(u => u.EthicsApplicationLog)
                .HasForeignKey(e => e.urecNo);

            base.OnModelCreating(modelBuilder);
        }
    }
}
