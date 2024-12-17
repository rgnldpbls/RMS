﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ResearchManagementSystem.Data;

#nullable disable

namespace ResearchManagementSystem.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("ResearchManagementSystem.Models.AddAccomplishment", b =>
                {
                    b.Property<string>("ProductionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("BranchCampus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CoLeadResearcherId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("College")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("ConditionalFileData")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("ConditionalFileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatedById")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateCompleted")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateStarted")
                        .HasColumnType("datetime2");

                    b.Property<string>("Department")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FundingAgency")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("FundingAmount")
                        .HasColumnType("float");

                    b.Property<bool>("IsStudentInvolved")
                        .HasColumnType("bit");

                    b.Property<string>("LeadResearcherId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("MemberoneId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("MemberthreeId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("MembertwoId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("RequiredFile1Data")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("RequiredFile1Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("RequiredFile2Data")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("RequiredFile2Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("RequiredFile3Data")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("RequiredFile3Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ResearchTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Year")
                        .HasColumnType("int");

                    b.HasKey("ProductionId");

                    b.HasIndex("CoLeadResearcherId");

                    b.HasIndex("CreatedById");

                    b.HasIndex("LeadResearcherId");

                    b.HasIndex("MemberoneId");

                    b.HasIndex("MemberthreeId");

                    b.HasIndex("MembertwoId");

                    b.ToTable("Production", (string)null);
                });

            modelBuilder.Entity("ResearchManagementSystem.Models.AddCitation", b =>
                {
                    b.Property<string>("CitationId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ArticleTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AuthorsofNewArticle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("CitationProofData")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("CitationProofFileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CoLeadResearcherId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Indexing")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LeadResearcherId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("MemberoneId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("MemberthreeId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("MembertwoId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("NewArticlePublished")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NewPublicationTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OriginalArticlePublished")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Pages")
                        .HasColumnType("int");

                    b.Property<string>("ProductionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("VolNoIssueNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("YearofPublication")
                        .HasColumnType("int");

                    b.Property<string>("publicationId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("CitationId");

                    b.HasIndex("CoLeadResearcherId");

                    b.HasIndex("LeadResearcherId");

                    b.HasIndex("MemberoneId");

                    b.HasIndex("MemberthreeId");

                    b.HasIndex("MembertwoId");

                    b.HasIndex("ProductionId");

                    b.HasIndex("publicationId");

                    b.ToTable("Citation", (string)null);
                });

            modelBuilder.Entity("ResearchManagementSystem.Models.AddFAQs", b =>
                {
                    b.Property<string>("FAQ_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("added_by")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("answer_id")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("question_id")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("FAQ_id");

                    b.HasIndex("added_by");

                    b.ToTable("FAQs", (string)null);
                });

            modelBuilder.Entity("ResearchManagementSystem.Models.AddPatent", b =>
                {
                    b.Property<string>("patentId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<byte[]>("ApplicationFormData")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("ApplicationFormFileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PatentNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductionId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("patentId");

                    b.HasIndex("ProductionId");

                    b.ToTable("Patent", (string)null);
                });

            modelBuilder.Entity("ResearchManagementSystem.Models.AddPresentation", b =>
                {
                    b.Property<string>("ConferenceId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<byte[]>("CertificateofParticipationFileData")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("CertificateofParticipationFileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DateofPresentation")
                        .HasColumnType("datetime2");

                    b.Property<string>("Level")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OrganizerOne")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OrganizerTwo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PresenterFive")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PresenterFour")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PresenterOne")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PresenterThree")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PresenterTwo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Venue")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ConferenceId");

                    b.HasIndex("ProductionId");

                    b.ToTable("Presentation", (string)null);
                });

            modelBuilder.Entity("ResearchManagementSystem.Models.AddPublication", b =>
                {
                    b.Property<string>("publicationId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ArticleTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DOI")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DocumentType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IndexJournal")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IssnIsbnEssn")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("JournalPubTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Link")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("ManuscriptJournalData")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("ManuscriptJournalFileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("PublicationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SuppDocs")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VolnoIssueNo")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("publicationId");

                    b.HasIndex("ProductionId");

                    b.ToTable("Publication", (string)null);
                });

            modelBuilder.Entity("ResearchManagementSystem.Models.AddUtilization", b =>
                {
                    b.Property<string>("UtilizationId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<byte[]>("CertificateofUtilizationData")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("CertificateofUtilizationFileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("SubmittedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("UtilizationId");

                    b.HasIndex("ProductionId");

                    b.ToTable("Utilization", (string)null);
                });

            modelBuilder.Entity("ResearchManagementSystem.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<DateOnly>("Birthday")
                        .HasColumnType("date");

                    b.Property<string>("Campus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("College")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Department")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastLoginTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LastLogoutTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("MiddleName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("Rank")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RankEndDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("RankStartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Webmail")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("ResearchManagementSystem.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("ResearchManagementSystem.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ResearchManagementSystem.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("ResearchManagementSystem.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ResearchManagementSystem.Models.AddAccomplishment", b =>
                {
                    b.HasOne("ResearchManagementSystem.Models.ApplicationUser", "CoLeadResearcher")
                        .WithMany()
                        .HasForeignKey("CoLeadResearcherId");

                    b.HasOne("ResearchManagementSystem.Models.ApplicationUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");

                    b.HasOne("ResearchManagementSystem.Models.ApplicationUser", "LeadResearcher")
                        .WithMany()
                        .HasForeignKey("LeadResearcherId");

                    b.HasOne("ResearchManagementSystem.Models.ApplicationUser", "Memberone")
                        .WithMany()
                        .HasForeignKey("MemberoneId");

                    b.HasOne("ResearchManagementSystem.Models.ApplicationUser", "Memberthree")
                        .WithMany()
                        .HasForeignKey("MemberthreeId");

                    b.HasOne("ResearchManagementSystem.Models.ApplicationUser", "Membertwo")
                        .WithMany()
                        .HasForeignKey("MembertwoId");

                    b.Navigation("CoLeadResearcher");

                    b.Navigation("CreatedBy");

                    b.Navigation("LeadResearcher");

                    b.Navigation("Memberone");

                    b.Navigation("Memberthree");

                    b.Navigation("Membertwo");
                });

            modelBuilder.Entity("ResearchManagementSystem.Models.AddCitation", b =>
                {
                    b.HasOne("ResearchManagementSystem.Models.AddAccomplishment", "CoLeadResearcher")
                        .WithMany()
                        .HasForeignKey("CoLeadResearcherId");

                    b.HasOne("ResearchManagementSystem.Models.AddAccomplishment", "LeadResearcher")
                        .WithMany()
                        .HasForeignKey("LeadResearcherId");

                    b.HasOne("ResearchManagementSystem.Models.AddAccomplishment", "Memberone")
                        .WithMany()
                        .HasForeignKey("MemberoneId");

                    b.HasOne("ResearchManagementSystem.Models.AddAccomplishment", "Memberthree")
                        .WithMany()
                        .HasForeignKey("MemberthreeId");

                    b.HasOne("ResearchManagementSystem.Models.AddAccomplishment", "Membertwo")
                        .WithMany()
                        .HasForeignKey("MembertwoId");

                    b.HasOne("ResearchManagementSystem.Models.AddAccomplishment", "AddAccomplishment")
                        .WithMany("AddCitations")
                        .HasForeignKey("ProductionId");

                    b.HasOne("ResearchManagementSystem.Models.AddPublication", "Publication")
                        .WithMany()
                        .HasForeignKey("publicationId");

                    b.Navigation("AddAccomplishment");

                    b.Navigation("CoLeadResearcher");

                    b.Navigation("LeadResearcher");

                    b.Navigation("Memberone");

                    b.Navigation("Memberthree");

                    b.Navigation("Membertwo");

                    b.Navigation("Publication");
                });

            modelBuilder.Entity("ResearchManagementSystem.Models.AddFAQs", b =>
                {
                    b.HasOne("ResearchManagementSystem.Models.ApplicationUser", "superadmin")
                        .WithMany()
                        .HasForeignKey("added_by");

                    b.Navigation("superadmin");
                });

            modelBuilder.Entity("ResearchManagementSystem.Models.AddPatent", b =>
                {
                    b.HasOne("ResearchManagementSystem.Models.AddAccomplishment", "AddAccomplishment")
                        .WithMany("AddPatents")
                        .HasForeignKey("ProductionId");

                    b.Navigation("AddAccomplishment");
                });

            modelBuilder.Entity("ResearchManagementSystem.Models.AddPresentation", b =>
                {
                    b.HasOne("ResearchManagementSystem.Models.AddAccomplishment", "AddAccomplishment")
                        .WithMany("AddPresentations")
                        .HasForeignKey("ProductionId");

                    b.Navigation("AddAccomplishment");
                });

            modelBuilder.Entity("ResearchManagementSystem.Models.AddPublication", b =>
                {
                    b.HasOne("ResearchManagementSystem.Models.AddAccomplishment", "AddAccomplishment")
                        .WithMany("AddPublications")
                        .HasForeignKey("ProductionId");

                    b.Navigation("AddAccomplishment");
                });

            modelBuilder.Entity("ResearchManagementSystem.Models.AddUtilization", b =>
                {
                    b.HasOne("ResearchManagementSystem.Models.AddAccomplishment", "AddAccomplishment")
                        .WithMany("AddUtilizations")
                        .HasForeignKey("ProductionId");

                    b.Navigation("AddAccomplishment");
                });

            modelBuilder.Entity("ResearchManagementSystem.Models.AddAccomplishment", b =>
                {
                    b.Navigation("AddCitations");

                    b.Navigation("AddPatents");

                    b.Navigation("AddPresentations");

                    b.Navigation("AddPublications");

                    b.Navigation("AddUtilizations");
                });
#pragma warning restore 612, 618
        }
    }
}
