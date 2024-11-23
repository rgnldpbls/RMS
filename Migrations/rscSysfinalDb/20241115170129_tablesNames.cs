using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResearchManagementSystem.Migrations.rscSysfinalDb
{
    /// <inheritdoc />
    public partial class tablesNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationSubCategories_ApplicationTypes_ApplicationTypeId",
                table: "ApplicationSubCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Checklists_ApplicationSubCategories_ApplicationSubCategoryId",
                table: "Checklists");

            migrationBuilder.DropForeignKey(
                name: "FK_Checklists_ApplicationTypes_ApplicationTypeId",
                table: "Checklists");

            migrationBuilder.DropForeignKey(
                name: "FK_Criteria_EvaluationForms_FormId",
                table: "Criteria");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentHistories_Requests_RequestId",
                table: "DocumentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_EvaluationDocuments_EvaluatorAssignments_EvaluatorAssignmentId",
                table: "EvaluationDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_EvaluationFormResponses_Criteria_CriterionId",
                table: "EvaluationFormResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_EvaluationFormResponses_EvaluatorAssignments_EvaluatorAssignmentId",
                table: "EvaluationFormResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_EvaluationFormResponses_Evaluators_EvaluatorId",
                table: "EvaluationFormResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_EvaluationGeneralComments_EvaluatorAssignments_EvaluatorAssignmentId",
                table: "EvaluationGeneralComments");

            migrationBuilder.DropForeignKey(
                name: "FK_EvaluatorAssignments_Evaluators_EvaluatorId",
                table: "EvaluatorAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_EvaluatorAssignments_Requests_RequestId",
                table: "EvaluatorAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_FinalDocuments_Requests_RequestId",
                table: "FinalDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_EvaluatorAssignments_EvaluatorAssignmentId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Requirements_Checklists_ChecklistId",
                table: "Requirements");

            migrationBuilder.DropForeignKey(
                name: "FK_Requirements_Drafts_DraftId",
                table: "Requirements");

            migrationBuilder.DropForeignKey(
                name: "FK_Requirements_Requests_RequestId",
                table: "Requirements");

            migrationBuilder.DropForeignKey(
                name: "FK_StatusHistories_Requests_RequestId",
                table: "StatusHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Templates",
                table: "Templates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StatusHistories",
                table: "StatusHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Requirements",
                table: "Requirements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Requests",
                table: "Requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Memorandums",
                table: "Memorandums");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GeneratedReports",
                table: "GeneratedReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FinalDocuments",
                table: "FinalDocuments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Evaluators",
                table: "Evaluators");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EvaluatorAssignments",
                table: "EvaluatorAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EvaluationGeneralComments",
                table: "EvaluationGeneralComments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EvaluationForms",
                table: "EvaluationForms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EvaluationFormResponses",
                table: "EvaluationFormResponses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EvaluationDocuments",
                table: "EvaluationDocuments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Drafts",
                table: "Drafts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DocumentHistories",
                table: "DocumentHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Criteria",
                table: "Criteria");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Checklists",
                table: "Checklists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationTypes",
                table: "ApplicationTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationSubCategories",
                table: "ApplicationSubCategories");

            migrationBuilder.RenameTable(
                name: "Templates",
                newName: "RSC_Templates");

            migrationBuilder.RenameTable(
                name: "StatusHistories",
                newName: "RSC_StatusHistories");

            migrationBuilder.RenameTable(
                name: "Requirements",
                newName: "RSC_Requirements");

            migrationBuilder.RenameTable(
                name: "Requests",
                newName: "RSC_Requests");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "RSC_Notifications");

            migrationBuilder.RenameTable(
                name: "Memorandums",
                newName: "RSC_Memorandums");

            migrationBuilder.RenameTable(
                name: "GeneratedReports",
                newName: "RSC_GeneratedReports");

            migrationBuilder.RenameTable(
                name: "FinalDocuments",
                newName: "RSC_FinalDocuments");

            migrationBuilder.RenameTable(
                name: "Evaluators",
                newName: "RSC_Evaluators");

            migrationBuilder.RenameTable(
                name: "EvaluatorAssignments",
                newName: "RSC_EvaluatorAssignments");

            migrationBuilder.RenameTable(
                name: "EvaluationGeneralComments",
                newName: "RSC_EvaluationGeneralComments");

            migrationBuilder.RenameTable(
                name: "EvaluationForms",
                newName: "RSC_EvaluationForms");

            migrationBuilder.RenameTable(
                name: "EvaluationFormResponses",
                newName: "RSC_EvaluationFormResponses");

            migrationBuilder.RenameTable(
                name: "EvaluationDocuments",
                newName: "RSC_EvaluationDocuments");

            migrationBuilder.RenameTable(
                name: "Drafts",
                newName: "RSC_Drafts");

            migrationBuilder.RenameTable(
                name: "DocumentHistories",
                newName: "RSC_DocumentHistories");

            migrationBuilder.RenameTable(
                name: "Criteria",
                newName: "RSC_Criteria");

            migrationBuilder.RenameTable(
                name: "Checklists",
                newName: "RSC_Checklists");

            migrationBuilder.RenameTable(
                name: "ApplicationTypes",
                newName: "RSC_ApplicationTypes");

            migrationBuilder.RenameTable(
                name: "ApplicationSubCategories",
                newName: "RSC_ApplicationSubCategories");

            migrationBuilder.RenameIndex(
                name: "IX_StatusHistories_RequestId",
                table: "RSC_StatusHistories",
                newName: "IX_RSC_StatusHistories_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Requirements_RequestId",
                table: "RSC_Requirements",
                newName: "IX_RSC_Requirements_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Requirements_DraftId",
                table: "RSC_Requirements",
                newName: "IX_RSC_Requirements_DraftId");

            migrationBuilder.RenameIndex(
                name: "IX_Requirements_ChecklistId",
                table: "RSC_Requirements",
                newName: "IX_RSC_Requirements_ChecklistId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_EvaluatorAssignmentId",
                table: "RSC_Notifications",
                newName: "IX_RSC_Notifications_EvaluatorAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_FinalDocuments_RequestId",
                table: "RSC_FinalDocuments",
                newName: "IX_RSC_FinalDocuments_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_EvaluatorAssignments_RequestId",
                table: "RSC_EvaluatorAssignments",
                newName: "IX_RSC_EvaluatorAssignments_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_EvaluatorAssignments_EvaluatorId",
                table: "RSC_EvaluatorAssignments",
                newName: "IX_RSC_EvaluatorAssignments_EvaluatorId");

            migrationBuilder.RenameIndex(
                name: "IX_EvaluationGeneralComments_EvaluatorAssignmentId",
                table: "RSC_EvaluationGeneralComments",
                newName: "IX_RSC_EvaluationGeneralComments_EvaluatorAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_EvaluationFormResponses_EvaluatorId",
                table: "RSC_EvaluationFormResponses",
                newName: "IX_RSC_EvaluationFormResponses_EvaluatorId");

            migrationBuilder.RenameIndex(
                name: "IX_EvaluationFormResponses_EvaluatorAssignmentId",
                table: "RSC_EvaluationFormResponses",
                newName: "IX_RSC_EvaluationFormResponses_EvaluatorAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_EvaluationFormResponses_CriterionId",
                table: "RSC_EvaluationFormResponses",
                newName: "IX_RSC_EvaluationFormResponses_CriterionId");

            migrationBuilder.RenameIndex(
                name: "IX_EvaluationDocuments_EvaluatorAssignmentId",
                table: "RSC_EvaluationDocuments",
                newName: "IX_RSC_EvaluationDocuments_EvaluatorAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_DocumentHistories_RequestId",
                table: "RSC_DocumentHistories",
                newName: "IX_RSC_DocumentHistories_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Criteria_FormId",
                table: "RSC_Criteria",
                newName: "IX_RSC_Criteria_FormId");

            migrationBuilder.RenameIndex(
                name: "IX_Checklists_ApplicationTypeId",
                table: "RSC_Checklists",
                newName: "IX_RSC_Checklists_ApplicationTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Checklists_ApplicationSubCategoryId",
                table: "RSC_Checklists",
                newName: "IX_RSC_Checklists_ApplicationSubCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationSubCategories_ApplicationTypeId",
                table: "RSC_ApplicationSubCategories",
                newName: "IX_RSC_ApplicationSubCategories_ApplicationTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RSC_Templates",
                table: "RSC_Templates",
                column: "TemplateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RSC_StatusHistories",
                table: "RSC_StatusHistories",
                column: "StatusHistoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RSC_Requirements",
                table: "RSC_Requirements",
                column: "RequirementId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RSC_Requests",
                table: "RSC_Requests",
                column: "RequestId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RSC_Notifications",
                table: "RSC_Notifications",
                column: "NotificationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RSC_Memorandums",
                table: "RSC_Memorandums",
                column: "memorandumId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RSC_GeneratedReports",
                table: "RSC_GeneratedReports",
                column: "ReportId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RSC_FinalDocuments",
                table: "RSC_FinalDocuments",
                column: "FinalDocuID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RSC_Evaluators",
                table: "RSC_Evaluators",
                column: "EvaluatorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RSC_EvaluatorAssignments",
                table: "RSC_EvaluatorAssignments",
                column: "AssignmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RSC_EvaluationGeneralComments",
                table: "RSC_EvaluationGeneralComments",
                column: "CommentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RSC_EvaluationForms",
                table: "RSC_EvaluationForms",
                column: "FormId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RSC_EvaluationFormResponses",
                table: "RSC_EvaluationFormResponses",
                column: "ResponseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RSC_EvaluationDocuments",
                table: "RSC_EvaluationDocuments",
                column: "EvaluationDocuId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RSC_Drafts",
                table: "RSC_Drafts",
                column: "DraftId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RSC_DocumentHistories",
                table: "RSC_DocumentHistories",
                column: "DocumentHistoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RSC_Criteria",
                table: "RSC_Criteria",
                column: "CriterionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RSC_Checklists",
                table: "RSC_Checklists",
                column: "ChecklistId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RSC_ApplicationTypes",
                table: "RSC_ApplicationTypes",
                column: "ApplicationTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RSC_ApplicationSubCategories",
                table: "RSC_ApplicationSubCategories",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_RSC_ApplicationSubCategories_RSC_ApplicationTypes_ApplicationTypeId",
                table: "RSC_ApplicationSubCategories",
                column: "ApplicationTypeId",
                principalTable: "RSC_ApplicationTypes",
                principalColumn: "ApplicationTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RSC_Checklists_RSC_ApplicationSubCategories_ApplicationSubCategoryId",
                table: "RSC_Checklists",
                column: "ApplicationSubCategoryId",
                principalTable: "RSC_ApplicationSubCategories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RSC_Checklists_RSC_ApplicationTypes_ApplicationTypeId",
                table: "RSC_Checklists",
                column: "ApplicationTypeId",
                principalTable: "RSC_ApplicationTypes",
                principalColumn: "ApplicationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RSC_Criteria_RSC_EvaluationForms_FormId",
                table: "RSC_Criteria",
                column: "FormId",
                principalTable: "RSC_EvaluationForms",
                principalColumn: "FormId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RSC_DocumentHistories_RSC_Requests_RequestId",
                table: "RSC_DocumentHistories",
                column: "RequestId",
                principalTable: "RSC_Requests",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RSC_EvaluationDocuments_RSC_EvaluatorAssignments_EvaluatorAssignmentId",
                table: "RSC_EvaluationDocuments",
                column: "EvaluatorAssignmentId",
                principalTable: "RSC_EvaluatorAssignments",
                principalColumn: "AssignmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RSC_EvaluationFormResponses_RSC_Criteria_CriterionId",
                table: "RSC_EvaluationFormResponses",
                column: "CriterionId",
                principalTable: "RSC_Criteria",
                principalColumn: "CriterionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RSC_EvaluationFormResponses_RSC_EvaluatorAssignments_EvaluatorAssignmentId",
                table: "RSC_EvaluationFormResponses",
                column: "EvaluatorAssignmentId",
                principalTable: "RSC_EvaluatorAssignments",
                principalColumn: "AssignmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RSC_EvaluationFormResponses_RSC_Evaluators_EvaluatorId",
                table: "RSC_EvaluationFormResponses",
                column: "EvaluatorId",
                principalTable: "RSC_Evaluators",
                principalColumn: "EvaluatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_RSC_EvaluationGeneralComments_RSC_EvaluatorAssignments_EvaluatorAssignmentId",
                table: "RSC_EvaluationGeneralComments",
                column: "EvaluatorAssignmentId",
                principalTable: "RSC_EvaluatorAssignments",
                principalColumn: "AssignmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RSC_EvaluatorAssignments_RSC_Evaluators_EvaluatorId",
                table: "RSC_EvaluatorAssignments",
                column: "EvaluatorId",
                principalTable: "RSC_Evaluators",
                principalColumn: "EvaluatorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RSC_EvaluatorAssignments_RSC_Requests_RequestId",
                table: "RSC_EvaluatorAssignments",
                column: "RequestId",
                principalTable: "RSC_Requests",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RSC_FinalDocuments_RSC_Requests_RequestId",
                table: "RSC_FinalDocuments",
                column: "RequestId",
                principalTable: "RSC_Requests",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RSC_Notifications_RSC_EvaluatorAssignments_EvaluatorAssignmentId",
                table: "RSC_Notifications",
                column: "EvaluatorAssignmentId",
                principalTable: "RSC_EvaluatorAssignments",
                principalColumn: "AssignmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RSC_Requirements_RSC_Checklists_ChecklistId",
                table: "RSC_Requirements",
                column: "ChecklistId",
                principalTable: "RSC_Checklists",
                principalColumn: "ChecklistId");

            migrationBuilder.AddForeignKey(
                name: "FK_RSC_Requirements_RSC_Drafts_DraftId",
                table: "RSC_Requirements",
                column: "DraftId",
                principalTable: "RSC_Drafts",
                principalColumn: "DraftId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RSC_Requirements_RSC_Requests_RequestId",
                table: "RSC_Requirements",
                column: "RequestId",
                principalTable: "RSC_Requests",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RSC_StatusHistories_RSC_Requests_RequestId",
                table: "RSC_StatusHistories",
                column: "RequestId",
                principalTable: "RSC_Requests",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RSC_ApplicationSubCategories_RSC_ApplicationTypes_ApplicationTypeId",
                table: "RSC_ApplicationSubCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_RSC_Checklists_RSC_ApplicationSubCategories_ApplicationSubCategoryId",
                table: "RSC_Checklists");

            migrationBuilder.DropForeignKey(
                name: "FK_RSC_Checklists_RSC_ApplicationTypes_ApplicationTypeId",
                table: "RSC_Checklists");

            migrationBuilder.DropForeignKey(
                name: "FK_RSC_Criteria_RSC_EvaluationForms_FormId",
                table: "RSC_Criteria");

            migrationBuilder.DropForeignKey(
                name: "FK_RSC_DocumentHistories_RSC_Requests_RequestId",
                table: "RSC_DocumentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_RSC_EvaluationDocuments_RSC_EvaluatorAssignments_EvaluatorAssignmentId",
                table: "RSC_EvaluationDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_RSC_EvaluationFormResponses_RSC_Criteria_CriterionId",
                table: "RSC_EvaluationFormResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_RSC_EvaluationFormResponses_RSC_EvaluatorAssignments_EvaluatorAssignmentId",
                table: "RSC_EvaluationFormResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_RSC_EvaluationFormResponses_RSC_Evaluators_EvaluatorId",
                table: "RSC_EvaluationFormResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_RSC_EvaluationGeneralComments_RSC_EvaluatorAssignments_EvaluatorAssignmentId",
                table: "RSC_EvaluationGeneralComments");

            migrationBuilder.DropForeignKey(
                name: "FK_RSC_EvaluatorAssignments_RSC_Evaluators_EvaluatorId",
                table: "RSC_EvaluatorAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_RSC_EvaluatorAssignments_RSC_Requests_RequestId",
                table: "RSC_EvaluatorAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_RSC_FinalDocuments_RSC_Requests_RequestId",
                table: "RSC_FinalDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_RSC_Notifications_RSC_EvaluatorAssignments_EvaluatorAssignmentId",
                table: "RSC_Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_RSC_Requirements_RSC_Checklists_ChecklistId",
                table: "RSC_Requirements");

            migrationBuilder.DropForeignKey(
                name: "FK_RSC_Requirements_RSC_Drafts_DraftId",
                table: "RSC_Requirements");

            migrationBuilder.DropForeignKey(
                name: "FK_RSC_Requirements_RSC_Requests_RequestId",
                table: "RSC_Requirements");

            migrationBuilder.DropForeignKey(
                name: "FK_RSC_StatusHistories_RSC_Requests_RequestId",
                table: "RSC_StatusHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RSC_Templates",
                table: "RSC_Templates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RSC_StatusHistories",
                table: "RSC_StatusHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RSC_Requirements",
                table: "RSC_Requirements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RSC_Requests",
                table: "RSC_Requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RSC_Notifications",
                table: "RSC_Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RSC_Memorandums",
                table: "RSC_Memorandums");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RSC_GeneratedReports",
                table: "RSC_GeneratedReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RSC_FinalDocuments",
                table: "RSC_FinalDocuments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RSC_Evaluators",
                table: "RSC_Evaluators");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RSC_EvaluatorAssignments",
                table: "RSC_EvaluatorAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RSC_EvaluationGeneralComments",
                table: "RSC_EvaluationGeneralComments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RSC_EvaluationForms",
                table: "RSC_EvaluationForms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RSC_EvaluationFormResponses",
                table: "RSC_EvaluationFormResponses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RSC_EvaluationDocuments",
                table: "RSC_EvaluationDocuments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RSC_Drafts",
                table: "RSC_Drafts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RSC_DocumentHistories",
                table: "RSC_DocumentHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RSC_Criteria",
                table: "RSC_Criteria");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RSC_Checklists",
                table: "RSC_Checklists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RSC_ApplicationTypes",
                table: "RSC_ApplicationTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RSC_ApplicationSubCategories",
                table: "RSC_ApplicationSubCategories");

            migrationBuilder.RenameTable(
                name: "RSC_Templates",
                newName: "Templates");

            migrationBuilder.RenameTable(
                name: "RSC_StatusHistories",
                newName: "StatusHistories");

            migrationBuilder.RenameTable(
                name: "RSC_Requirements",
                newName: "Requirements");

            migrationBuilder.RenameTable(
                name: "RSC_Requests",
                newName: "Requests");

            migrationBuilder.RenameTable(
                name: "RSC_Notifications",
                newName: "Notifications");

            migrationBuilder.RenameTable(
                name: "RSC_Memorandums",
                newName: "Memorandums");

            migrationBuilder.RenameTable(
                name: "RSC_GeneratedReports",
                newName: "GeneratedReports");

            migrationBuilder.RenameTable(
                name: "RSC_FinalDocuments",
                newName: "FinalDocuments");

            migrationBuilder.RenameTable(
                name: "RSC_Evaluators",
                newName: "Evaluators");

            migrationBuilder.RenameTable(
                name: "RSC_EvaluatorAssignments",
                newName: "EvaluatorAssignments");

            migrationBuilder.RenameTable(
                name: "RSC_EvaluationGeneralComments",
                newName: "EvaluationGeneralComments");

            migrationBuilder.RenameTable(
                name: "RSC_EvaluationForms",
                newName: "EvaluationForms");

            migrationBuilder.RenameTable(
                name: "RSC_EvaluationFormResponses",
                newName: "EvaluationFormResponses");

            migrationBuilder.RenameTable(
                name: "RSC_EvaluationDocuments",
                newName: "EvaluationDocuments");

            migrationBuilder.RenameTable(
                name: "RSC_Drafts",
                newName: "Drafts");

            migrationBuilder.RenameTable(
                name: "RSC_DocumentHistories",
                newName: "DocumentHistories");

            migrationBuilder.RenameTable(
                name: "RSC_Criteria",
                newName: "Criteria");

            migrationBuilder.RenameTable(
                name: "RSC_Checklists",
                newName: "Checklists");

            migrationBuilder.RenameTable(
                name: "RSC_ApplicationTypes",
                newName: "ApplicationTypes");

            migrationBuilder.RenameTable(
                name: "RSC_ApplicationSubCategories",
                newName: "ApplicationSubCategories");

            migrationBuilder.RenameIndex(
                name: "IX_RSC_StatusHistories_RequestId",
                table: "StatusHistories",
                newName: "IX_StatusHistories_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_RSC_Requirements_RequestId",
                table: "Requirements",
                newName: "IX_Requirements_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_RSC_Requirements_DraftId",
                table: "Requirements",
                newName: "IX_Requirements_DraftId");

            migrationBuilder.RenameIndex(
                name: "IX_RSC_Requirements_ChecklistId",
                table: "Requirements",
                newName: "IX_Requirements_ChecklistId");

            migrationBuilder.RenameIndex(
                name: "IX_RSC_Notifications_EvaluatorAssignmentId",
                table: "Notifications",
                newName: "IX_Notifications_EvaluatorAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_RSC_FinalDocuments_RequestId",
                table: "FinalDocuments",
                newName: "IX_FinalDocuments_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_RSC_EvaluatorAssignments_RequestId",
                table: "EvaluatorAssignments",
                newName: "IX_EvaluatorAssignments_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_RSC_EvaluatorAssignments_EvaluatorId",
                table: "EvaluatorAssignments",
                newName: "IX_EvaluatorAssignments_EvaluatorId");

            migrationBuilder.RenameIndex(
                name: "IX_RSC_EvaluationGeneralComments_EvaluatorAssignmentId",
                table: "EvaluationGeneralComments",
                newName: "IX_EvaluationGeneralComments_EvaluatorAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_RSC_EvaluationFormResponses_EvaluatorId",
                table: "EvaluationFormResponses",
                newName: "IX_EvaluationFormResponses_EvaluatorId");

            migrationBuilder.RenameIndex(
                name: "IX_RSC_EvaluationFormResponses_EvaluatorAssignmentId",
                table: "EvaluationFormResponses",
                newName: "IX_EvaluationFormResponses_EvaluatorAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_RSC_EvaluationFormResponses_CriterionId",
                table: "EvaluationFormResponses",
                newName: "IX_EvaluationFormResponses_CriterionId");

            migrationBuilder.RenameIndex(
                name: "IX_RSC_EvaluationDocuments_EvaluatorAssignmentId",
                table: "EvaluationDocuments",
                newName: "IX_EvaluationDocuments_EvaluatorAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_RSC_DocumentHistories_RequestId",
                table: "DocumentHistories",
                newName: "IX_DocumentHistories_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_RSC_Criteria_FormId",
                table: "Criteria",
                newName: "IX_Criteria_FormId");

            migrationBuilder.RenameIndex(
                name: "IX_RSC_Checklists_ApplicationTypeId",
                table: "Checklists",
                newName: "IX_Checklists_ApplicationTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_RSC_Checklists_ApplicationSubCategoryId",
                table: "Checklists",
                newName: "IX_Checklists_ApplicationSubCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_RSC_ApplicationSubCategories_ApplicationTypeId",
                table: "ApplicationSubCategories",
                newName: "IX_ApplicationSubCategories_ApplicationTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Templates",
                table: "Templates",
                column: "TemplateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StatusHistories",
                table: "StatusHistories",
                column: "StatusHistoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Requirements",
                table: "Requirements",
                column: "RequirementId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Requests",
                table: "Requests",
                column: "RequestId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "NotificationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Memorandums",
                table: "Memorandums",
                column: "memorandumId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GeneratedReports",
                table: "GeneratedReports",
                column: "ReportId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FinalDocuments",
                table: "FinalDocuments",
                column: "FinalDocuID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Evaluators",
                table: "Evaluators",
                column: "EvaluatorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EvaluatorAssignments",
                table: "EvaluatorAssignments",
                column: "AssignmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EvaluationGeneralComments",
                table: "EvaluationGeneralComments",
                column: "CommentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EvaluationForms",
                table: "EvaluationForms",
                column: "FormId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EvaluationFormResponses",
                table: "EvaluationFormResponses",
                column: "ResponseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EvaluationDocuments",
                table: "EvaluationDocuments",
                column: "EvaluationDocuId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Drafts",
                table: "Drafts",
                column: "DraftId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DocumentHistories",
                table: "DocumentHistories",
                column: "DocumentHistoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Criteria",
                table: "Criteria",
                column: "CriterionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Checklists",
                table: "Checklists",
                column: "ChecklistId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationTypes",
                table: "ApplicationTypes",
                column: "ApplicationTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationSubCategories",
                table: "ApplicationSubCategories",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationSubCategories_ApplicationTypes_ApplicationTypeId",
                table: "ApplicationSubCategories",
                column: "ApplicationTypeId",
                principalTable: "ApplicationTypes",
                principalColumn: "ApplicationTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Checklists_ApplicationSubCategories_ApplicationSubCategoryId",
                table: "Checklists",
                column: "ApplicationSubCategoryId",
                principalTable: "ApplicationSubCategories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Checklists_ApplicationTypes_ApplicationTypeId",
                table: "Checklists",
                column: "ApplicationTypeId",
                principalTable: "ApplicationTypes",
                principalColumn: "ApplicationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Criteria_EvaluationForms_FormId",
                table: "Criteria",
                column: "FormId",
                principalTable: "EvaluationForms",
                principalColumn: "FormId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentHistories_Requests_RequestId",
                table: "DocumentHistories",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluationDocuments_EvaluatorAssignments_EvaluatorAssignmentId",
                table: "EvaluationDocuments",
                column: "EvaluatorAssignmentId",
                principalTable: "EvaluatorAssignments",
                principalColumn: "AssignmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluationFormResponses_Criteria_CriterionId",
                table: "EvaluationFormResponses",
                column: "CriterionId",
                principalTable: "Criteria",
                principalColumn: "CriterionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluationFormResponses_EvaluatorAssignments_EvaluatorAssignmentId",
                table: "EvaluationFormResponses",
                column: "EvaluatorAssignmentId",
                principalTable: "EvaluatorAssignments",
                principalColumn: "AssignmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluationFormResponses_Evaluators_EvaluatorId",
                table: "EvaluationFormResponses",
                column: "EvaluatorId",
                principalTable: "Evaluators",
                principalColumn: "EvaluatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluationGeneralComments_EvaluatorAssignments_EvaluatorAssignmentId",
                table: "EvaluationGeneralComments",
                column: "EvaluatorAssignmentId",
                principalTable: "EvaluatorAssignments",
                principalColumn: "AssignmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluatorAssignments_Evaluators_EvaluatorId",
                table: "EvaluatorAssignments",
                column: "EvaluatorId",
                principalTable: "Evaluators",
                principalColumn: "EvaluatorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluatorAssignments_Requests_RequestId",
                table: "EvaluatorAssignments",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FinalDocuments_Requests_RequestId",
                table: "FinalDocuments",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_EvaluatorAssignments_EvaluatorAssignmentId",
                table: "Notifications",
                column: "EvaluatorAssignmentId",
                principalTable: "EvaluatorAssignments",
                principalColumn: "AssignmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requirements_Checklists_ChecklistId",
                table: "Requirements",
                column: "ChecklistId",
                principalTable: "Checklists",
                principalColumn: "ChecklistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requirements_Drafts_DraftId",
                table: "Requirements",
                column: "DraftId",
                principalTable: "Drafts",
                principalColumn: "DraftId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requirements_Requests_RequestId",
                table: "Requirements",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StatusHistories_Requests_RequestId",
                table: "StatusHistories",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
