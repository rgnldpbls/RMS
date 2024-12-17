using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResearchManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingFundingtypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TypesofFunding",
                table: "Production",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TypesofFunding",
                table: "Production");
        }
    }
}
