using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResearchManagementSystem.Migrations.CrdlDb
{
    /// <inheritdoc />
    public partial class addcrdltables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CRDL_ChiefUpload",
                columns: table => new
                {
                    DocumentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NameOfDocument = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeOfDocument = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentFile = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    DocumentDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StakeholderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailOfStakeholder = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ContractStartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ContractEndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContractStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false),
                    LastNotificationSent = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsManuallyUnarchived = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRDL_ChiefUpload", x => x.DocumentId);
                });

            migrationBuilder.CreateTable(
                name: "CRDL_ResearchEvent",
                columns: table => new
                {
                    ResearchEventId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EventName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventThumbnail = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegistrationOpen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegistrationDeadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParticipantsSlot = table.Column<int>(type: "int", nullable: false),
                    ParticipantsCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRDL_ResearchEvent", x => x.ResearchEventId);
                });

            migrationBuilder.CreateTable(
                name: "CRDL_StakeholderUpload",
                columns: table => new
                {
                    DocumentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StakeholderId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StakeholderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StakeholdeEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameOfDocument = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeOfDocument = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeOfMOA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentFile = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    DocumentDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ContractStartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ContractEndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContractStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false),
                    IsManuallyUnarchived = table.Column<bool>(type: "bit", nullable: false),
                    LastNotificationSent = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRDL_StakeholderUpload", x => x.DocumentId);
                });

            migrationBuilder.CreateTable(
                name: "CRDL_GeneratedReport",
                columns: table => new
                {
                    ReportId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeOfReport = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GenerateReportFile = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: true),
                    ResearchEventId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRDL_GeneratedReport", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK_CRDL_GeneratedReport_CRDL_ResearchEvent_ResearchEventId",
                        column: x => x.ResearchEventId,
                        principalTable: "CRDL_ResearchEvent",
                        principalColumn: "ResearchEventId");
                });

            migrationBuilder.CreateTable(
                name: "CRDL_GeneratedSentimentAnalysis",
                columns: table => new
                {
                    SentimentAnalysisId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GenerateReportFile = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    SurveyFile = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ResearchEventId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRDL_GeneratedSentimentAnalysis", x => x.SentimentAnalysisId);
                    table.ForeignKey(
                        name: "FK_CRDL_GeneratedSentimentAnalysis_CRDL_ResearchEvent_ResearchEventId",
                        column: x => x.ResearchEventId,
                        principalTable: "CRDL_ResearchEvent",
                        principalColumn: "ResearchEventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CRDL_ResearchEventInvitation",
                columns: table => new
                {
                    InvitationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ResearchEventId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InvitationStatus = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    InvitedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRDL_ResearchEventInvitation", x => x.InvitationId);
                    table.ForeignKey(
                        name: "FK_CRDL_ResearchEventInvitation_CRDL_ResearchEvent_ResearchEventId",
                        column: x => x.ResearchEventId,
                        principalTable: "CRDL_ResearchEvent",
                        principalColumn: "ResearchEventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CRDL_ResearchEventRegistration",
                columns: table => new
                {
                    RegistrationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ResearchEventId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastNotificationSent = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRDL_ResearchEventRegistration", x => x.RegistrationId);
                    table.ForeignKey(
                        name: "FK_CRDL_ResearchEventRegistration_CRDL_ResearchEvent_ResearchEventId",
                        column: x => x.ResearchEventId,
                        principalTable: "CRDL_ResearchEvent",
                        principalColumn: "ResearchEventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CRDL_DocumentTracking",
                columns: table => new
                {
                    TrackingId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DocumentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsReceivedByRMO = table.Column<bool>(type: "bit", nullable: false),
                    IsReceivedByRMOUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsSubmittedToOVPRED = table.Column<bool>(type: "bit", nullable: false),
                    IsSubmittedToOVPREDUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsSubmittedToLegalOffice = table.Column<bool>(type: "bit", nullable: false),
                    IsSubmittedToLegalOfficeUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsReceivedByOVPRED = table.Column<bool>(type: "bit", nullable: false),
                    IsReceivedByOVPREDUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsReceivedByRMOAfterOVPRED = table.Column<bool>(type: "bit", nullable: false),
                    IsReceivedByRMOAfterOVPREDUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsSubmittedToOfficeOfThePresident = table.Column<bool>(type: "bit", nullable: false),
                    IsSubmittedToOfficeOfThePresidentUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsReceivedByRMOAfterOfficeOfThePresident = table.Column<bool>(type: "bit", nullable: false),
                    IsReceivedByRMOAfterOfficeOfThePresidentUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRDL_DocumentTracking", x => x.TrackingId);
                    table.ForeignKey(
                        name: "FK_CRDL_DocumentTracking_CRDL_StakeholderUpload_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "CRDL_StakeholderUpload",
                        principalColumn: "DocumentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CRDL_RenewalHistory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DocumentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TypeOfDocument = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreviousEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    NewEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RenewalDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRDL_RenewalHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CRDL_RenewalHistory_CRDL_StakeholderUpload_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "CRDL_StakeholderUpload",
                        principalColumn: "DocumentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CRDL_DocumentTracking_DocumentId",
                table: "CRDL_DocumentTracking",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_CRDL_GeneratedReport_ResearchEventId",
                table: "CRDL_GeneratedReport",
                column: "ResearchEventId");

            migrationBuilder.CreateIndex(
                name: "IX_CRDL_GeneratedSentimentAnalysis_ResearchEventId",
                table: "CRDL_GeneratedSentimentAnalysis",
                column: "ResearchEventId");

            migrationBuilder.CreateIndex(
                name: "IX_CRDL_RenewalHistory_DocumentId",
                table: "CRDL_RenewalHistory",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_CRDL_ResearchEventInvitation_ResearchEventId",
                table: "CRDL_ResearchEventInvitation",
                column: "ResearchEventId");

            migrationBuilder.CreateIndex(
                name: "IX_CRDL_ResearchEventRegistration_ResearchEventId",
                table: "CRDL_ResearchEventRegistration",
                column: "ResearchEventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CRDL_ChiefUpload");

            migrationBuilder.DropTable(
                name: "CRDL_DocumentTracking");

            migrationBuilder.DropTable(
                name: "CRDL_GeneratedReport");

            migrationBuilder.DropTable(
                name: "CRDL_GeneratedSentimentAnalysis");

            migrationBuilder.DropTable(
                name: "CRDL_RenewalHistory");

            migrationBuilder.DropTable(
                name: "CRDL_ResearchEventInvitation");

            migrationBuilder.DropTable(
                name: "CRDL_ResearchEventRegistration");

            migrationBuilder.DropTable(
                name: "CRDL_StakeholderUpload");

            migrationBuilder.DropTable(
                name: "CRDL_ResearchEvent");
        }
    }
}
