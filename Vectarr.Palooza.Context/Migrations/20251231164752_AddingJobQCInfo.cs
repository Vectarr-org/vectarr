using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vectarr.Palooza.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddingJobQCInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QCPrecentage",
                table: "Job.Jobs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequireQcConfirmation",
                table: "Job.Jobs",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QCPrecentage",
                table: "Job.Jobs");

            migrationBuilder.DropColumn(
                name: "RequireQcConfirmation",
                table: "Job.Jobs");
        }
    }
}
