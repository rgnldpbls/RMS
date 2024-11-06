using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RemcSys.Models;

namespace RemcSys.Data
{
    public class RemcDBContext : DbContext
    {
        public RemcDBContext (DbContextOptions<RemcDBContext> options)
            : base(options)
        {
        }

        public DbSet<FundedResearchApplication> REMC_FundedResearchApplication { get; set; }
        public DbSet<GeneratedForm> REMC_GeneratedForms { get; set; }
        public DbSet<FileRequirement> REMC_FileRequirement { get; set; }
        public DbSet<FundedResearchEthics> REMC_FundedResearchEthics { get; set; }
        public DbSet<ActionLog> REMC_ActionLogs { get; set; }
        public DbSet<Evaluation> REMC_Evaluations { get; set; }
        public DbSet<Evaluator> REMC_Evaluator { get; set; }
        public DbSet<FundedResearch> REMC_FundedResearches { get; set; }
        public DbSet<ProgressReport> REMC_ProgressReports { get; set; }
        public DbSet<GenerateReport> REMC_GenerateReports { get; set; }
        public DbSet<GenerateGAWADNominees> REMC_GenerateGAWADNominees { get; set; }
        public DbSet<CalendarEvent> REMC_CalendarEvents { get; set; }
        public DbSet<Settings> REMC_Settings { get; set; }
        public DbSet<Guidelines> REMC_Guidelines { get; set; }
        public DbSet<Criteria> REMC_Criterias { get; set; }
        public DbSet<SubCategory> REMC_SubCategories { get; set; }
        public DbSet<UFRForecasting> REMC_UFRForecastings { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FundedResearchApplication>()
                .HasMany(f => f.GeneratedForms)
                .WithOne(g => g.FundedResearchApplication)
                .HasForeignKey(g => g.fra_Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FundedResearchApplication>()
                .HasMany(f => f.FileRequirements)
                .WithOne(g => g.fundedResearchApplication)
                .HasForeignKey(g => g.fra_Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FundedResearchApplication>()
                .HasOne(f => f.FundedResearchEthics)
                .WithOne(g => g.FundedResearchApplication)
                .HasForeignKey<FundedResearchEthics>(g => g.fra_Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FundedResearchApplication>()
                .HasMany(f => f.ActionLogs)
                .WithOne(g => g.fundedResearchApplication)
                .HasForeignKey(g => g.FraId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FundedResearchApplication>()
                .HasMany(f => f.Evaluations)
                .WithOne(g => g.fundedResearchApplication)
                .HasForeignKey(g => g.fra_Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Evaluator>()
                .HasMany(e => e.Evaluations)
                .WithOne(g => g.evaluator)
                .HasForeignKey(g => g.evaluator_Id);

            modelBuilder.Entity<FundedResearch>()
                .HasOne(f => f.FundedResearchApplication)
                .WithOne(f => f.FundedResearch)
                .HasForeignKey<FundedResearch>(g => g.fra_Id);

            modelBuilder.Entity<FundedResearch>()
                .HasMany(f => f.ProgressReports)
                .WithOne(f => f.FundedResearch)
                .HasForeignKey(g => g.fr_Id);

            modelBuilder.Entity<Criteria>()
                .HasMany(c => c.subCategory)
                .WithOne(c => c.criteria)
                .HasForeignKey(c => c.CriteriaId);

            base .OnModelCreating(modelBuilder);
        }
    }
}
