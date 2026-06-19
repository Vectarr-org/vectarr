using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Vectarr.Palooza.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddingJobQCs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image.Images_Job.QualityCheckRequests_QualityCheckRequestId",
                table: "Image.Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Job.QualityCheckRequests_Job.WorkOrders_WorkOrderId",
                table: "Job.QualityCheckRequests");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Job.QualityCheckRequests");

            migrationBuilder.DropColumn(
                name: "Passed",
                table: "Job.QualityCheckRequests");

            migrationBuilder.DropColumn(
                name: "ResultNotes",
                table: "Job.QualityCheckRequests");

            migrationBuilder.RenameColumn(
                name: "WorkOrderId",
                table: "Job.QualityCheckRequests",
                newName: "JobId");

            migrationBuilder.RenameIndex(
                name: "IX_Job.QualityCheckRequests_WorkOrderId",
                table: "Job.QualityCheckRequests",
                newName: "IX_Job.QualityCheckRequests_JobId");

            migrationBuilder.RenameColumn(
                name: "QualityCheckRequestId",
                table: "Image.Images",
                newName: "WorkOrderQualityCheckRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Image.Images_QualityCheckRequestId",
                table: "Image.Images",
                newName: "IX_Image.Images_WorkOrderQualityCheckRequestId");

            migrationBuilder.CreateTable(
                name: "WorkOrder.QualityCheckRequests",
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
                    table.PrimaryKey("PK_WorkOrder.QualityCheckRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkOrder.QualityCheckRequests_Job.WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "Job.WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder.QualityCheckRequests_WorkOrderId",
                table: "WorkOrder.QualityCheckRequests",
                column: "WorkOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Image.Images_WorkOrder.QualityCheckRequests_WorkOrderQualit~",
                table: "Image.Images",
                column: "WorkOrderQualityCheckRequestId",
                principalTable: "WorkOrder.QualityCheckRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Job.QualityCheckRequests_Job.Jobs_JobId",
                table: "Job.QualityCheckRequests",
                column: "JobId",
                principalTable: "Job.Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image.Images_WorkOrder.QualityCheckRequests_WorkOrderQualit~",
                table: "Image.Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Job.QualityCheckRequests_Job.Jobs_JobId",
                table: "Job.QualityCheckRequests");

            migrationBuilder.DropTable(
                name: "WorkOrder.QualityCheckRequests");

            migrationBuilder.RenameColumn(
                name: "JobId",
                table: "Job.QualityCheckRequests",
                newName: "WorkOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Job.QualityCheckRequests_JobId",
                table: "Job.QualityCheckRequests",
                newName: "IX_Job.QualityCheckRequests_WorkOrderId");

            migrationBuilder.RenameColumn(
                name: "WorkOrderQualityCheckRequestId",
                table: "Image.Images",
                newName: "QualityCheckRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Image.Images_WorkOrderQualityCheckRequestId",
                table: "Image.Images",
                newName: "IX_Image.Images_QualityCheckRequestId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Job.QualityCheckRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Passed",
                table: "Job.QualityCheckRequests",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResultNotes",
                table: "Job.QualityCheckRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Image.Images_Job.QualityCheckRequests_QualityCheckRequestId",
                table: "Image.Images",
                column: "QualityCheckRequestId",
                principalTable: "Job.QualityCheckRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Job.QualityCheckRequests_Job.WorkOrders_WorkOrderId",
                table: "Job.QualityCheckRequests",
                column: "WorkOrderId",
                principalTable: "Job.WorkOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
