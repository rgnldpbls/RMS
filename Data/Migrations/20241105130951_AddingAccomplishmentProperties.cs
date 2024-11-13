using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResearchManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingAccomplishmentProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Production",
                columns: table => new
                {
                    ProductionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ResearchTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LeadResearcherId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CoLeadResearcherId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MemberoneId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MembertwoId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MemberthreeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    College = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FundingAgency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FundingAmount = table.Column<double>(type: "float", nullable: true),
                    DateStarted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateCompleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequiredFile1Data = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    RequiredFile2Data = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ConditionalFileData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    RequiredFile1Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequiredFile2Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConditionalFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsStudentInvolved = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Production", x => x.ProductionId);
                    table.ForeignKey(
                        name: "FK_Production_AspNetUsers_CoLeadResearcherId",
                        column: x => x.CoLeadResearcherId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Production_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Production_AspNetUsers_LeadResearcherId",
                        column: x => x.LeadResearcherId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Production_AspNetUsers_MemberoneId",
                        column: x => x.MemberoneId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Production_AspNetUsers_MemberthreeId",
                        column: x => x.MemberthreeId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Production_AspNetUsers_MembertwoId",
                        column: x => x.MembertwoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Patent",
                columns: table => new
                {
                    patentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PatentNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationFormData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ApplicationFormFileName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patent", x => x.patentId);
                    table.ForeignKey(
                        name: "FK_Patent_Production_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "Production",
                        principalColumn: "ProductionId");
                });

            migrationBuilder.CreateTable(
                name: "Presentation",
                columns: table => new
                {
                    ConferenceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OrganizerOne = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizerTwo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PresenterOne = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PresenterTwo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PresenterThree = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PresenterFour = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PresenterFive = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateofPresentation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Venue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertificateofParticipationFileData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CertificateofParticipationFileName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presentation", x => x.ConferenceId);
                    table.ForeignKey(
                        name: "FK_Presentation_Production_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "Production",
                        principalColumn: "ProductionId");
                });

            migrationBuilder.CreateTable(
                name: "Publication",
                columns: table => new
                {
                    publicationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ArticleTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublicationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    JournalPubTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VolnoIssueNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssnIsbnEssn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DOI = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IndexJournal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SuppDocs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManuscriptJournalData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ManuscriptJournalFileName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publication", x => x.publicationId);
                    table.ForeignKey(
                        name: "FK_Publication_Production_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "Production",
                        principalColumn: "ProductionId");
                });

            migrationBuilder.CreateTable(
                name: "Utilization",
                columns: table => new
                {
                    UtilizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CertificateofUtilizationData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CertificateofUtilizationFileName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilization", x => x.UtilizationId);
                    table.ForeignKey(
                        name: "FK_Utilization_Production_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "Production",
                        principalColumn: "ProductionId");
                });

            migrationBuilder.CreateTable(
                name: "Citation",
                columns: table => new
                {
                    CitationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ArticleTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    publicationId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LeadResearcherId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CoLeadResearcherId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MemberoneId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MembertwoId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MemberthreeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OriginalArticlePublished = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewPublicationTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorsofNewArticle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewArticlePublished = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VolNoIssueNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pages = table.Column<int>(type: "int", nullable: true),
                    YearofPublication = table.Column<int>(type: "int", nullable: true),
                    Indexing = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CitationProofData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CitationProofFileName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citation", x => x.CitationId);
                    table.ForeignKey(
                        name: "FK_Citation_Production_CoLeadResearcherId",
                        column: x => x.CoLeadResearcherId,
                        principalTable: "Production",
                        principalColumn: "ProductionId");
                    table.ForeignKey(
                        name: "FK_Citation_Production_LeadResearcherId",
                        column: x => x.LeadResearcherId,
                        principalTable: "Production",
                        principalColumn: "ProductionId");
                    table.ForeignKey(
                        name: "FK_Citation_Production_MemberoneId",
                        column: x => x.MemberoneId,
                        principalTable: "Production",
                        principalColumn: "ProductionId");
                    table.ForeignKey(
                        name: "FK_Citation_Production_MemberthreeId",
                        column: x => x.MemberthreeId,
                        principalTable: "Production",
                        principalColumn: "ProductionId");
                    table.ForeignKey(
                        name: "FK_Citation_Production_MembertwoId",
                        column: x => x.MembertwoId,
                        principalTable: "Production",
                        principalColumn: "ProductionId");
                    table.ForeignKey(
                        name: "FK_Citation_Production_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "Production",
                        principalColumn: "ProductionId");
                    table.ForeignKey(
                        name: "FK_Citation_Publication_publicationId",
                        column: x => x.publicationId,
                        principalTable: "Publication",
                        principalColumn: "publicationId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Citation_CoLeadResearcherId",
                table: "Citation",
                column: "CoLeadResearcherId");

            migrationBuilder.CreateIndex(
                name: "IX_Citation_LeadResearcherId",
                table: "Citation",
                column: "LeadResearcherId");

            migrationBuilder.CreateIndex(
                name: "IX_Citation_MemberoneId",
                table: "Citation",
                column: "MemberoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Citation_MemberthreeId",
                table: "Citation",
                column: "MemberthreeId");

            migrationBuilder.CreateIndex(
                name: "IX_Citation_MembertwoId",
                table: "Citation",
                column: "MembertwoId");

            migrationBuilder.CreateIndex(
                name: "IX_Citation_ProductionId",
                table: "Citation",
                column: "ProductionId");

            migrationBuilder.CreateIndex(
                name: "IX_Citation_publicationId",
                table: "Citation",
                column: "publicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Patent_ProductionId",
                table: "Patent",
                column: "ProductionId");

            migrationBuilder.CreateIndex(
                name: "IX_Presentation_ProductionId",
                table: "Presentation",
                column: "ProductionId");

            migrationBuilder.CreateIndex(
                name: "IX_Production_CoLeadResearcherId",
                table: "Production",
                column: "CoLeadResearcherId");

            migrationBuilder.CreateIndex(
                name: "IX_Production_CreatedById",
                table: "Production",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Production_LeadResearcherId",
                table: "Production",
                column: "LeadResearcherId");

            migrationBuilder.CreateIndex(
                name: "IX_Production_MemberoneId",
                table: "Production",
                column: "MemberoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Production_MemberthreeId",
                table: "Production",
                column: "MemberthreeId");

            migrationBuilder.CreateIndex(
                name: "IX_Production_MembertwoId",
                table: "Production",
                column: "MembertwoId");

            migrationBuilder.CreateIndex(
                name: "IX_Publication_ProductionId",
                table: "Publication",
                column: "ProductionId");

            migrationBuilder.CreateIndex(
                name: "IX_Utilization_ProductionId",
                table: "Utilization",
                column: "ProductionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Citation");

            migrationBuilder.DropTable(
                name: "Patent");

            migrationBuilder.DropTable(
                name: "Presentation");

            migrationBuilder.DropTable(
                name: "Utilization");

            migrationBuilder.DropTable(
                name: "Publication");

            migrationBuilder.DropTable(
                name: "Production");
        }
    }
}
