using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResearchManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class modifysettingscsv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lathalaNatureOfInvolvement",
                table: "REMC_Settings");

            migrationBuilder.DropColumn(
                name: "tuklasNatureOfInvolvement",
                table: "REMC_Settings");

            migrationBuilder.AddColumn<string>(
                name: "lathalaInvolvement",
                table: "REMC_Settings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "tuklasInvolvement",
                table: "REMC_Settings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lathalaInvolvement",
                table: "REMC_Settings");

            migrationBuilder.DropColumn(
                name: "tuklasInvolvement",
                table: "REMC_Settings");

            migrationBuilder.AddColumn<string>(
                name: "lathalaNatureOfInvolvement",
                table: "REMC_Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tuklasNatureOfInvolvement",
                table: "REMC_Settings",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
