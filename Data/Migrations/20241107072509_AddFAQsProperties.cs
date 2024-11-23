using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResearchManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFAQsProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FAQs",
                columns: table => new
                {
                    FAQ_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    added_by = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    question_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    answer_id = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQs", x => x.FAQ_id);
                    table.ForeignKey(
                        name: "FK_FAQs_AspNetUsers_added_by",
                        column: x => x.added_by,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FAQs_added_by",
                table: "FAQs",
                column: "added_by");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FAQs");
        }
    }
}
