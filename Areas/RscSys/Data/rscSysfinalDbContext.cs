using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using rscSys_final.Models;
using System.Reflection.Emit;

namespace rscSys_final.Data;

public class rscSysfinalDbContext : DbContext
{
    public rscSysfinalDbContext(DbContextOptions<rscSysfinalDbContext> options)
        : base(options)
    {
    }

    public DbSet<Draft> Drafts { get; set; }
    public DbSet<Requirement> Requirements { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<Memorandum> Memorandums { get; set; }
    public DbSet<StatusHistory> StatusHistories { get; set; }
    public DbSet<Notifications> Notifications { get; set; }
    public DbSet<DocumentHistory> DocumentHistories { get; set; }
    public DbSet<EvaluatorAssignment> EvaluatorAssignments { get; set; }
    public DbSet<ApplicationType> ApplicationTypes { get; set; }
    public DbSet<Checklist> Checklists { get; set; }
    public DbSet<ApplicationSubCategory> ApplicationSubCategories { get; set; }
    public DbSet<Evaluator> Evaluators { get; set; }

    public DbSet<EvaluationForm> EvaluationForms { get; set; }
    public DbSet<Criterion> Criteria { get; set; }
    public DbSet<EvaluationFormResponse> EvaluationFormResponses { get; set; }

    public DbSet<EvaluationGeneralComment> EvaluationGeneralComments { get; set; }
    public DbSet<EvaluationDocument> EvaluationDocuments { get; set; }
    public DbSet<GeneratedReport> GeneratedReports { get; set; }
    public DbSet<FinalDocument> FinalDocuments { get; set; }
    public DbSet<Template> Templates { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Relationship between Draft and ApplicationUser (one-to-many)
        /*builder.Entity<Draft>()
            .HasOne(d => d.User)
            .WithMany(u => u.Drafts)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);*/

/*        // Relationship between Request and ApplicationUser (one-to-many)
        builder.Entity<Request>()
            .HasOne(r => r.User)
            .WithMany(u => u.Requests)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);*/

        /*builder.Entity<Memorandum>()
            .HasOne(d => d.User)
            .WithMany(u => u.Memorandums)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<GeneratedReport>()
           .HasOne(d => d.User)
           .WithMany(u => u.GeneratedReports)
           .HasForeignKey(d => d.GeneratedBy)
           .OnDelete(DeleteBehavior.Cascade);*/

        builder.Entity<Requirement>()
          .HasOne(r => r.Draft)
          .WithMany(d => d.Requirements)
          .HasForeignKey(r => r.DraftId)
          .OnDelete(DeleteBehavior.Cascade);

       // Requirement related to Request with cascading delete
       builder.Entity<Requirement>()
          .HasOne(r => r.Request)
          .WithMany(req => req.Requirements)
          .HasForeignKey(r => r.RequestId)
          .OnDelete(DeleteBehavior.Restrict);

        // StatusHistory relationship with Request (one-to-many)
        builder.Entity<StatusHistory>()
            .HasOne(sh => sh.Request)
            .WithMany(r => r.StatusHistories)
            .HasForeignKey(sh => sh.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        // DocumentHistory relationship with Request    (one-to-many)
        builder.Entity<DocumentHistory>()
            .HasOne(dh => dh.Request)
            .WithMany(r => r.DocumentHistories)
            .HasForeignKey(dh => dh.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        // Define primary key for EvaluatorAssignment
        builder.Entity<EvaluatorAssignment>()
            .HasKey(ea => ea.AssignmentId);  // Explicitly set the primary key

        // EvaluatorAssignment relationship with Request (one-to-many)
        builder.Entity<EvaluatorAssignment>()
            .HasOne(ea => ea.Request)
            .WithMany(r => r.EvaluatorAssignments)
            .HasForeignKey(ea => ea.RequestId)
            .OnDelete(DeleteBehavior.Restrict);

        // Notification relationship with User (many-to-one)
        /*builder.Entity<Notifications>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.NoAction); // Or NoAction*/

        builder.Entity<ApplicationType>()
        .HasMany(a => a.ApplicationSubCategories) // Relationship with ApplicationSubCategories
        .WithOne(a => a.ApplicationType)
        .HasForeignKey(a => a.ApplicationTypeId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationType>()
            .HasMany(a => a.Checklists)
            .WithOne(c => c.ApplicationType)
            .HasForeignKey(c => c.ApplicationTypeId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete from ApplicationType to Checklists

        builder.Entity<ApplicationSubCategory>()
            .HasMany(c => c.Checklists) // Relationship with Checklists
            .WithOne(c => c.ApplicationSubCategory) // Each checklist belongs to one sub-category
            .HasForeignKey(c => c.ApplicationSubCategoryId) // FK in Checklist
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Evaluator>()
           .HasKey(e => e.EvaluatorId);

        /*builder.Entity<Evaluator>()
            .HasOne(e => e.User)
            .WithOne(u => u.EvaluatorProfile)
            .HasForeignKey<Evaluator>(e => e.EvaluatorId);*/

        builder.Entity<Notifications>()
       .HasOne(n => n.EvaluatorAssignment)
       .WithMany(a => a.Notifications)
       .HasForeignKey(n => n.EvaluatorAssignmentId)
       .OnDelete(DeleteBehavior.Cascade); // Set cascade delete

        builder.Entity<EvaluationForm>()
        .HasKey(e => e.FormId);

        builder.Entity<EvaluationForm>()
            .HasMany(e => e.Criteria)
            .WithOne()
            .HasForeignKey(c => c.FormId) // Keep this cascading if needed
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete

        // Configure Criterion
        builder.Entity<Criterion>()
            .HasKey(c => c.CriterionId);

        // Define the other foreign key relationship
        // Define the relationship
        builder.Entity<Criterion>()
            .HasOne(c => c.EvaluationForm) // Specify the navigation property
            .WithMany(e => e.Criteria) // Specify the collection on EvaluationForm
            .HasForeignKey(c => c.FormId) // Define the foreign key
            .OnDelete(DeleteBehavior.Cascade); // Set cascade delete as needed

        builder.Entity<Criterion>()
            .Property(c => c.Percentage)
            .HasColumnType("numeric(11,2)");

        // Configure EvaluationFormResponse
        builder.Entity<EvaluationFormResponse>()
            .HasKey(e => e.ResponseId);

        builder.Entity<EvaluationFormResponse>()
            .HasOne(e => e.Criterion)
            .WithMany(c => c.EvaluationFormResponses) // Assuming you want to connect to comments
            .HasForeignKey(e => e.CriterionId);

        builder.Entity<EvaluationFormResponse>()
             .HasOne(e => e.Evaluator)
             .WithMany(e => e.EvaluationFormResponses) // Link to the collection in Evaluator
             .HasForeignKey(e => e.EvaluatorId)
             .OnDelete(DeleteBehavior.NoAction); // Adjust as necessary

        builder.Entity<EvaluationFormResponse>()
            .HasOne(e => e.EvaluatorAssignment)
            .WithMany() // Adjust as needed
            .HasForeignKey(e => e.EvaluatorAssignmentId);

        builder.Entity<EvaluationFormResponse>()
                    .Property(cr => cr.UserPercentage)
                    .HasColumnType("numeric(11,2)");

        // Configure EvaluationGeneralComment
        builder.Entity<EvaluationGeneralComment>()
            .HasKey(c => c.CommentId);

        builder.Entity<EvaluationGeneralComment>()
            .HasOne(c => c.EvaluatorAssignment)
            .WithMany() // Update based on your relationship
            .HasForeignKey(c => c.EvaluatorAssignmentId);

        builder.Entity<EvaluationDocument>()
            .HasKey(e => e.EvaluationDocuId); // Set primary key

        builder.Entity<EvaluationDocument>()
            .HasOne(e => e.EvaluatorAssignment)
            .WithMany() // Adjust this if you have a collection of documents in EvaluatorAssignment
            .HasForeignKey(e => e.EvaluatorAssignmentId)
            .OnDelete(DeleteBehavior.Cascade); // Optional: set delete behavior

        builder.Entity<FinalDocument>()
            .HasOne(fd => fd.Request)    // Navigation property in FinalDocument
            .WithMany(r => r.FinalDocuments) // Assuming a Request can have multiple FinalDocuments
            .HasForeignKey(fd => fd.RequestId) // Foreign key property
            .OnDelete(DeleteBehavior.Cascade); // Configure cascade delete behavior if needed

        // Checklist to Requirement (cascade delete)
        builder.Entity<Checklist>()
            .HasMany(c => c.Requirements)
            .WithOne(r => r.Checklist)
            .HasForeignKey(r => r.ChecklistId);

        builder.Entity<Requirement>()
            .HasOne(r => r.Checklist)
            .WithMany(c => c.Requirements)
            .HasForeignKey(r => r.ChecklistId);

        builder.Entity<Checklist>()
            .HasOne(c => c.ApplicationType)
            .WithMany(a => a.Checklists)
            .HasForeignKey(c => c.ApplicationTypeId)
            .OnDelete(DeleteBehavior.NoAction); // Adjust based on your requirements
        /*
                builder.Entity<Checklist>()
                .HasMany(c => c.Requirements)
                .WithOne(r => r.Checklist)
                 .OnDelete(DeleteBehavior.NoAction); // Changed to NoAction*/


        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
