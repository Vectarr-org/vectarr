using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vectarr.Palooza.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddingRejectionReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "WorkOrder.QualityCheckRequests",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "WorkOrder.QualityCheckRequests");
        }
    }
}
