using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Vectarr.Palooza.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddingBids : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WinningBidId",
                table: "Job.Jobs",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Job.Bids",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    WorkshopId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    EstimatedDaysToComplete = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job.Bids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Job.Bids_Job.Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Job.Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Job.Bids_Workshop.Workshops_WorkshopId",
                        column: x => x.WorkshopId,
                        principalTable: "Workshop.Workshops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Job.Jobs_WinningBidId",
                table: "Job.Jobs",
                column: "WinningBidId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Job.Bids_JobId",
                table: "Job.Bids",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Job.Bids_WorkshopId",
                table: "Job.Bids",
                column: "WorkshopId");

            migrationBuilder.AddForeignKey(
                name: "FK_Job.Jobs_Job.Bids_WinningBidId",
                table: "Job.Jobs",
                column: "WinningBidId",
                principalTable: "Job.Bids",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Job.Jobs_Job.Bids_WinningBidId",
                table: "Job.Jobs");

            migrationBuilder.DropTable(
                name: "Job.Bids");

            migrationBuilder.DropIndex(
                name: "IX_Job.Jobs_WinningBidId",
                table: "Job.Jobs");

            migrationBuilder.DropColumn(
                name: "WinningBidId",
                table: "Job.Jobs");
        }
    }
}
