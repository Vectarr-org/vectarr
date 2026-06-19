using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Vectarr.Palooza.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddingRejectionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "WorkOrder.QualityCheckRequests");

            migrationBuilder.CreateTable(
                name: "WorkOrder.WorkOrderQualityCheckRejections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RejectionReason = table.Column<string>(type: "text", nullable: true),
                    QualityCheckRequestId = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrder.WorkOrderQualityCheckRejections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkOrder.WorkOrderQualityCheckRejections_WorkOrder.Quality~",
                        column: x => x.QualityCheckRequestId,
                        principalTable: "WorkOrder.QualityCheckRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder.WorkOrderQualityCheckRejections_QualityCheckReque~",
                table: "WorkOrder.WorkOrderQualityCheckRejections",
                column: "QualityCheckRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkOrder.WorkOrderQualityCheckRejections");

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "WorkOrder.QualityCheckRequests",
                type: "text",
                nullable: true);
        }
    }
}
