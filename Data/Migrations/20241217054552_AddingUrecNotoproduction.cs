using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResearchManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingUrecNotoproduction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrecNo",
                table: "Production",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrecNo",
                table: "Production");
        }
    }
}
