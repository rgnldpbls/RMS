using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResearchManagementSystem.Migrations.rscSysfinalDb
{
    /// <inheritdoc />
    public partial class Removerelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RSC_Checklists_RSC_ApplicationSubCategories_ApplicationSubCategoryId",
                table: "RSC_Checklists");

            migrationBuilder.AddForeignKey(
                name: "FK_RSC_Checklists_RSC_ApplicationSubCategories_ApplicationSubCategoryId",
                table: "RSC_Checklists",
                column: "ApplicationSubCategoryId",
                principalTable: "RSC_ApplicationSubCategories",
                principalColumn: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RSC_Checklists_RSC_ApplicationSubCategories_ApplicationSubCategoryId",
                table: "RSC_Checklists");

            migrationBuilder.AddForeignKey(
                name: "FK_RSC_Checklists_RSC_ApplicationSubCategories_ApplicationSubCategoryId",
                table: "RSC_Checklists",
                column: "ApplicationSubCategoryId",
                principalTable: "RSC_ApplicationSubCategories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
