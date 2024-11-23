using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ResearchManagementSystem.Migrations.CreDb
{
    /// <inheritdoc />
    public partial class addcredb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CRE_Chairperson",
                columns: table => new
                {
                    ChairpersonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FieldOfStudy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRE_Chairperson", x => x.ChairpersonId);
                });

            migrationBuilder.CreateTable(
                name: "CRE_EthicsApplication",
                columns: table => new
                {
                    UrecNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DtsNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FieldOfStudy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRE_EthicsApplication", x => x.UrecNo);
                });

            migrationBuilder.CreateTable(
                name: "CRE_EthicsEvaluator",
                columns: table => new
                {
                    EthicsEvaluatorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Completed = table.Column<int>(type: "int", nullable: false),
                    Pending = table.Column<int>(type: "int", nullable: false),
                    Declined = table.Column<int>(type: "int", nullable: false),
                    AccountStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRE_EthicsEvaluator", x => x.EthicsEvaluatorId);
                });

            migrationBuilder.CreateTable(
                name: "CRE_EthicsForms",
                columns: table => new
                {
                    EthicsFormId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FormName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EthicsFormFile = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRE_EthicsForms", x => x.EthicsFormId);
                });

            migrationBuilder.CreateTable(
                name: "CRE_EthicsMemoranda",
                columns: table => new
                {
                    MemoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemoNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MemoName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemoDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MemoFile = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRE_EthicsMemoranda", x => x.MemoId);
                });

            migrationBuilder.CreateTable(
                name: "CRE_EthicsReport",
                columns: table => new
                {
                    EthicsReportId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReportName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportFileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportFile = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ReportStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReportEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    College = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateGenerated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRE_EthicsReport", x => x.EthicsReportId);
                });

            migrationBuilder.CreateTable(
                name: "CRE_Expertise",
                columns: table => new
                {
                    ExpertiseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpertiseName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRE_Expertise", x => x.ExpertiseId);
                });

            migrationBuilder.CreateTable(
                name: "CRE_CompletionCertificates",
                columns: table => new
                {
                    CompletionCertId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrecNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssuedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CertificateFile = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRE_CompletionCertificates", x => x.CompletionCertId);
                    table.ForeignKey(
                        name: "FK_CRE_CompletionCertificates_CRE_EthicsApplication_UrecNo",
                        column: x => x.UrecNo,
                        principalTable: "CRE_EthicsApplication",
                        principalColumn: "UrecNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CRE_CompletionReports",
                columns: table => new
                {
                    CompletionReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrecNo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TerminalReport = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ResearchStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResearchEndDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRE_CompletionReports", x => x.CompletionReportId);
                    table.ForeignKey(
                        name: "FK_CRE_CompletionReports_CRE_EthicsApplication_UrecNo",
                        column: x => x.UrecNo,
                        principalTable: "CRE_EthicsApplication",
                        principalColumn: "UrecNo");
                });

            migrationBuilder.CreateTable(
                name: "CRE_EthicsApplicationForms",
                columns: table => new
                {
                    EthicsApplicationFormId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrecNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EthicsFormId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateUploaded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    File = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRE_EthicsApplicationForms", x => x.EthicsApplicationFormId);
                    table.ForeignKey(
                        name: "FK_CRE_EthicsApplicationForms_CRE_EthicsApplication_UrecNo",
                        column: x => x.UrecNo,
                        principalTable: "CRE_EthicsApplication",
                        principalColumn: "UrecNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CRE_EthicsApplicationLogs",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrecNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRE_EthicsApplicationLogs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_CRE_EthicsApplicationLogs_CRE_EthicsApplication_UrecNo",
                        column: x => x.UrecNo,
                        principalTable: "CRE_EthicsApplication",
                        principalColumn: "UrecNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CRE_EthicsClearance",
                columns: table => new
                {
                    EthicsClearanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrecNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IssuedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClearanceFile = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRE_EthicsClearance", x => x.EthicsClearanceId);
                    table.ForeignKey(
                        name: "FK_CRE_EthicsClearance_CRE_EthicsApplication_UrecNo",
                        column: x => x.UrecNo,
                        principalTable: "CRE_EthicsApplication",
                        principalColumn: "UrecNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CRE_EthicsNotifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrecNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotificationTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotificationMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotificationCreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NotificationStatus = table.Column<bool>(type: "bit", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PerformedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRE_EthicsNotifications", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_CRE_EthicsNotifications_CRE_EthicsApplication_UrecNo",
                        column: x => x.UrecNo,
                        principalTable: "CRE_EthicsApplication",
                        principalColumn: "UrecNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CRE_InitialReview",
                columns: table => new
                {
                    InitalReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrecNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateReviewed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Feedback = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRE_InitialReview", x => x.InitalReviewId);
                    table.ForeignKey(
                        name: "FK_CRE_InitialReview_CRE_EthicsApplication_UrecNo",
                        column: x => x.UrecNo,
                        principalTable: "CRE_EthicsApplication",
                        principalColumn: "UrecNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CRE_ReceiptInfo",
                columns: table => new
                {
                    ReceiptNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UrecNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AmountPaid = table.Column<float>(type: "real", nullable: false),
                    DatePaid = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScanReceipt = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRE_ReceiptInfo", x => x.ReceiptNo);
                    table.ForeignKey(
                        name: "FK_CRE_ReceiptInfo_CRE_EthicsApplication_UrecNo",
                        column: x => x.UrecNo,
                        principalTable: "CRE_EthicsApplication",
                        principalColumn: "UrecNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CRE_DeclinedEthicsEvaluation",
                columns: table => new
                {
                    DeclinedEvaluationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EvaluationId = table.Column<int>(type: "int", nullable: false),
                    UrecNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReasonForDeclining = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeclineDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EthicsEvaluatorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRE_DeclinedEthicsEvaluation", x => x.DeclinedEvaluationId);
                    table.ForeignKey(
                        name: "FK_CRE_DeclinedEthicsEvaluation_CRE_EthicsApplication_UrecNo",
                        column: x => x.UrecNo,
                        principalTable: "CRE_EthicsApplication",
                        principalColumn: "UrecNo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CRE_DeclinedEthicsEvaluation_CRE_EthicsEvaluator_EthicsEvaluatorId",
                        column: x => x.EthicsEvaluatorId,
                        principalTable: "CRE_EthicsEvaluator",
                        principalColumn: "EthicsEvaluatorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CRE_EthicsEvaluation",
                columns: table => new
                {
                    EvaluationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrecNo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EthicsEvaluatorId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EvaluationStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProtocolRecommendation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProtocolRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProtocolReviewSheet = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ConsentRecommendation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsentRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InformedConsentForm = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRE_EthicsEvaluation", x => x.EvaluationId);
                    table.ForeignKey(
                        name: "FK_CRE_EthicsEvaluation_CRE_EthicsApplication_UrecNo",
                        column: x => x.UrecNo,
                        principalTable: "CRE_EthicsApplication",
                        principalColumn: "UrecNo");
                    table.ForeignKey(
                        name: "FK_CRE_EthicsEvaluation_CRE_EthicsEvaluator_EthicsEvaluatorId",
                        column: x => x.EthicsEvaluatorId,
                        principalTable: "CRE_EthicsEvaluator",
                        principalColumn: "EthicsEvaluatorId");
                });

            migrationBuilder.CreateTable(
                name: "CRE_EthicsEvaluatorExpertise",
                columns: table => new
                {
                    EthicsEvaluatorId = table.Column<int>(type: "int", nullable: false),
                    ExpertiseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRE_EthicsEvaluatorExpertise", x => new { x.EthicsEvaluatorId, x.ExpertiseId });
                    table.ForeignKey(
                        name: "FK_CRE_EthicsEvaluatorExpertise_CRE_EthicsEvaluator_EthicsEvaluatorId",
                        column: x => x.EthicsEvaluatorId,
                        principalTable: "CRE_EthicsEvaluator",
                        principalColumn: "EthicsEvaluatorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CRE_EthicsEvaluatorExpertise_CRE_Expertise_ExpertiseId",
                        column: x => x.ExpertiseId,
                        principalTable: "CRE_Expertise",
                        principalColumn: "ExpertiseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EthicsApplicationFormsEthicsForms",
                columns: table => new
                {
                    EthicsApplicationFormsEthicsApplicationFormId = table.Column<int>(type: "int", nullable: false),
                    EthicsFormsEthicsFormId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EthicsApplicationFormsEthicsForms", x => new { x.EthicsApplicationFormsEthicsApplicationFormId, x.EthicsFormsEthicsFormId });
                    table.ForeignKey(
                        name: "FK_EthicsApplicationFormsEthicsForms_CRE_EthicsApplicationForms_EthicsApplicationFormsEthicsApplicationFormId",
                        column: x => x.EthicsApplicationFormsEthicsApplicationFormId,
                        principalTable: "CRE_EthicsApplicationForms",
                        principalColumn: "EthicsApplicationFormId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EthicsApplicationFormsEthicsForms_CRE_EthicsForms_EthicsFormsEthicsFormId",
                        column: x => x.EthicsFormsEthicsFormId,
                        principalTable: "CRE_EthicsForms",
                        principalColumn: "EthicsFormId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CRE_NonFundedResearchInfo",
                columns: table => new
                {
                    NonFundedResearchId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UrecNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EthicsClearanceId = table.Column<int>(type: "int", nullable: true),
                    CompletionCertId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateSubmitted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    College = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    University = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRE_NonFundedResearchInfo", x => x.NonFundedResearchId);
                    table.ForeignKey(
                        name: "FK_CRE_NonFundedResearchInfo_CRE_CompletionCertificates_CompletionCertId",
                        column: x => x.CompletionCertId,
                        principalTable: "CRE_CompletionCertificates",
                        principalColumn: "CompletionCertId");
                    table.ForeignKey(
                        name: "FK_CRE_NonFundedResearchInfo_CRE_EthicsApplication_UrecNo",
                        column: x => x.UrecNo,
                        principalTable: "CRE_EthicsApplication",
                        principalColumn: "UrecNo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CRE_NonFundedResearchInfo_CRE_EthicsClearance_EthicsClearanceId",
                        column: x => x.EthicsClearanceId,
                        principalTable: "CRE_EthicsClearance",
                        principalColumn: "EthicsClearanceId");
                });

            migrationBuilder.CreateTable(
                name: "CRE_CoProponents",
                columns: table => new
                {
                    CoProponentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NonFundedResearchId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CoProponentName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRE_CoProponents", x => x.CoProponentId);
                    table.ForeignKey(
                        name: "FK_CRE_CoProponents_CRE_NonFundedResearchInfo_NonFundedResearchId",
                        column: x => x.NonFundedResearchId,
                        principalTable: "CRE_NonFundedResearchInfo",
                        principalColumn: "NonFundedResearchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CRE_Expertise",
                columns: new[] { "ExpertiseId", "ExpertiseName" },
                values: new object[,]
                {
                    { 1, "Education" },
                    { 2, "Computer Science, Information Systems, and Technology" },
                    { 3, "Engineering, Architecture, and Design" },
                    { 4, "Humanities, Language, and Communication" },
                    { 5, "Business" },
                    { 6, "Social Sciences" },
                    { 7, "Science, Mathematics, and Statistics" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CRE_CompletionCertificates_UrecNo",
                table: "CRE_CompletionCertificates",
                column: "UrecNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CRE_CompletionReports_UrecNo",
                table: "CRE_CompletionReports",
                column: "UrecNo",
                unique: true,
                filter: "[UrecNo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CRE_CoProponents_NonFundedResearchId",
                table: "CRE_CoProponents",
                column: "NonFundedResearchId");

            migrationBuilder.CreateIndex(
                name: "IX_CRE_DeclinedEthicsEvaluation_EthicsEvaluatorId",
                table: "CRE_DeclinedEthicsEvaluation",
                column: "EthicsEvaluatorId");

            migrationBuilder.CreateIndex(
                name: "IX_CRE_DeclinedEthicsEvaluation_UrecNo",
                table: "CRE_DeclinedEthicsEvaluation",
                column: "UrecNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CRE_EthicsApplicationForms_UrecNo",
                table: "CRE_EthicsApplicationForms",
                column: "UrecNo");

            migrationBuilder.CreateIndex(
                name: "IX_CRE_EthicsApplicationLogs_UrecNo",
                table: "CRE_EthicsApplicationLogs",
                column: "UrecNo");

            migrationBuilder.CreateIndex(
                name: "IX_CRE_EthicsClearance_UrecNo",
                table: "CRE_EthicsClearance",
                column: "UrecNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CRE_EthicsEvaluation_EthicsEvaluatorId",
                table: "CRE_EthicsEvaluation",
                column: "EthicsEvaluatorId");

            migrationBuilder.CreateIndex(
                name: "IX_CRE_EthicsEvaluation_UrecNo",
                table: "CRE_EthicsEvaluation",
                column: "UrecNo");

            migrationBuilder.CreateIndex(
                name: "IX_CRE_EthicsEvaluatorExpertise_ExpertiseId",
                table: "CRE_EthicsEvaluatorExpertise",
                column: "ExpertiseId");

            migrationBuilder.CreateIndex(
                name: "IX_CRE_EthicsNotifications_UrecNo",
                table: "CRE_EthicsNotifications",
                column: "UrecNo");

            migrationBuilder.CreateIndex(
                name: "IX_CRE_InitialReview_UrecNo",
                table: "CRE_InitialReview",
                column: "UrecNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CRE_NonFundedResearchInfo_CompletionCertId",
                table: "CRE_NonFundedResearchInfo",
                column: "CompletionCertId",
                unique: true,
                filter: "[CompletionCertId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CRE_NonFundedResearchInfo_EthicsClearanceId",
                table: "CRE_NonFundedResearchInfo",
                column: "EthicsClearanceId");

            migrationBuilder.CreateIndex(
                name: "IX_CRE_NonFundedResearchInfo_UrecNo",
                table: "CRE_NonFundedResearchInfo",
                column: "UrecNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CRE_ReceiptInfo_UrecNo",
                table: "CRE_ReceiptInfo",
                column: "UrecNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EthicsApplicationFormsEthicsForms_EthicsFormsEthicsFormId",
                table: "EthicsApplicationFormsEthicsForms",
                column: "EthicsFormsEthicsFormId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CRE_Chairperson");

            migrationBuilder.DropTable(
                name: "CRE_CompletionReports");

            migrationBuilder.DropTable(
                name: "CRE_CoProponents");

            migrationBuilder.DropTable(
                name: "CRE_DeclinedEthicsEvaluation");

            migrationBuilder.DropTable(
                name: "CRE_EthicsApplicationLogs");

            migrationBuilder.DropTable(
                name: "CRE_EthicsEvaluation");

            migrationBuilder.DropTable(
                name: "CRE_EthicsEvaluatorExpertise");

            migrationBuilder.DropTable(
                name: "CRE_EthicsMemoranda");

            migrationBuilder.DropTable(
                name: "CRE_EthicsNotifications");

            migrationBuilder.DropTable(
                name: "CRE_EthicsReport");

            migrationBuilder.DropTable(
                name: "CRE_InitialReview");

            migrationBuilder.DropTable(
                name: "CRE_ReceiptInfo");

            migrationBuilder.DropTable(
                name: "EthicsApplicationFormsEthicsForms");

            migrationBuilder.DropTable(
                name: "CRE_NonFundedResearchInfo");

            migrationBuilder.DropTable(
                name: "CRE_EthicsEvaluator");

            migrationBuilder.DropTable(
                name: "CRE_Expertise");

            migrationBuilder.DropTable(
                name: "CRE_EthicsApplicationForms");

            migrationBuilder.DropTable(
                name: "CRE_EthicsForms");

            migrationBuilder.DropTable(
                name: "CRE_CompletionCertificates");

            migrationBuilder.DropTable(
                name: "CRE_EthicsClearance");

            migrationBuilder.DropTable(
                name: "CRE_EthicsApplication");
        }
    }
}
