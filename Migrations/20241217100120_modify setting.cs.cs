using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResearchManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class modifysettingcs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "lathalaFieldOfStudy",
                table: "REMC_Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "lathalaNatureOfInvolvement",
                table: "REMC_Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tuklasFieldOfStudy",
                table: "REMC_Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tuklasNatureOfInvolvement",
                table: "REMC_Settings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lathalaFieldOfStudy",
                table: "REMC_Settings");

            migrationBuilder.DropColumn(
                name: "lathalaNatureOfInvolvement",
                table: "REMC_Settings");

            migrationBuilder.DropColumn(
                name: "tuklasFieldOfStudy",
                table: "REMC_Settings");

            migrationBuilder.DropColumn(
                name: "tuklasNatureOfInvolvement",
                table: "REMC_Settings");
        }
    }
}
