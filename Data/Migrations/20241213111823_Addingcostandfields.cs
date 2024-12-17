using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResearchManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class Addingcostandfields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "field_of_Study",
                table: "Production",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "total_project_Cost",
                table: "Production",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "field_of_Study",
                table: "Production");

            migrationBuilder.DropColumn(
                name: "total_project_Cost",
                table: "Production");
        }
    }
}
