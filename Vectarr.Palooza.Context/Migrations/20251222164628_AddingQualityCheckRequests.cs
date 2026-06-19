using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Vectarr.Palooza.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddingQualityCheckRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "WorkshopSubmittedAt",
                table: "Job.WorkOrderProgressStages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QualityCheckRequestId",
                table: "Image.Images",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Job.QualityCheckRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkOrderId = table.Column<int>(type: "integer", nullable: false),
                    CheckType = table.Column<int>(type: "integer", nullable: false),
                    Instructions = table.Column<string>(type: "text", nullable: true),
                    Passed = table.Column<bool>(type: "boolean", nullable: true),
                    ResultNotes = table.Column<string>(type: "text", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job.QualityCheckRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Job.QualityCheckRequests_Job.WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "Job.WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Image.Images_QualityCheckRequestId",
                table: "Image.Images",
                column: "QualityCheckRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Job.QualityCheckRequests_WorkOrderId",
                table: "Job.QualityCheckRequests",
                column: "WorkOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Image.Images_Job.QualityCheckRequests_QualityCheckRequestId",
                table: "Image.Images",
                column: "QualityCheckRequestId",
                principalTable: "Job.QualityCheckRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image.Images_Job.QualityCheckRequests_QualityCheckRequestId",
                table: "Image.Images");

            migrationBuilder.DropTable(
                name: "Job.QualityCheckRequests");

            migrationBuilder.DropIndex(
                name: "IX_Image.Images_QualityCheckRequestId",
                table: "Image.Images");

            migrationBuilder.DropColumn(
                name: "WorkshopSubmittedAt",
                table: "Job.WorkOrderProgressStages");

            migrationBuilder.DropColumn(
                name: "QualityCheckRequestId",
                table: "Image.Images");
        }
    }
}
