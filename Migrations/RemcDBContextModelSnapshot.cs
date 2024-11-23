﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RemcSys.Data;

#nullable disable

namespace ResearchManagementSystem.Migrations
{
    [DbContext(typeof(RemcDBContext))]
    partial class RemcDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("RemcSys.Models.ActionLog", b =>
                {
                    b.Property<string>("LogId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Action")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FraId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ResearchType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<bool>("isChief")
                        .HasColumnType("bit");

                    b.Property<bool>("isEvaluator")
                        .HasColumnType("bit");

                    b.Property<bool>("isTeamLeader")
                        .HasColumnType("bit");

                    b.HasKey("LogId");

                    b.HasIndex("FraId");

                    b.ToTable("REMC_ActionLogs", (string)null);
                });

            modelBuilder.Entity("RemcSys.Models.CalendarEvent", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("End")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Start")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Visibility")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("REMC_CalendarEvents", (string)null);
                });

            modelBuilder.Entity("RemcSys.Models.Criteria", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Weight")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("REMC_Criterias", (string)null);
                });

            modelBuilder.Entity("RemcSys.Models.Evaluation", b =>
                {
                    b.Property<string>("evaluation_Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("assigned_Date")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("evaluation_Date")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("evaluation_Deadline")
                        .HasColumnType("datetime2");

                    b.Property<double?>("evaluation_Grade")
                        .HasColumnType("float");

                    b.Property<string>("evaluation_Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("evaluator_Id")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("evaluator_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("fra_Id")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("reminded_OneDayBefore")
                        .HasColumnType("bit");

                    b.Property<bool>("reminded_OneDayOverdue")
                        .HasColumnType("bit");

                    b.Property<bool>("reminded_SevenDaysOverdue")
                        .HasColumnType("bit");

                    b.Property<bool>("reminded_ThreeDaysBefore")
                        .HasColumnType("bit");

                    b.Property<bool>("reminded_ThreeDaysOverdue")
                        .HasColumnType("bit");

                    b.Property<bool>("reminded_Today")
                        .HasColumnType("bit");

                    b.HasKey("evaluation_Id");

                    b.HasIndex("evaluator_Id");

                    b.HasIndex("fra_Id");

                    b.ToTable("REMC_Evaluations", (string)null);
                });

            modelBuilder.Entity("RemcSys.Models.Evaluator", b =>
                {
                    b.Property<string>("evaluator_Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("evaluator_Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("evaluator_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("field_of_Interest")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("evaluator_Id");

                    b.ToTable("REMC_Evaluator", (string)null);
                });

            modelBuilder.Entity("RemcSys.Models.FileRequirement", b =>
                {
                    b.Property<string>("fr_Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<byte[]>("data")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("document_Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("file_Feedback")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("file_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("file_Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("file_Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("file_Uploaded")
                        .HasColumnType("datetime2");

                    b.Property<string>("fra_Id")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("fr_Id");

                    b.HasIndex("fra_Id");

                    b.ToTable("REMC_FileRequirement", (string)null);
                });

            modelBuilder.Entity("RemcSys.Models.FundedResearch", b =>
                {
                    b.Property<string>("fr_Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("branch")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("college")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("dts_No")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("end_Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("field_of_Study")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("fr_Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("fra_Id")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("isArchive")
                        .HasColumnType("bit");

                    b.Property<bool>("isExtension1")
                        .HasColumnType("bit");

                    b.Property<bool>("isExtension2")
                        .HasColumnType("bit");

                    b.Property<int>("project_Duration")
                        .HasColumnType("int");

                    b.Property<bool>("reminded_OneDayBefore")
                        .HasColumnType("bit");

                    b.Property<bool>("reminded_OneDayOverdue")
                        .HasColumnType("bit");

                    b.Property<bool>("reminded_SevenDaysOverdue")
                        .HasColumnType("bit");

                    b.Property<bool>("reminded_ThreeDaysBefore")
                        .HasColumnType("bit");

                    b.Property<bool>("reminded_ThreeDaysOverdue")
                        .HasColumnType("bit");

                    b.Property<bool>("reminded_Today")
                        .HasColumnType("bit");

                    b.Property<string>("research_Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("start_Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("teamLead_Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("team_Leader")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("team_Members")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("total_project_Cost")
                        .HasColumnType("float");

                    b.HasKey("fr_Id");

                    b.HasIndex("fra_Id")
                        .IsUnique();

                    b.ToTable("REMC_FundedResearches", (string)null);
                });

            modelBuilder.Entity("RemcSys.Models.FundedResearchApplication", b =>
                {
                    b.Property<string>("fra_Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("applicant_Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("applicant_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("application_Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("branch")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("college")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("dts_No")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("field_of_Study")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("fra_Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isArchive")
                        .HasColumnType("bit");

                    b.Property<int>("project_Duration")
                        .HasColumnType("int");

                    b.Property<string>("research_Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("submission_Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("team_Members")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("total_project_Cost")
                        .HasColumnType("float");

                    b.HasKey("fra_Id");

                    b.ToTable("REMC_FundedResearchApplication", (string)null);
                });

            modelBuilder.Entity("RemcSys.Models.FundedResearchEthics", b =>
                {
                    b.Property<string>("fre_Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<byte[]>("clearanceFile")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("file_Feedback")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("file_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("file_Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("file_Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("file_Uploaded")
                        .HasColumnType("datetime2");

                    b.Property<string>("fra_Id")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("urecNo")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("fre_Id");

                    b.HasIndex("fra_Id")
                        .IsUnique();

                    b.ToTable("REMC_FundedResearchEthics", (string)null);
                });

            modelBuilder.Entity("RemcSys.Models.GenerateGAWADNominees", b =>
                {
                    b.Property<string>("gn_Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("generateDate")
                        .HasColumnType("datetime2");

                    b.Property<byte[]>("gn_Data")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("gn_fileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("gn_fileType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("gn_type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isArchived")
                        .HasColumnType("bit");

                    b.HasKey("gn_Id");

                    b.ToTable("REMC_GenerateGAWADNominees", (string)null);
                });

            modelBuilder.Entity("RemcSys.Models.GenerateReport", b =>
                {
                    b.Property<string>("gr_Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("generateDate")
                        .HasColumnType("datetime2");

                    b.Property<byte[]>("gr_Data")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<DateTime>("gr_endDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("gr_fileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("gr_fileType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("gr_startDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("gr_typeofReport")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isArchived")
                        .HasColumnType("bit");

                    b.HasKey("gr_Id");

                    b.ToTable("REMC_GenerateReports", (string)null);
                });

            modelBuilder.Entity("RemcSys.Models.GeneratedForm", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<byte[]>("FileContent")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("GeneratedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("fra_Id")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("fra_Id");

                    b.ToTable("REMC_GeneratedForms", (string)null);
                });

            modelBuilder.Entity("RemcSys.Models.Guidelines", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<byte[]>("data")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("document_Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("file_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("file_Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("file_Uploaded")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("REMC_Guidelines", (string)null);
                });

            modelBuilder.Entity("RemcSys.Models.ProgressReport", b =>
                {
                    b.Property<string>("pr_Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<byte[]>("data")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("document_Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("file_Feedback")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("file_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("file_Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("file_Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("file_Uploaded")
                        .HasColumnType("datetime2");

                    b.Property<string>("fr_Id")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("pr_Id");

                    b.HasIndex("fr_Id");

                    b.ToTable("REMC_ProgressReports", (string)null);
                });

            modelBuilder.Entity("RemcSys.Models.Settings", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("daysEvaluation")
                        .HasColumnType("int");

                    b.Property<int>("evaluatorNum")
                        .HasColumnType("int");

                    b.Property<bool>("isEFRApplication")
                        .HasColumnType("bit");

                    b.Property<bool>("isMaintenance")
                        .HasColumnType("bit");

                    b.Property<bool>("isUFRApplication")
                        .HasColumnType("bit");

                    b.Property<bool>("isUFRLApplication")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("REMC_Settings", (string)null);
                });

            modelBuilder.Entity("RemcSys.Models.SubCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CriteriaId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MaxScore")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CriteriaId");

                    b.ToTable("REMC_SubCategories", (string)null);
                });

            modelBuilder.Entity("RemcSys.Models.UFRForecasting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<float>("ProjectCosts")
                        .HasColumnType("real");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("REMC_UFRForecastings", (string)null);
                });

            modelBuilder.Entity("RemcSys.Models.ActionLog", b =>
                {
                    b.HasOne("RemcSys.Models.FundedResearchApplication", "fundedResearchApplication")
                        .WithMany("ActionLogs")
                        .HasForeignKey("FraId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("fundedResearchApplication");
                });

            modelBuilder.Entity("RemcSys.Models.Evaluation", b =>
                {
                    b.HasOne("RemcSys.Models.Evaluator", "evaluator")
                        .WithMany("Evaluations")
                        .HasForeignKey("evaluator_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RemcSys.Models.FundedResearchApplication", "fundedResearchApplication")
                        .WithMany("Evaluations")
                        .HasForeignKey("fra_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("evaluator");

                    b.Navigation("fundedResearchApplication");
                });

            modelBuilder.Entity("RemcSys.Models.FileRequirement", b =>
                {
                    b.HasOne("RemcSys.Models.FundedResearchApplication", "fundedResearchApplication")
                        .WithMany("FileRequirements")
                        .HasForeignKey("fra_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("fundedResearchApplication");
                });

            modelBuilder.Entity("RemcSys.Models.FundedResearch", b =>
                {
                    b.HasOne("RemcSys.Models.FundedResearchApplication", "FundedResearchApplication")
                        .WithOne("FundedResearch")
                        .HasForeignKey("RemcSys.Models.FundedResearch", "fra_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FundedResearchApplication");
                });

            modelBuilder.Entity("RemcSys.Models.FundedResearchEthics", b =>
                {
                    b.HasOne("RemcSys.Models.FundedResearchApplication", "FundedResearchApplication")
                        .WithOne("FundedResearchEthics")
                        .HasForeignKey("RemcSys.Models.FundedResearchEthics", "fra_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FundedResearchApplication");
                });

            modelBuilder.Entity("RemcSys.Models.GeneratedForm", b =>
                {
                    b.HasOne("RemcSys.Models.FundedResearchApplication", "FundedResearchApplication")
                        .WithMany("GeneratedForms")
                        .HasForeignKey("fra_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FundedResearchApplication");
                });

            modelBuilder.Entity("RemcSys.Models.ProgressReport", b =>
                {
                    b.HasOne("RemcSys.Models.FundedResearch", "FundedResearch")
                        .WithMany("ProgressReports")
                        .HasForeignKey("fr_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FundedResearch");
                });

            modelBuilder.Entity("RemcSys.Models.SubCategory", b =>
                {
                    b.HasOne("RemcSys.Models.Criteria", "criteria")
                        .WithMany("subCategory")
                        .HasForeignKey("CriteriaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("criteria");
                });

            modelBuilder.Entity("RemcSys.Models.Criteria", b =>
                {
                    b.Navigation("subCategory");
                });

            modelBuilder.Entity("RemcSys.Models.Evaluator", b =>
                {
                    b.Navigation("Evaluations");
                });

            modelBuilder.Entity("RemcSys.Models.FundedResearch", b =>
                {
                    b.Navigation("ProgressReports");
                });

            modelBuilder.Entity("RemcSys.Models.FundedResearchApplication", b =>
                {
                    b.Navigation("ActionLogs");

                    b.Navigation("Evaluations");

                    b.Navigation("FileRequirements");

                    b.Navigation("FundedResearch")
                        .IsRequired();

                    b.Navigation("FundedResearchEthics")
                        .IsRequired();

                    b.Navigation("GeneratedForms");
                });
#pragma warning restore 612, 618
        }
    }
}
