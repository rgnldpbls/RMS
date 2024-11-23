﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using rscSys_final.Data;

#nullable disable

namespace ResearchManagementSystem.Migrations.rscSysfinalDb
{
    [DbContext(typeof(rscSysfinalDbContext))]
    [Migration("20241115170129_tablesNames")]
    partial class tablesNames
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("rscSys_final.Models.ApplicationSubCategory", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryId"));

                    b.Property<int>("ApplicationTypeId")
                        .HasColumnType("int");

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("SubAmount")
                        .HasColumnType("numeric(11,2)");

                    b.HasKey("CategoryId");

                    b.HasIndex("ApplicationTypeId");

                    b.ToTable("RSC_ApplicationSubCategories");
                });

            modelBuilder.Entity("rscSys_final.Models.ApplicationType", b =>
                {
                    b.Property<int>("ApplicationTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ApplicationTypeId"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric(11,2)");

                    b.Property<DateTime>("ApplicationTypeCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("ApplicationTypeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ApplicationTypeUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ApplicationTypeId");

                    b.ToTable("RSC_ApplicationTypes");
                });

            modelBuilder.Entity("rscSys_final.Models.Checklist", b =>
                {
                    b.Property<int>("ChecklistId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ChecklistId"));

                    b.Property<int?>("ApplicationSubCategoryId")
                        .HasColumnType("int");

                    b.Property<int?>("ApplicationTypeId")
                        .HasColumnType("int");

                    b.Property<string>("ChecklistName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ChecklistId");

                    b.HasIndex("ApplicationSubCategoryId");

                    b.HasIndex("ApplicationTypeId");

                    b.ToTable("RSC_Checklists");
                });

            modelBuilder.Entity("rscSys_final.Models.Criterion", b =>
                {
                    b.Property<int>("CriterionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CriterionId"));

                    b.Property<int>("FormId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Percentage")
                        .HasColumnType("numeric(11,2)");

                    b.HasKey("CriterionId");

                    b.HasIndex("FormId");

                    b.ToTable("RSC_Criteria");
                });

            modelBuilder.Entity("rscSys_final.Models.DocumentHistory", b =>
                {
                    b.Property<int>("DocumentHistoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DocumentHistoryId"));

                    b.Property<string>("Comments")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PerformedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RequestId")
                        .HasColumnType("int");

                    b.HasKey("DocumentHistoryId");

                    b.HasIndex("RequestId");

                    b.ToTable("RSC_DocumentHistories");
                });

            modelBuilder.Entity("rscSys_final.Models.Draft", b =>
                {
                    b.Property<int>("DraftId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DraftId"));

                    b.Property<string>("ApplicationType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Branch")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("College")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("DtsNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FieldOfStudy")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ResearchTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("DraftId");

                    b.ToTable("RSC_Drafts");
                });

            modelBuilder.Entity("rscSys_final.Models.EvaluationDocument", b =>
                {
                    b.Property<int>("EvaluationDocuId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EvaluationDocuId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("EvaluatorAssignmentId")
                        .HasColumnType("int");

                    b.Property<byte[]>("FileData")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EvaluationDocuId");

                    b.HasIndex("EvaluatorAssignmentId");

                    b.ToTable("RSC_EvaluationDocuments");
                });

            modelBuilder.Entity("rscSys_final.Models.EvaluationForm", b =>
                {
                    b.Property<int>("FormId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FormId"));

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("FormId");

                    b.ToTable("RSC_EvaluationForms");
                });

            modelBuilder.Entity("rscSys_final.Models.EvaluationFormResponse", b =>
                {
                    b.Property<int>("ResponseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ResponseId"));

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CriterionId")
                        .HasColumnType("int");

                    b.Property<int>("EvaluatorAssignmentId")
                        .HasColumnType("int");

                    b.Property<string>("EvaluatorId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("UserPercentage")
                        .HasColumnType("numeric(11,2)");

                    b.HasKey("ResponseId");

                    b.HasIndex("CriterionId");

                    b.HasIndex("EvaluatorAssignmentId");

                    b.HasIndex("EvaluatorId");

                    b.ToTable("RSC_EvaluationFormResponses");
                });

            modelBuilder.Entity("rscSys_final.Models.EvaluationGeneralComment", b =>
                {
                    b.Property<int>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CommentId"));

                    b.Property<string>("CommentText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("EvaluatorAssignmentId")
                        .HasColumnType("int");

                    b.HasKey("CommentId");

                    b.HasIndex("EvaluatorAssignmentId");

                    b.ToTable("RSC_EvaluationGeneralComments");
                });

            modelBuilder.Entity("rscSys_final.Models.Evaluator", b =>
                {
                    b.Property<string>("EvaluatorId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("CompletedCount")
                        .HasColumnType("int");

                    b.Property<int?>("DeclinedCount")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PendingCount")
                        .HasColumnType("int");

                    b.Property<string>("Specialization")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EvaluatorId");

                    b.ToTable("RSC_Evaluators");
                });

            modelBuilder.Entity("rscSys_final.Models.EvaluatorAssignment", b =>
                {
                    b.Property<int>("AssignmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AssignmentId"));

                    b.Property<DateTime>("AssignedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EvaluationDeadline")
                        .HasColumnType("datetime2");

                    b.Property<string>("EvaluationStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EvaluatorId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Feedback")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RequestId")
                        .HasColumnType("int");

                    b.HasKey("AssignmentId");

                    b.HasIndex("EvaluatorId");

                    b.HasIndex("RequestId");

                    b.ToTable("RSC_EvaluatorAssignments");
                });

            modelBuilder.Entity("rscSys_final.Models.FinalDocument", b =>
                {
                    b.Property<int>("FinalDocuID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FinalDocuID"));

                    b.Property<byte[]>("FileData")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("FileType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FinalDocuName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("RequestId")
                        .HasColumnType("int");

                    b.HasKey("FinalDocuID");

                    b.HasIndex("RequestId");

                    b.ToTable("RSC_FinalDocuments");
                });

            modelBuilder.Entity("rscSys_final.Models.GeneratedReport", b =>
                {
                    b.Property<int>("ReportId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ReportId"));

                    b.Property<byte[]>("FileData")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("FileType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GeneratedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("GeneratedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ReportName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ReportId");

                    b.ToTable("RSC_GeneratedReports");
                });

            modelBuilder.Entity("rscSys_final.Models.Memorandum", b =>
                {
                    b.Property<int>("memorandumId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("memorandumId"));

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("filetype")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("memorandumData")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("memorandumName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateOnly?>("memorandumUpdateDate")
                        .HasColumnType("date");

                    b.Property<DateOnly>("memorandumUploadDate")
                        .HasColumnType("date");

                    b.HasKey("memorandumId");

                    b.ToTable("RSC_Memorandums");
                });

            modelBuilder.Entity("rscSys_final.Models.Notifications", b =>
                {
                    b.Property<int>("NotificationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("NotificationId"));

                    b.Property<int?>("EvaluatorAssignmentId")
                        .HasColumnType("int");

                    b.Property<DateTime>("NotificationCreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("NotificationMessage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("NotificationStatus")
                        .HasColumnType("bit");

                    b.Property<string>("NotificationTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PerformedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("NotificationId");

                    b.HasIndex("EvaluatorAssignmentId");

                    b.ToTable("RSC_Notifications");
                });

            modelBuilder.Entity("rscSys_final.Models.Request", b =>
                {
                    b.Property<int>("RequestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RequestId"));

                    b.Property<string>("ApplicationType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Branch")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("College")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("DtsNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FieldOfStudy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsHardCopyReceived")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("RequestSpent")
                        .HasColumnType("float");

                    b.Property<string>("ResearchTitle")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("SubmittedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RequestId");

                    b.ToTable("RSC_Requests");
                });

            modelBuilder.Entity("rscSys_final.Models.Requirement", b =>
                {
                    b.Property<int>("RequirementId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RequirementId"));

                    b.Property<int?>("ChecklistId")
                        .HasColumnType("int");

                    b.Property<int?>("DraftId")
                        .HasColumnType("int");

                    b.Property<byte[]>("FileData")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsResubmitted")
                        .HasColumnType("bit");

                    b.Property<int?>("RequestId")
                        .HasColumnType("int");

                    b.Property<DateTime>("UploadDate")
                        .HasColumnType("datetime2");

                    b.HasKey("RequirementId");

                    b.HasIndex("ChecklistId");

                    b.HasIndex("DraftId");

                    b.HasIndex("RequestId");

                    b.ToTable("RSC_Requirements");
                });

            modelBuilder.Entity("rscSys_final.Models.StatusHistory", b =>
                {
                    b.Property<int>("StatusHistoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StatusHistoryId"));

                    b.Property<string>("Comments")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RequestId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StatusDate")
                        .HasColumnType("datetime2");

                    b.HasKey("StatusHistoryId");

                    b.HasIndex("RequestId");

                    b.ToTable("RSC_StatusHistories");
                });

            modelBuilder.Entity("rscSys_final.Models.Template", b =>
                {
                    b.Property<string>("TemplateId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<byte[]>("FileData")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("FileType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("FileUploaded")
                        .HasColumnType("datetime2");

                    b.Property<string>("TemplateName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TemplateId");

                    b.ToTable("RSC_Templates");
                });

            modelBuilder.Entity("rscSys_final.Models.ApplicationSubCategory", b =>
                {
                    b.HasOne("rscSys_final.Models.ApplicationType", "ApplicationType")
                        .WithMany("ApplicationSubCategories")
                        .HasForeignKey("ApplicationTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApplicationType");
                });

            modelBuilder.Entity("rscSys_final.Models.Checklist", b =>
                {
                    b.HasOne("rscSys_final.Models.ApplicationSubCategory", "ApplicationSubCategory")
                        .WithMany("Checklists")
                        .HasForeignKey("ApplicationSubCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("rscSys_final.Models.ApplicationType", "ApplicationType")
                        .WithMany("Checklists")
                        .HasForeignKey("ApplicationTypeId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("ApplicationSubCategory");

                    b.Navigation("ApplicationType");
                });

            modelBuilder.Entity("rscSys_final.Models.Criterion", b =>
                {
                    b.HasOne("rscSys_final.Models.EvaluationForm", "EvaluationForm")
                        .WithMany("Criteria")
                        .HasForeignKey("FormId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EvaluationForm");
                });

            modelBuilder.Entity("rscSys_final.Models.DocumentHistory", b =>
                {
                    b.HasOne("rscSys_final.Models.Request", "Request")
                        .WithMany("DocumentHistories")
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Request");
                });

            modelBuilder.Entity("rscSys_final.Models.EvaluationDocument", b =>
                {
                    b.HasOne("rscSys_final.Models.EvaluatorAssignment", "EvaluatorAssignment")
                        .WithMany()
                        .HasForeignKey("EvaluatorAssignmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EvaluatorAssignment");
                });

            modelBuilder.Entity("rscSys_final.Models.EvaluationFormResponse", b =>
                {
                    b.HasOne("rscSys_final.Models.Criterion", "Criterion")
                        .WithMany("EvaluationFormResponses")
                        .HasForeignKey("CriterionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("rscSys_final.Models.EvaluatorAssignment", "EvaluatorAssignment")
                        .WithMany()
                        .HasForeignKey("EvaluatorAssignmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("rscSys_final.Models.Evaluator", "Evaluator")
                        .WithMany("EvaluationFormResponses")
                        .HasForeignKey("EvaluatorId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Criterion");

                    b.Navigation("Evaluator");

                    b.Navigation("EvaluatorAssignment");
                });

            modelBuilder.Entity("rscSys_final.Models.EvaluationGeneralComment", b =>
                {
                    b.HasOne("rscSys_final.Models.EvaluatorAssignment", "EvaluatorAssignment")
                        .WithMany()
                        .HasForeignKey("EvaluatorAssignmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EvaluatorAssignment");
                });

            modelBuilder.Entity("rscSys_final.Models.EvaluatorAssignment", b =>
                {
                    b.HasOne("rscSys_final.Models.Evaluator", "Evaluators")
                        .WithMany()
                        .HasForeignKey("EvaluatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("rscSys_final.Models.Request", "Request")
                        .WithMany("EvaluatorAssignments")
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Evaluators");

                    b.Navigation("Request");
                });

            modelBuilder.Entity("rscSys_final.Models.FinalDocument", b =>
                {
                    b.HasOne("rscSys_final.Models.Request", "Request")
                        .WithMany("FinalDocuments")
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Request");
                });

            modelBuilder.Entity("rscSys_final.Models.Notifications", b =>
                {
                    b.HasOne("rscSys_final.Models.EvaluatorAssignment", "EvaluatorAssignment")
                        .WithMany("Notifications")
                        .HasForeignKey("EvaluatorAssignmentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("EvaluatorAssignment");
                });

            modelBuilder.Entity("rscSys_final.Models.Requirement", b =>
                {
                    b.HasOne("rscSys_final.Models.Checklist", "Checklist")
                        .WithMany("Requirements")
                        .HasForeignKey("ChecklistId");

                    b.HasOne("rscSys_final.Models.Draft", "Draft")
                        .WithMany("Requirements")
                        .HasForeignKey("DraftId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("rscSys_final.Models.Request", "Request")
                        .WithMany("Requirements")
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Checklist");

                    b.Navigation("Draft");

                    b.Navigation("Request");
                });

            modelBuilder.Entity("rscSys_final.Models.StatusHistory", b =>
                {
                    b.HasOne("rscSys_final.Models.Request", "Request")
                        .WithMany("StatusHistories")
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Request");
                });

            modelBuilder.Entity("rscSys_final.Models.ApplicationSubCategory", b =>
                {
                    b.Navigation("Checklists");
                });

            modelBuilder.Entity("rscSys_final.Models.ApplicationType", b =>
                {
                    b.Navigation("ApplicationSubCategories");

                    b.Navigation("Checklists");
                });

            modelBuilder.Entity("rscSys_final.Models.Checklist", b =>
                {
                    b.Navigation("Requirements");
                });

            modelBuilder.Entity("rscSys_final.Models.Criterion", b =>
                {
                    b.Navigation("EvaluationFormResponses");
                });

            modelBuilder.Entity("rscSys_final.Models.Draft", b =>
                {
                    b.Navigation("Requirements");
                });

            modelBuilder.Entity("rscSys_final.Models.EvaluationForm", b =>
                {
                    b.Navigation("Criteria");
                });

            modelBuilder.Entity("rscSys_final.Models.Evaluator", b =>
                {
                    b.Navigation("EvaluationFormResponses");
                });

            modelBuilder.Entity("rscSys_final.Models.EvaluatorAssignment", b =>
                {
                    b.Navigation("Notifications");
                });

            modelBuilder.Entity("rscSys_final.Models.Request", b =>
                {
                    b.Navigation("DocumentHistories");

                    b.Navigation("EvaluatorAssignments");

                    b.Navigation("FinalDocuments");

                    b.Navigation("Requirements");

                    b.Navigation("StatusHistories");
                });
#pragma warning restore 612, 618
        }
    }
}
