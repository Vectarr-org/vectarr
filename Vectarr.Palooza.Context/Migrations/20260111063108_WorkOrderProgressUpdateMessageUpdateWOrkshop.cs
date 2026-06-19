using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vectarr.Palooza.Context.Migrations
{
    /// <inheritdoc />
    public partial class WorkOrderProgressUpdateMessageUpdateWOrkshop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workshop.WorkshopCustomProgressMessages_Job.WorkOrderProgre~",
                table: "Workshop.WorkshopCustomProgressMessages");

            migrationBuilder.DropIndex(
                name: "IX_Workshop.WorkshopCustomProgressMessages_WorkOrderProgressUp~",
                table: "Workshop.WorkshopCustomProgressMessages");

            migrationBuilder.RenameColumn(
                name: "WorkOrderProgressUpdateId",
                table: "Workshop.WorkshopCustomProgressMessages",
                newName: "WorkshopId");

            migrationBuilder.AddColumn<int>(
                name: "WorkshopCustomProgressMessageId",
                table: "Job.WorkOrderProgressUpdates",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workshop.WorkshopCustomProgressMessages_WorkshopId",
                table: "Workshop.WorkshopCustomProgressMessages",
                column: "WorkshopId");

            migrationBuilder.CreateIndex(
                name: "IX_Job.WorkOrderProgressUpdates_WorkshopCustomProgressMessageId",
                table: "Job.WorkOrderProgressUpdates",
                column: "WorkshopCustomProgressMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Job.WorkOrderProgressUpdates_Workshop.WorkshopCustomProgres~",
                table: "Job.WorkOrderProgressUpdates",
                column: "WorkshopCustomProgressMessageId",
                principalTable: "Workshop.WorkshopCustomProgressMessages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Workshop.WorkshopCustomProgressMessages_Workshop.Workshops_~",
                table: "Workshop.WorkshopCustomProgressMessages",
                column: "WorkshopId",
                principalTable: "Workshop.Workshops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Job.WorkOrderProgressUpdates_Workshop.WorkshopCustomProgres~",
                table: "Job.WorkOrderProgressUpdates");

            migrationBuilder.DropForeignKey(
                name: "FK_Workshop.WorkshopCustomProgressMessages_Workshop.Workshops_~",
                table: "Workshop.WorkshopCustomProgressMessages");

            migrationBuilder.DropIndex(
                name: "IX_Workshop.WorkshopCustomProgressMessages_WorkshopId",
                table: "Workshop.WorkshopCustomProgressMessages");

            migrationBuilder.DropIndex(
                name: "IX_Job.WorkOrderProgressUpdates_WorkshopCustomProgressMessageId",
                table: "Job.WorkOrderProgressUpdates");

            migrationBuilder.DropColumn(
                name: "WorkshopCustomProgressMessageId",
                table: "Job.WorkOrderProgressUpdates");

            migrationBuilder.RenameColumn(
                name: "WorkshopId",
                table: "Workshop.WorkshopCustomProgressMessages",
                newName: "WorkOrderProgressUpdateId");

            migrationBuilder.CreateIndex(
                name: "IX_Workshop.WorkshopCustomProgressMessages_WorkOrderProgressUp~",
                table: "Workshop.WorkshopCustomProgressMessages",
                column: "WorkOrderProgressUpdateId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Workshop.WorkshopCustomProgressMessages_Job.WorkOrderProgre~",
                table: "Workshop.WorkshopCustomProgressMessages",
                column: "WorkOrderProgressUpdateId",
                principalTable: "Job.WorkOrderProgressUpdates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
