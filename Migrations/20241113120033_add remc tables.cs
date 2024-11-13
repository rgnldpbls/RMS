﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResearchManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class addremctables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "REMC_CalendarEvents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Visibility = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REMC_CalendarEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "REMC_Criterias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REMC_Criterias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "REMC_Evaluator",
                columns: table => new
                {
                    evaluator_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    evaluator_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    evaluator_Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    field_of_Interest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REMC_Evaluator", x => x.evaluator_Id);
                });

            migrationBuilder.CreateTable(
                name: "REMC_FundedResearchApplication",
                columns: table => new
                {
                    fra_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    fra_Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    research_Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    applicant_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    applicant_Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    team_Members = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    college = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    branch = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    field_of_Study = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    application_Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    submission_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    dts_No = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    project_Duration = table.Column<int>(type: "int", nullable: false),
                    total_project_Cost = table.Column<double>(type: "float", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isArchive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REMC_FundedResearchApplication", x => x.fra_Id);
                });

            migrationBuilder.CreateTable(
                name: "REMC_GenerateGAWADNominees",
                columns: table => new
                {
                    gn_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    gn_fileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gn_fileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gn_Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    gn_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    generateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isArchived = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REMC_GenerateGAWADNominees", x => x.gn_Id);
                });

            migrationBuilder.CreateTable(
                name: "REMC_GenerateReports",
                columns: table => new
                {
                    gr_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    gr_fileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gr_fileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gr_Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    gr_startDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    gr_endDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    gr_typeofReport = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    generateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isArchived = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REMC_GenerateReports", x => x.gr_Id);
                });

            migrationBuilder.CreateTable(
                name: "REMC_Guidelines",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    file_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    file_Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    data = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    document_Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    file_Uploaded = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REMC_Guidelines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "REMC_Settings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    isMaintenance = table.Column<bool>(type: "bit", nullable: false),
                    isUFRApplication = table.Column<bool>(type: "bit", nullable: false),
                    isEFRApplication = table.Column<bool>(type: "bit", nullable: false),
                    isUFRLApplication = table.Column<bool>(type: "bit", nullable: false),
                    evaluatorNum = table.Column<int>(type: "int", nullable: false),
                    daysEvaluation = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REMC_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "REMC_UFRForecastings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectCosts = table.Column<float>(type: "real", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REMC_UFRForecastings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "REMC_SubCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxScore = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CriteriaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REMC_SubCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_REMC_SubCategories_REMC_Criterias_CriteriaId",
                        column: x => x.CriteriaId,
                        principalTable: "REMC_Criterias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "REMC_ActionLogs",
                columns: table => new
                {
                    LogId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResearchType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isTeamLeader = table.Column<bool>(type: "bit", nullable: false),
                    isChief = table.Column<bool>(type: "bit", nullable: false),
                    isEvaluator = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FraId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REMC_ActionLogs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_REMC_ActionLogs_REMC_FundedResearchApplication_FraId",
                        column: x => x.FraId,
                        principalTable: "REMC_FundedResearchApplication",
                        principalColumn: "fra_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "REMC_Evaluations",
                columns: table => new
                {
                    evaluation_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    evaluation_Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    evaluator_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    evaluation_Grade = table.Column<double>(type: "float", nullable: true),
                    assigned_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    evaluation_Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    evaluation_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    evaluator_Id = table.Column<int>(type: "int", nullable: false),
                    fra_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    reminded_ThreeDaysBefore = table.Column<bool>(type: "bit", nullable: false),
                    reminded_OneDayBefore = table.Column<bool>(type: "bit", nullable: false),
                    reminded_Today = table.Column<bool>(type: "bit", nullable: false),
                    reminded_OneDayOverdue = table.Column<bool>(type: "bit", nullable: false),
                    reminded_ThreeDaysOverdue = table.Column<bool>(type: "bit", nullable: false),
                    reminded_SevenDaysOverdue = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REMC_Evaluations", x => x.evaluation_Id);
                    table.ForeignKey(
                        name: "FK_REMC_Evaluations_REMC_Evaluator_evaluator_Id",
                        column: x => x.evaluator_Id,
                        principalTable: "REMC_Evaluator",
                        principalColumn: "evaluator_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_REMC_Evaluations_REMC_FundedResearchApplication_fra_Id",
                        column: x => x.fra_Id,
                        principalTable: "REMC_FundedResearchApplication",
                        principalColumn: "fra_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "REMC_FileRequirement",
                columns: table => new
                {
                    fr_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    file_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    file_Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    data = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    file_Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    document_Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    file_Feedback = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    file_Uploaded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fra_Id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REMC_FileRequirement", x => x.fr_Id);
                    table.ForeignKey(
                        name: "FK_REMC_FileRequirement_REMC_FundedResearchApplication_fra_Id",
                        column: x => x.fra_Id,
                        principalTable: "REMC_FundedResearchApplication",
                        principalColumn: "fra_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "REMC_FundedResearches",
                columns: table => new
                {
                    fr_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    fr_Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    research_Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    team_Leader = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    teamLead_Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    team_Members = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    college = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    branch = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    field_of_Study = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    start_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    dts_No = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    project_Duration = table.Column<int>(type: "int", nullable: false),
                    total_project_Cost = table.Column<double>(type: "float", nullable: true),
                    isExtension1 = table.Column<bool>(type: "bit", nullable: false),
                    isExtension2 = table.Column<bool>(type: "bit", nullable: false),
                    fra_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isArchive = table.Column<bool>(type: "bit", nullable: false),
                    reminded_ThreeDaysBefore = table.Column<bool>(type: "bit", nullable: false),
                    reminded_OneDayBefore = table.Column<bool>(type: "bit", nullable: false),
                    reminded_Today = table.Column<bool>(type: "bit", nullable: false),
                    reminded_OneDayOverdue = table.Column<bool>(type: "bit", nullable: false),
                    reminded_ThreeDaysOverdue = table.Column<bool>(type: "bit", nullable: false),
                    reminded_SevenDaysOverdue = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REMC_FundedResearches", x => x.fr_Id);
                    table.ForeignKey(
                        name: "FK_REMC_FundedResearches_REMC_FundedResearchApplication_fra_Id",
                        column: x => x.fra_Id,
                        principalTable: "REMC_FundedResearchApplication",
                        principalColumn: "fra_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "REMC_FundedResearchEthics",
                columns: table => new
                {
                    fre_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    file_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    file_Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    clearanceFile = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    file_Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    file_Feedback = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    file_Uploaded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fra_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    urecNo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REMC_FundedResearchEthics", x => x.fre_Id);
                    table.ForeignKey(
                        name: "FK_REMC_FundedResearchEthics_REMC_FundedResearchApplication_fra_Id",
                        column: x => x.fra_Id,
                        principalTable: "REMC_FundedResearchApplication",
                        principalColumn: "fra_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "REMC_GeneratedForms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileContent = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fra_Id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REMC_GeneratedForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_REMC_GeneratedForms_REMC_FundedResearchApplication_fra_Id",
                        column: x => x.fra_Id,
                        principalTable: "REMC_FundedResearchApplication",
                        principalColumn: "fra_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "REMC_ProgressReports",
                columns: table => new
                {
                    pr_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    file_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    file_Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    data = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    file_Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    document_Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    file_Feedback = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    file_Uploaded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fr_Id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REMC_ProgressReports", x => x.pr_Id);
                    table.ForeignKey(
                        name: "FK_REMC_ProgressReports_REMC_FundedResearches_fr_Id",
                        column: x => x.fr_Id,
                        principalTable: "REMC_FundedResearches",
                        principalColumn: "fr_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_REMC_ActionLogs_FraId",
                table: "REMC_ActionLogs",
                column: "FraId");

            migrationBuilder.CreateIndex(
                name: "IX_REMC_Evaluations_evaluator_Id",
                table: "REMC_Evaluations",
                column: "evaluator_Id");

            migrationBuilder.CreateIndex(
                name: "IX_REMC_Evaluations_fra_Id",
                table: "REMC_Evaluations",
                column: "fra_Id");

            migrationBuilder.CreateIndex(
                name: "IX_REMC_FileRequirement_fra_Id",
                table: "REMC_FileRequirement",
                column: "fra_Id");

            migrationBuilder.CreateIndex(
                name: "IX_REMC_FundedResearches_fra_Id",
                table: "REMC_FundedResearches",
                column: "fra_Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_REMC_FundedResearchEthics_fra_Id",
                table: "REMC_FundedResearchEthics",
                column: "fra_Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_REMC_GeneratedForms_fra_Id",
                table: "REMC_GeneratedForms",
                column: "fra_Id");

            migrationBuilder.CreateIndex(
                name: "IX_REMC_ProgressReports_fr_Id",
                table: "REMC_ProgressReports",
                column: "fr_Id");

            migrationBuilder.CreateIndex(
                name: "IX_REMC_SubCategories_CriteriaId",
                table: "REMC_SubCategories",
                column: "CriteriaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "REMC_ActionLogs");

            migrationBuilder.DropTable(
                name: "REMC_CalendarEvents");

            migrationBuilder.DropTable(
                name: "REMC_Evaluations");

            migrationBuilder.DropTable(
                name: "REMC_FileRequirement");

            migrationBuilder.DropTable(
                name: "REMC_FundedResearchEthics");

            migrationBuilder.DropTable(
                name: "REMC_GeneratedForms");

            migrationBuilder.DropTable(
                name: "REMC_GenerateGAWADNominees");

            migrationBuilder.DropTable(
                name: "REMC_GenerateReports");

            migrationBuilder.DropTable(
                name: "REMC_Guidelines");

            migrationBuilder.DropTable(
                name: "REMC_ProgressReports");

            migrationBuilder.DropTable(
                name: "REMC_Settings");

            migrationBuilder.DropTable(
                name: "REMC_SubCategories");

            migrationBuilder.DropTable(
                name: "REMC_UFRForecastings");

            migrationBuilder.DropTable(
                name: "REMC_Evaluator");

            migrationBuilder.DropTable(
                name: "REMC_FundedResearches");

            migrationBuilder.DropTable(
                name: "REMC_Criterias");

            migrationBuilder.DropTable(
                name: "REMC_FundedResearchApplication");
        }
    }
}
