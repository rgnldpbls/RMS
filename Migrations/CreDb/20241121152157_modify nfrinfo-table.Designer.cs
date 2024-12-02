﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ResearchManagementSystem.Areas.CreSys.Data;

#nullable disable

namespace ResearchManagementSystem.Migrations.CreDb
{
    [DbContext(typeof(CreDbContext))]
    [Migration("20241121152157_modify nfrinfo-table")]
    partial class modifynfrinfotable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EthicsApplicationFormsEthicsForms", b =>
                {
                    b.Property<int>("EthicsApplicationFormsEthicsApplicationFormId")
                        .HasColumnType("int");

                    b.Property<string>("EthicsFormsEthicsFormId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("EthicsApplicationFormsEthicsApplicationFormId", "EthicsFormsEthicsFormId");

                    b.HasIndex("EthicsFormsEthicsFormId");

                    b.ToTable("EthicsApplicationFormsEthicsForms");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.Chairperson", b =>
                {
                    b.Property<int>("ChairpersonId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ChairpersonId"));

                    b.Property<string>("FieldOfStudy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ChairpersonId");

                    b.ToTable("CRE_Chairperson");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.CoProponent", b =>
                {
                    b.Property<int>("CoProponentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CoProponentId"));

                    b.Property<string>("CoProponentName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NonFundedResearchId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("CoProponentId");

                    b.HasIndex("NonFundedResearchId");

                    b.ToTable("CRE_CoProponents");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.CompletionCertificate", b =>
                {
                    b.Property<int>("CompletionCertId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CompletionCertId"));

                    b.Property<byte[]>("CertificateFile")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<DateOnly>("IssuedDate")
                        .HasColumnType("date");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UrecNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("CompletionCertId");

                    b.HasIndex("UrecNo")
                        .IsUnique();

                    b.ToTable("CRE_CompletionCertificates");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.CompletionReport", b =>
                {
                    b.Property<int>("CompletionReportId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CompletionReportId"));

                    b.Property<DateTime?>("ResearchEndDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ResearchStartDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("SubmissionDate")
                        .HasColumnType("datetime2");

                    b.Property<byte[]>("TerminalReport")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("UrecNo")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("CompletionReportId");

                    b.HasIndex("UrecNo")
                        .IsUnique()
                        .HasFilter("[UrecNo] IS NOT NULL");

                    b.ToTable("CRE_CompletionReports");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.DeclinedEthicsEvaluation", b =>
                {
                    b.Property<int>("DeclinedEvaluationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DeclinedEvaluationId"));

                    b.Property<DateOnly>("DeclineDate")
                        .HasColumnType("date");

                    b.Property<int>("EthicsEvaluatorId")
                        .HasColumnType("int");

                    b.Property<int>("EvaluationId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReasonForDeclining")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UrecNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("DeclinedEvaluationId");

                    b.HasIndex("EthicsEvaluatorId");

                    b.HasIndex("UrecNo")
                        .IsUnique();

                    b.ToTable("CRE_DeclinedEthicsEvaluation");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.EthicsApplication", b =>
                {
                    b.Property<string>("UrecNo")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DtsNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FieldOfStudy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("SubmissionDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UrecNo");

                    b.ToTable("CRE_EthicsApplication");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.EthicsApplicationForms", b =>
                {
                    b.Property<int>("EthicsApplicationFormId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EthicsApplicationFormId"));

                    b.Property<DateTime>("DateUploaded")
                        .HasColumnType("datetime2");

                    b.Property<string>("EthicsFormId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("File")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UrecNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("EthicsApplicationFormId");

                    b.HasIndex("UrecNo");

                    b.ToTable("CRE_EthicsApplicationForms");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.EthicsApplicationLogs", b =>
                {
                    b.Property<int>("LogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LogId"));

                    b.Property<DateTime>("ChangeDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Comments")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UrecNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LogId");

                    b.HasIndex("UrecNo");

                    b.ToTable("CRE_EthicsApplicationLogs");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.EthicsClearance", b =>
                {
                    b.Property<int>("EthicsClearanceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EthicsClearanceId"));

                    b.Property<byte[]>("ClearanceFile")
                        .HasColumnType("varbinary(max)");

                    b.Property<DateTime?>("ExpirationDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("IssuedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UrecNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("EthicsClearanceId");

                    b.HasIndex("UrecNo")
                        .IsUnique();

                    b.ToTable("CRE_EthicsClearance");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.EthicsEvaluation", b =>
                {
                    b.Property<int>("EvaluationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EvaluationId"));

                    b.Property<string>("ConsentRecommendation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConsentRemarks")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("EthicsEvaluatorId")
                        .HasColumnType("int");

                    b.Property<string>("EvaluationStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("InformedConsentForm")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProtocolRecommendation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProtocolRemarks")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("ProtocolReviewSheet")
                        .HasColumnType("varbinary(max)");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UrecNo")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EvaluationId");

                    b.HasIndex("EthicsEvaluatorId");

                    b.HasIndex("UrecNo");

                    b.ToTable("CRE_EthicsEvaluation");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.EthicsEvaluator", b =>
                {
                    b.Property<int>("EthicsEvaluatorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EthicsEvaluatorId"));

                    b.Property<string>("AccountStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Completed")
                        .HasColumnType("int");

                    b.Property<int>("Declined")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Pending")
                        .HasColumnType("int");

                    b.Property<string>("UserID")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EthicsEvaluatorId");

                    b.ToTable("CRE_EthicsEvaluator");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.EthicsEvaluatorExpertise", b =>
                {
                    b.Property<int>("EthicsEvaluatorId")
                        .HasColumnType("int");

                    b.Property<int>("ExpertiseId")
                        .HasColumnType("int");

                    b.HasKey("EthicsEvaluatorId", "ExpertiseId");

                    b.HasIndex("ExpertiseId");

                    b.ToTable("CRE_EthicsEvaluatorExpertise");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.EthicsForms", b =>
                {
                    b.Property<string>("EthicsFormId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<byte[]>("EthicsFormFile")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("FormDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FormName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsRequired")
                        .HasColumnType("bit");

                    b.HasKey("EthicsFormId");

                    b.ToTable("CRE_EthicsForms");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.EthicsMemoranda", b =>
                {
                    b.Property<int>("MemoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MemoId"));

                    b.Property<string>("MemoDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("MemoFile")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("MemoName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MemoNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MemoId");

                    b.ToTable("CRE_EthicsMemoranda");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.EthicsNotifications", b =>
                {
                    b.Property<int>("NotificationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("NotificationId"));

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

                    b.Property<string>("UrecNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("NotificationId");

                    b.HasIndex("UrecNo");

                    b.ToTable("CRE_EthicsNotifications");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.EthicsReport", b =>
                {
                    b.Property<string>("EthicsReportId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("College")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateGenerated")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsArchived")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ReportEndDate")
                        .HasColumnType("datetime2");

                    b.Property<byte[]>("ReportFile")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("ReportFileType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReportName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ReportStartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EthicsReportId");

                    b.ToTable("CRE_EthicsReport");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.Expertise", b =>
                {
                    b.Property<int>("ExpertiseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ExpertiseId"));

                    b.Property<string>("ExpertiseName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ExpertiseId");

                    b.ToTable("CRE_Expertise");

                    b.HasData(
                        new
                        {
                            ExpertiseId = 1,
                            ExpertiseName = "Education"
                        },
                        new
                        {
                            ExpertiseId = 2,
                            ExpertiseName = "Computer Science, Information Systems, and Technology"
                        },
                        new
                        {
                            ExpertiseId = 3,
                            ExpertiseName = "Engineering, Architecture, and Design"
                        },
                        new
                        {
                            ExpertiseId = 4,
                            ExpertiseName = "Humanities, Language, and Communication"
                        },
                        new
                        {
                            ExpertiseId = 5,
                            ExpertiseName = "Business"
                        },
                        new
                        {
                            ExpertiseId = 6,
                            ExpertiseName = "Social Sciences"
                        },
                        new
                        {
                            ExpertiseId = 7,
                            ExpertiseName = "Science, Mathematics, and Statistics"
                        });
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.InitialReview", b =>
                {
                    b.Property<int>("InitalReviewId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("InitalReviewId"));

                    b.Property<DateTime>("DateReviewed")
                        .HasColumnType("datetime2");

                    b.Property<string>("Feedback")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReviewType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UrecNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("InitalReviewId");

                    b.HasIndex("UrecNo")
                        .IsUnique();

                    b.ToTable("CRE_InitialReview");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.NonFundedResearchInfo", b =>
                {
                    b.Property<string>("NonFundedResearchId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Campus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("College")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CompletionCertId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CompletionDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateSubmitted")
                        .HasColumnType("datetime2");

                    b.Property<int?>("EthicsClearanceId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("University")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UrecNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("NonFundedResearchId");

                    b.HasIndex("CompletionCertId")
                        .IsUnique()
                        .HasFilter("[CompletionCertId] IS NOT NULL");

                    b.HasIndex("EthicsClearanceId");

                    b.HasIndex("UrecNo")
                        .IsUnique();

                    b.ToTable("CRE_NonFundedResearchInfo");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.ReceiptInfo", b =>
                {
                    b.Property<string>("ReceiptNo")
                        .HasColumnType("nvarchar(450)");

                    b.Property<float>("AmountPaid")
                        .HasColumnType("real");

                    b.Property<DateTime>("DatePaid")
                        .HasColumnType("datetime2");

                    b.Property<byte[]>("ScanReceipt")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("UrecNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ReceiptNo");

                    b.HasIndex("UrecNo")
                        .IsUnique();

                    b.ToTable("CRE_ReceiptInfo");
                });

            modelBuilder.Entity("EthicsApplicationFormsEthicsForms", b =>
                {
                    b.HasOne("ResearchManagementSystem.Areas.CreSys.Models.EthicsApplicationForms", null)
                        .WithMany()
                        .HasForeignKey("EthicsApplicationFormsEthicsApplicationFormId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ResearchManagementSystem.Areas.CreSys.Models.EthicsForms", null)
                        .WithMany()
                        .HasForeignKey("EthicsFormsEthicsFormId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.CoProponent", b =>
                {
                    b.HasOne("ResearchManagementSystem.Areas.CreSys.Models.NonFundedResearchInfo", "NonFundedResearchInfo")
                        .WithMany("CoProponents")
                        .HasForeignKey("NonFundedResearchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("NonFundedResearchInfo");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.CompletionCertificate", b =>
                {
                    b.HasOne("ResearchManagementSystem.Areas.CreSys.Models.EthicsApplication", "EthicsApplication")
                        .WithOne("CompletionCertificate")
                        .HasForeignKey("ResearchManagementSystem.Areas.CreSys.Models.CompletionCertificate", "UrecNo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EthicsApplication");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.CompletionReport", b =>
                {
                    b.HasOne("ResearchManagementSystem.Areas.CreSys.Models.EthicsApplication", "EthicsApplication")
                        .WithOne("CompletionReport")
                        .HasForeignKey("ResearchManagementSystem.Areas.CreSys.Models.CompletionReport", "UrecNo");

                    b.Navigation("EthicsApplication");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.DeclinedEthicsEvaluation", b =>
                {
                    b.HasOne("ResearchManagementSystem.Areas.CreSys.Models.EthicsEvaluator", "EthicsEvaluator")
                        .WithMany("DeclinedEthicsEvaluation")
                        .HasForeignKey("EthicsEvaluatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ResearchManagementSystem.Areas.CreSys.Models.EthicsApplication", "EthicsApplication")
                        .WithOne("DeclinedEthicsEvaluation")
                        .HasForeignKey("ResearchManagementSystem.Areas.CreSys.Models.DeclinedEthicsEvaluation", "UrecNo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EthicsApplication");

                    b.Navigation("EthicsEvaluator");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.EthicsApplicationForms", b =>
                {
                    b.HasOne("ResearchManagementSystem.Areas.CreSys.Models.EthicsApplication", "EthicsApplication")
                        .WithMany("EthicsApplicationForms")
                        .HasForeignKey("UrecNo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EthicsApplication");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.EthicsApplicationLogs", b =>
                {
                    b.HasOne("ResearchManagementSystem.Areas.CreSys.Models.EthicsApplication", "EthicsApplication")
                        .WithMany("EthicsApplicationLogs")
                        .HasForeignKey("UrecNo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EthicsApplication");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.EthicsClearance", b =>
                {
                    b.HasOne("ResearchManagementSystem.Areas.CreSys.Models.EthicsApplication", "EthicsApplication")
                        .WithOne("EthicsClearance")
                        .HasForeignKey("ResearchManagementSystem.Areas.CreSys.Models.EthicsClearance", "UrecNo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EthicsApplication");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.EthicsEvaluation", b =>
                {
                    b.HasOne("ResearchManagementSystem.Areas.CreSys.Models.EthicsEvaluator", "EthicsEvaluator")
                        .WithMany("EthicsEvaluation")
                        .HasForeignKey("EthicsEvaluatorId");

                    b.HasOne("ResearchManagementSystem.Areas.CreSys.Models.EthicsApplication", "EthicsApplication")
                        .WithMany("EthicsEvaluation")
                        .HasForeignKey("UrecNo");

                    b.Navigation("EthicsApplication");

                    b.Navigation("EthicsEvaluator");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.EthicsEvaluatorExpertise", b =>
                {
                    b.HasOne("ResearchManagementSystem.Areas.CreSys.Models.EthicsEvaluator", "EthicsEvaluator")
                        .WithMany("EthicsEvaluatorExpertises")
                        .HasForeignKey("EthicsEvaluatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ResearchManagementSystem.Areas.CreSys.Models.Expertise", "Expertise")
                        .WithMany("EthicsEvaluatorExpertise")
                        .HasForeignKey("ExpertiseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EthicsEvaluator");

                    b.Navigation("Expertise");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.EthicsNotifications", b =>
                {
                    b.HasOne("ResearchManagementSystem.Areas.CreSys.Models.EthicsApplication", "EthicsApplication")
                        .WithMany("EthicsNotifications")
                        .HasForeignKey("UrecNo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EthicsApplication");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.InitialReview", b =>
                {
                    b.HasOne("ResearchManagementSystem.Areas.CreSys.Models.EthicsApplication", "EthicsApplication")
                        .WithOne("InitialReview")
                        .HasForeignKey("ResearchManagementSystem.Areas.CreSys.Models.InitialReview", "UrecNo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EthicsApplication");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.NonFundedResearchInfo", b =>
                {
                    b.HasOne("ResearchManagementSystem.Areas.CreSys.Models.CompletionCertificate", "CompletionCertificate")
                        .WithOne("NonFundedResearchInfo")
                        .HasForeignKey("ResearchManagementSystem.Areas.CreSys.Models.NonFundedResearchInfo", "CompletionCertId");

                    b.HasOne("ResearchManagementSystem.Areas.CreSys.Models.EthicsClearance", "EthicsClearance")
                        .WithMany()
                        .HasForeignKey("EthicsClearanceId");

                    b.HasOne("ResearchManagementSystem.Areas.CreSys.Models.EthicsApplication", "EthicsApplication")
                        .WithOne("NonFundedResearchInfo")
                        .HasForeignKey("ResearchManagementSystem.Areas.CreSys.Models.NonFundedResearchInfo", "UrecNo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CompletionCertificate");

                    b.Navigation("EthicsApplication");

                    b.Navigation("EthicsClearance");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.ReceiptInfo", b =>
                {
                    b.HasOne("ResearchManagementSystem.Areas.CreSys.Models.EthicsApplication", "EthicsApplication")
                        .WithOne("ReceiptInfo")
                        .HasForeignKey("ResearchManagementSystem.Areas.CreSys.Models.ReceiptInfo", "UrecNo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EthicsApplication");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.CompletionCertificate", b =>
                {
                    b.Navigation("NonFundedResearchInfo")
                        .IsRequired();
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.EthicsApplication", b =>
                {
                    b.Navigation("CompletionCertificate")
                        .IsRequired();

                    b.Navigation("CompletionReport")
                        .IsRequired();

                    b.Navigation("DeclinedEthicsEvaluation")
                        .IsRequired();

                    b.Navigation("EthicsApplicationForms");

                    b.Navigation("EthicsApplicationLogs");

                    b.Navigation("EthicsClearance")
                        .IsRequired();

                    b.Navigation("EthicsEvaluation");

                    b.Navigation("EthicsNotifications");

                    b.Navigation("InitialReview")
                        .IsRequired();

                    b.Navigation("NonFundedResearchInfo")
                        .IsRequired();

                    b.Navigation("ReceiptInfo")
                        .IsRequired();
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.EthicsEvaluator", b =>
                {
                    b.Navigation("DeclinedEthicsEvaluation");

                    b.Navigation("EthicsEvaluation");

                    b.Navigation("EthicsEvaluatorExpertises");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.Expertise", b =>
                {
                    b.Navigation("EthicsEvaluatorExpertise");
                });

            modelBuilder.Entity("ResearchManagementSystem.Areas.CreSys.Models.NonFundedResearchInfo", b =>
                {
                    b.Navigation("CoProponents");
                });
#pragma warning restore 612, 618
        }
    }
}