using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vectarr.Palooza.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddingBidAndQCUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoAcceptQcReport",
                table: "Job.Jobs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Shipping",
                table: "Job.Bids",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Tarrifs",
                table: "Job.Bids",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Taxes",
                table: "Job.Bids",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoAcceptQcReport",
                table: "Job.Jobs");

            migrationBuilder.DropColumn(
                name: "Shipping",
                table: "Job.Bids");

            migrationBuilder.DropColumn(
                name: "Tarrifs",
                table: "Job.Bids");

            migrationBuilder.DropColumn(
                name: "Taxes",
                table: "Job.Bids");
        }
    }
}
