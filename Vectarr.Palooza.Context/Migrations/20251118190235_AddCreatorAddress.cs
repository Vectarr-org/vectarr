using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vectarr.Palooza.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatorAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "Creator.Creators",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Creator.Creators_AddressId",
                table: "Creator.Creators",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Creator.Creators_Address.Addresses_AddressId",
                table: "Creator.Creators",
                column: "AddressId",
                principalTable: "Address.Addresses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Creator.Creators_Address.Addresses_AddressId",
                table: "Creator.Creators");

            migrationBuilder.DropIndex(
                name: "IX_Creator.Creators_AddressId",
                table: "Creator.Creators");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Creator.Creators");
        }
    }
}
