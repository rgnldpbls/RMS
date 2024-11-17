using CrdlSys.Models;
using Microsoft.EntityFrameworkCore;

namespace CrdlSys.Data
{
    public class CrdlDbContext : DbContext
    {
        public CrdlDbContext(DbContextOptions<CrdlDbContext> options) : base(options) { }
        //public DbSet<User> Users { get; set; }
        //public DbSet<Role> Roles { get; set; } 
        //public DbSet<UserRole> UserRoles { get; set; }  
        public DbSet<ChiefUpload> ChiefUpload { get; set; }  
        public DbSet<StakeholderUpload> StakeholderUpload { get; set; } 
        public DbSet<DocumentTracking> DocumentTracking { get; set; } 
        public DbSet<ResearchEvent> ResearchEvent { get; set; } 
        public DbSet<ResearchEventRegistration> ResearchEventRegistration { get; set; } 
        public DbSet<ResearchEventInvitation> ResearchEventInvitation { get; set; } 
        public DbSet<GeneratedReport> GeneratedReport { get; set; } 
        public DbSet<GeneratedSentimentAnalysis> GeneratedSentimentAnalysis { get; set; } 
        public DbSet<RenewalHistory> RenewalHistory { get; set; }
    }
}