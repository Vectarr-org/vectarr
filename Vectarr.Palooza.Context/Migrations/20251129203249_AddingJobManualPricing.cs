using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vectarr.Palooza.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddingJobManualPricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "InitialPrice",
                table: "Job.Jobs",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ManualPricing",
                table: "Job.Jobs",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InitialPrice",
                table: "Job.Jobs");

            migrationBuilder.DropColumn(
                name: "ManualPricing",
                table: "Job.Jobs");
        }
    }
}
