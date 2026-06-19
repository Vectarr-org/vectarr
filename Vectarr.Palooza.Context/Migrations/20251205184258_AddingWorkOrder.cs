using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Vectarr.Palooza.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddingWorkOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image.Images_Job.Jobs_JobId",
                table: "Image.Images");

            migrationBuilder.AddColumn<int>(
                name: "ProgressStageId",
                table: "Image.Images",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProgressUpdateId",
                table: "Image.Images",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkOrderId",
                table: "Image.Images",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Job.WorkOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    AcceptedBidId = table.Column<int>(type: "integer", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    WorkshopId = table.Column<int>(type: "integer", nullable: false),
                    CurrentPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrentDeadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StartedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job.WorkOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Job.WorkOrders_Creator.Creators_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Creator.Creators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job.WorkOrders_Job.Bids_AcceptedBidId",
                        column: x => x.AcceptedBidId,
                        principalTable: "Job.Bids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job.WorkOrders_Job.Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Job.Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job.WorkOrders_Workshop.Workshops_WorkshopId",
                        column: x => x.WorkshopId,
                        principalTable: "Workshop.Workshops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Job.QuoteRevisions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkOrderId = table.Column<int>(type: "integer", nullable: false),
                    NewPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    OldPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    NewDeadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OldDeadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    ChangeSummary = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RespondedByCreatorId = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job.QuoteRevisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Job.QuoteRevisions_Creator.Creators_RespondedByCreatorId",
                        column: x => x.RespondedByCreatorId,
                        principalTable: "Creator.Creators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job.QuoteRevisions_Job.WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "Job.WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Job.WorkOrderProgressStages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkOrderId = table.Column<int>(type: "integer", nullable: false),
                    Stage = table.Column<int>(type: "integer", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    TrackingNumber = table.Column<string>(type: "text", nullable: true),
                    Carrier = table.Column<string>(type: "text", nullable: true),
                    EstimatedDelivery = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PassedQualityCheck = table.Column<bool>(type: "boolean", nullable: true),
                    QualityCheckNotes = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job.WorkOrderProgressStages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Job.WorkOrderProgressStages_Job.WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "Job.WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Job.ChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkOrderId = table.Column<int>(type: "integer", nullable: false),
                    MessageType = table.Column<int>(type: "integer", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: true),
                    WorkshopId = table.Column<int>(type: "integer", nullable: true),
                    Message = table.Column<string>(type: "text", nullable: true),
                    QuoteRevisionId = table.Column<int>(type: "integer", nullable: true),
                    ProgressStageId = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job.ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Job.ChatMessages_Creator.Creators_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Creator.Creators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job.ChatMessages_Job.QuoteRevisions_QuoteRevisionId",
                        column: x => x.QuoteRevisionId,
                        principalTable: "Job.QuoteRevisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job.ChatMessages_Job.WorkOrderProgressStages_ProgressStageId",
                        column: x => x.ProgressStageId,
                        principalTable: "Job.WorkOrderProgressStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job.ChatMessages_Job.WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "Job.WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Job.ChatMessages_Workshop.Workshops_WorkshopId",
                        column: x => x.WorkshopId,
                        principalTable: "Workshop.Workshops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Job.WorkOrderProgressUpdates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProgressStageId = table.Column<int>(type: "integer", nullable: false),
                    PercentComplete = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job.WorkOrderProgressUpdates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Job.WorkOrderProgressUpdates_Job.WorkOrderProgressStages_Pr~",
                        column: x => x.ProgressStageId,
                        principalTable: "Job.WorkOrderProgressStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Image.Images_ProgressStageId",
                table: "Image.Images",
                column: "ProgressStageId");

            migrationBuilder.CreateIndex(
                name: "IX_Image.Images_ProgressUpdateId",
                table: "Image.Images",
                column: "ProgressUpdateId");

            migrationBuilder.CreateIndex(
                name: "IX_Image.Images_WorkOrderId",
                table: "Image.Images",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Job.ChatMessages_CreatorId",
                table: "Job.ChatMessages",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Job.ChatMessages_ProgressStageId",
                table: "Job.ChatMessages",
                column: "ProgressStageId");

            migrationBuilder.CreateIndex(
                name: "IX_Job.ChatMessages_QuoteRevisionId",
                table: "Job.ChatMessages",
                column: "QuoteRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Job.ChatMessages_WorkOrderId",
                table: "Job.ChatMessages",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Job.ChatMessages_WorkshopId",
                table: "Job.ChatMessages",
                column: "WorkshopId");

            migrationBuilder.CreateIndex(
                name: "IX_Job.QuoteRevisions_RespondedByCreatorId",
                table: "Job.QuoteRevisions",
                column: "RespondedByCreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Job.QuoteRevisions_WorkOrderId",
                table: "Job.QuoteRevisions",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Job.WorkOrderProgressStages_WorkOrderId",
                table: "Job.WorkOrderProgressStages",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Job.WorkOrderProgressUpdates_ProgressStageId",
                table: "Job.WorkOrderProgressUpdates",
                column: "ProgressStageId");

            migrationBuilder.CreateIndex(
                name: "IX_Job.WorkOrders_AcceptedBidId",
                table: "Job.WorkOrders",
                column: "AcceptedBidId");

            migrationBuilder.CreateIndex(
                name: "IX_Job.WorkOrders_CreatorId",
                table: "Job.WorkOrders",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Job.WorkOrders_JobId",
                table: "Job.WorkOrders",
                column: "JobId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Job.WorkOrders_WorkshopId",
                table: "Job.WorkOrders",
                column: "WorkshopId");

            migrationBuilder.AddForeignKey(
                name: "FK_Image.Images_Job.Jobs_JobId",
                table: "Image.Images",
                column: "JobId",
                principalTable: "Job.Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Image.Images_Job.WorkOrderProgressStages_ProgressStageId",
                table: "Image.Images",
                column: "ProgressStageId",
                principalTable: "Job.WorkOrderProgressStages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Image.Images_Job.WorkOrderProgressUpdates_ProgressUpdateId",
                table: "Image.Images",
                column: "ProgressUpdateId",
                principalTable: "Job.WorkOrderProgressUpdates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Image.Images_Job.WorkOrders_WorkOrderId",
                table: "Image.Images",
                column: "WorkOrderId",
                principalTable: "Job.WorkOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image.Images_Job.Jobs_JobId",
                table: "Image.Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Image.Images_Job.WorkOrderProgressStages_ProgressStageId",
                table: "Image.Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Image.Images_Job.WorkOrderProgressUpdates_ProgressUpdateId",
                table: "Image.Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Image.Images_Job.WorkOrders_WorkOrderId",
                table: "Image.Images");

            migrationBuilder.DropTable(
                name: "Job.ChatMessages");

            migrationBuilder.DropTable(
                name: "Job.WorkOrderProgressUpdates");

            migrationBuilder.DropTable(
                name: "Job.QuoteRevisions");

            migrationBuilder.DropTable(
                name: "Job.WorkOrderProgressStages");

            migrationBuilder.DropTable(
                name: "Job.WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_Image.Images_ProgressStageId",
                table: "Image.Images");

            migrationBuilder.DropIndex(
                name: "IX_Image.Images_ProgressUpdateId",
                table: "Image.Images");

            migrationBuilder.DropIndex(
                name: "IX_Image.Images_WorkOrderId",
                table: "Image.Images");

            migrationBuilder.DropColumn(
                name: "ProgressStageId",
                table: "Image.Images");

            migrationBuilder.DropColumn(
                name: "ProgressUpdateId",
                table: "Image.Images");

            migrationBuilder.DropColumn(
                name: "WorkOrderId",
                table: "Image.Images");

            migrationBuilder.AddForeignKey(
                name: "FK_Image.Images_Job.Jobs_JobId",
                table: "Image.Images",
                column: "JobId",
                principalTable: "Job.Jobs",
                principalColumn: "Id");
        }
    }
}
