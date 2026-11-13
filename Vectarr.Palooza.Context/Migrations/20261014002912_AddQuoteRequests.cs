using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Vectarr.Palooza.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddQuoteRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVectarrAdmin",
                table: "Identity.Identities",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Quote.QuoteRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReferenceNumber = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AccessToken = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IdentityId = table.Column<int>(type: "integer", nullable: true),
                    JobId = table.Column<int>(type: "integer", nullable: true),
                    ContactName = table.Column<string>(type: "text", nullable: false),
                    ContactEmail = table.Column<string>(type: "text", nullable: false),
                    Company = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    ShippingStreet1 = table.Column<string>(type: "text", nullable: true),
                    ShippingStreet2 = table.Column<string>(type: "text", nullable: true),
                    ShippingCity = table.Column<string>(type: "text", nullable: true),
                    ShippingState = table.Column<string>(type: "text", nullable: true),
                    ShippingPostalCode = table.Column<string>(type: "text", nullable: true),
                    ShippingCountry = table.Column<string>(type: "text", nullable: true),
                    PartName = table.Column<string>(type: "text", nullable: true),
                    PartNumber = table.Column<string>(type: "text", nullable: true),
                    Revision = table.Column<string>(type: "text", nullable: true),
                    BuyerNotes = table.Column<string>(type: "text", nullable: true),
                    SourceFormat = table.Column<string>(type: "text", nullable: true),
                    IsAssembly = table.Column<bool>(type: "boolean", nullable: false),
                    AssemblyPartCount = table.Column<int>(type: "integer", nullable: true),
                    MaterialId = table.Column<string>(type: "text", nullable: true),
                    MaterialName = table.Column<string>(type: "text", nullable: true),
                    ProcessId = table.Column<string>(type: "text", nullable: true),
                    ProcessName = table.Column<string>(type: "text", nullable: true),
                    FinishId = table.Column<string>(type: "text", nullable: true),
                    FinishName = table.Column<string>(type: "text", nullable: true),
                    ToleranceId = table.Column<string>(type: "text", nullable: true),
                    ToleranceLabel = table.Column<string>(type: "text", nullable: true),
                    SpeedId = table.Column<string>(type: "text", nullable: true),
                    SpeedLabel = table.Column<string>(type: "text", nullable: true),
                    InspectionDocs = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    AmountDue = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    AmountPaid = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    Currency = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    LeadDays = table.Column<int>(type: "integer", nullable: true),
                    ShipByDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BboxXmm = table.Column<double>(type: "double precision", nullable: true),
                    BboxYmm = table.Column<double>(type: "double precision", nullable: true),
                    BboxZmm = table.Column<double>(type: "double precision", nullable: true),
                    VolumeMm3 = table.Column<double>(type: "double precision", nullable: true),
                    QuoteSnapshotJson = table.Column<string>(type: "text", nullable: true),
                    CadFileId = table.Column<int>(type: "integer", nullable: true),
                    CadFileName = table.Column<string>(type: "text", nullable: true),
                    DrawingFileId = table.Column<int>(type: "integer", nullable: true),
                    DrawingFileName = table.Column<string>(type: "text", nullable: true),
                    StripeCheckoutSessionId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    StripePaymentIntentId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AdminNotes = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quote.QuoteRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quote.QuoteRequests_Identity.Identities_IdentityId",
                        column: x => x.IdentityId,
                        principalTable: "Identity.Identities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Quote.QuoteRequests_Image.Images_CadFileId",
                        column: x => x.CadFileId,
                        principalTable: "Image.Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Quote.QuoteRequests_Image.Images_DrawingFileId",
                        column: x => x.DrawingFileId,
                        principalTable: "Image.Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Quote.QuoteRequests_Job.Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Job.Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quote.QuoteRequests_CadFileId",
                table: "Quote.QuoteRequests",
                column: "CadFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Quote.QuoteRequests_DrawingFileId",
                table: "Quote.QuoteRequests",
                column: "DrawingFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Quote.QuoteRequests_IdentityId",
                table: "Quote.QuoteRequests",
                column: "IdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_Quote.QuoteRequests_JobId",
                table: "Quote.QuoteRequests",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Quote.QuoteRequests_ReferenceNumber",
                table: "Quote.QuoteRequests",
                column: "ReferenceNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Quote.QuoteRequests");

            migrationBuilder.DropColumn(
                name: "IsVectarrAdmin",
                table: "Identity.Identities");
        }
    }
}
