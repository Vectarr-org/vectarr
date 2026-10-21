using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vectarr.Palooza.Context.Migrations
{
    /// <inheritdoc />
    public partial class UsInternationalSourcingAndCertificationReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocumentId",
                table: "WorkshopCertification.WorkshopCertifications",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "WorkshopCertification.WorkshopCertifications",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "WorkshopCertification.WorkshopCertifications",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewStatus",
                table: "WorkshopCertification.WorkshopCertifications",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "WorkshopCertification.WorkshopCertifications",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewedByUserId",
                table: "WorkshopCertification.WorkshopCertifications",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Country",
                table: "Workshop.Workshops",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsExportControlled",
                table: "Job.Jobs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsItar",
                table: "Job.Jobs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SourcingRegion",
                table: "Job.Jobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "DutyEstimate",
                table: "Job.Bids",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ShipFromCountry",
                table: "Job.Bids",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransitDays",
                table: "Job.Bids",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopCertification.WorkshopCertifications_DocumentId",
                table: "WorkshopCertification.WorkshopCertifications",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopCertification.WorkshopCertifications_ReviewedByUser~",
                table: "WorkshopCertification.WorkshopCertifications",
                column: "ReviewedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkshopCertification.WorkshopCertifications_AspNetUsers_Re~",
                table: "WorkshopCertification.WorkshopCertifications",
                column: "ReviewedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkshopCertification.WorkshopCertifications_Image.Images_D~",
                table: "WorkshopCertification.WorkshopCertifications",
                column: "DocumentId",
                principalTable: "Image.Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            // Copy the address country onto the shop, then default each bid's ship-from country from that shop.
            migrationBuilder.Sql("""
                UPDATE "Workshop.Workshops" AS w
                SET "Country" = a."Country"
                FROM "Address.Addresses" AS a
                WHERE w."AddressId" = a."Id"
                  AND w."Country" IS NULL
                  AND a."Country" IS NOT NULL;
                """);

            migrationBuilder.Sql("""
                UPDATE "Job.Bids" AS b
                SET "ShipFromCountry" = COALESCE(w."Country", a."Country")
                FROM "Workshop.Workshops" AS w
                LEFT JOIN "Address.Addresses" AS a ON w."AddressId" = a."Id"
                WHERE b."WorkshopId" = w."Id"
                  AND b."ShipFromCountry" IS NULL
                  AND COALESCE(w."Country", a."Country") IS NOT NULL;
                """);

            // DutyEstimate is the duties/tariff estimate. Seed it from the legacy Tarrifs column.
            migrationBuilder.Sql("""
                UPDATE "Job.Bids"
                SET "DutyEstimate" = "Tarrifs"
                WHERE "Tarrifs" IS NOT NULL
                  AND "DutyEstimate" = 0;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkshopCertification.WorkshopCertifications_AspNetUsers_Re~",
                table: "WorkshopCertification.WorkshopCertifications");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkshopCertification.WorkshopCertifications_Image.Images_D~",
                table: "WorkshopCertification.WorkshopCertifications");

            migrationBuilder.DropIndex(
                name: "IX_WorkshopCertification.WorkshopCertifications_DocumentId",
                table: "WorkshopCertification.WorkshopCertifications");

            migrationBuilder.DropIndex(
                name: "IX_WorkshopCertification.WorkshopCertifications_ReviewedByUser~",
                table: "WorkshopCertification.WorkshopCertifications");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "WorkshopCertification.WorkshopCertifications");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "WorkshopCertification.WorkshopCertifications");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "WorkshopCertification.WorkshopCertifications");

            migrationBuilder.DropColumn(
                name: "ReviewStatus",
                table: "WorkshopCertification.WorkshopCertifications");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "WorkshopCertification.WorkshopCertifications");

            migrationBuilder.DropColumn(
                name: "ReviewedByUserId",
                table: "WorkshopCertification.WorkshopCertifications");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Workshop.Workshops");

            migrationBuilder.DropColumn(
                name: "IsExportControlled",
                table: "Job.Jobs");

            migrationBuilder.DropColumn(
                name: "IsItar",
                table: "Job.Jobs");

            migrationBuilder.DropColumn(
                name: "SourcingRegion",
                table: "Job.Jobs");

            migrationBuilder.DropColumn(
                name: "DutyEstimate",
                table: "Job.Bids");

            migrationBuilder.DropColumn(
                name: "ShipFromCountry",
                table: "Job.Bids");

            migrationBuilder.DropColumn(
                name: "TransitDays",
                table: "Job.Bids");
        }
    }
}
