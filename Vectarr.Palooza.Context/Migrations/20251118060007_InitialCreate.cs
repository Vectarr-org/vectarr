using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Vectarr.Palooza.Context.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Address.Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Country = table.Column<int>(type: "integer", nullable: true),
                    Street1 = table.Column<string>(type: "text", nullable: true),
                    Street2 = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    PostalCode = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address.Addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    Discriminator = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: true),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: true),
                    Role = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Application.Applications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application.Applications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationSubscription.ApplicationSubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationId = table.Column<int>(type: "integer", nullable: false),
                    Subscription = table.Column<int>(type: "integer", nullable: false),
                    BillingCycleStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaymentsFailureStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SubscriptionStatus = table.Column<int>(type: "integer", nullable: false),
                    CancelReason = table.Column<int>(type: "integer", nullable: true),
                    SubscriptionPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationSubscription.ApplicationSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationSubscription.ApplicationSubscriptions_Applicatio~",
                        column: x => x.ApplicationId,
                        principalTable: "Application.Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdentityId = table.Column<int>(type: "integer", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
                    ReferralId = table.Column<int>(type: "integer", nullable: true),
                    ReferralCode = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_AspNetUsers_ReferralId",
                        column: x => x.ReferralId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Referral.Referral",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReferredUserId = table.Column<int>(type: "integer", nullable: false),
                    FreeTubEarned = table.Column<bool>(type: "boolean", nullable: false),
                    ApplicationId = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Referral.Referral", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Referral.Referral_Application.Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Application.Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Referral.Referral_AspNetUsers_ReferredUserId",
                        column: x => x.ReferredUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Creator.Creators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserAccountId = table.Column<int>(type: "integer", nullable: false),
                    Bio = table.Column<string>(type: "text", nullable: true),
                    ProfilePictureId = table.Column<int>(type: "integer", nullable: true),
                    Company = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Creator.Creators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Creator.Creators_AspNetUsers_UserAccountId",
                        column: x => x.UserAccountId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Job.Jobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ManufacturingType = table.Column<int>(type: "integer", nullable: true),
                    Material = table.Column<int>(type: "integer", nullable: true),
                    CostVsSpeed = table.Column<int>(type: "integer", nullable: false),
                    Tolerance = table.Column<int>(type: "integer", nullable: true),
                    SourceFinish = table.Column<int>(type: "integer", nullable: true),
                    AdditionalNotes = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job.Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Job.Jobs_Creator.Creators_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Creator.Creators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Identity.Identities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    EmailAddress = table.Column<string>(type: "text", nullable: false),
                    AddressId = table.Column<int>(type: "integer", nullable: true),
                    ProfilePictureId = table.Column<int>(type: "integer", nullable: true),
                    Gender = table.Column<int>(type: "integer", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Age = table.Column<int>(type: "integer", nullable: true),
                    IsEmailSubscribed = table.Column<bool>(type: "boolean", nullable: false),
                    TaxRate = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identity.Identities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Identity.Identities_Address.Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address.Addresses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Image.Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OriginalPath = table.Column<string>(type: "text", nullable: false),
                    AltText = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    StorageUrl = table.Column<string>(type: "text", nullable: true),
                    RelativeThumbnailOriginalPath = table.Column<string>(type: "text", nullable: true),
                    WorkshopId = table.Column<int>(type: "integer", nullable: true),
                    JobId = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image.Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Image.Images_Job.Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Job.Jobs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Workshop.Workshops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserAccountId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    WebsiteUrl = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false),
                    LogoId = table.Column<int>(type: "integer", nullable: true),
                    AddressId = table.Column<int>(type: "integer", nullable: true),
                    CompanyName = table.Column<string>(type: "text", nullable: true),
                    CompanyInfo = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workshop.Workshops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workshop.Workshops_Address.Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address.Addresses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Workshop.Workshops_AspNetUsers_UserAccountId",
                        column: x => x.UserAccountId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Workshop.Workshops_Image.Images_LogoId",
                        column: x => x.LogoId,
                        principalTable: "Image.Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "WorkshopCapability.WorkshopCapabilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkshopId = table.Column<int>(type: "integer", nullable: false),
                    Capability = table.Column<int>(type: "integer", nullable: true),
                    OtherCapability = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkshopCapability.WorkshopCapabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkshopCapability.WorkshopCapabilities_Workshop.Workshops_~",
                        column: x => x.WorkshopId,
                        principalTable: "Workshop.Workshops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkshopCertification.WorkshopCertifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkshopId = table.Column<int>(type: "integer", nullable: false),
                    Certification = table.Column<int>(type: "integer", nullable: true),
                    OtherCertification = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkshopCertification.WorkshopCertifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkshopCertification.WorkshopCertifications_Workshop.Works~",
                        column: x => x.WorkshopId,
                        principalTable: "Workshop.Workshops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkshopEquipment.WorkshopEquipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkshopId = table.Column<int>(type: "integer", nullable: false),
                    Equipment = table.Column<int>(type: "integer", nullable: true),
                    OtherEquipment = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkshopEquipment.WorkshopEquipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkshopEquipment.WorkshopEquipments_Workshop.Workshops_Wor~",
                        column: x => x.WorkshopId,
                        principalTable: "Workshop.Workshops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkshopSpecialization.WorkshopSpecializations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkshopId = table.Column<int>(type: "integer", nullable: false),
                    Specialization = table.Column<int>(type: "integer", nullable: true),
                    OtherSpecialization = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkshopSpecialization.WorkshopSpecializations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkshopSpecialization.WorkshopSpecializations_Workshop.Wor~",
                        column: x => x.WorkshopId,
                        principalTable: "Workshop.Workshops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Application.Applications_UserId",
                table: "Application.Applications",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationSubscription.ApplicationSubscriptions_Applicatio~",
                table: "ApplicationSubscription.ApplicationSubscriptions",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IdentityId",
                table: "AspNetUsers",
                column: "IdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ReferralId",
                table: "AspNetUsers",
                column: "ReferralId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Creator.Creators_ProfilePictureId",
                table: "Creator.Creators",
                column: "ProfilePictureId");

            migrationBuilder.CreateIndex(
                name: "IX_Creator.Creators_UserAccountId",
                table: "Creator.Creators",
                column: "UserAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Identity.Identities_AddressId",
                table: "Identity.Identities",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Identity.Identities_ProfilePictureId",
                table: "Identity.Identities",
                column: "ProfilePictureId");

            migrationBuilder.CreateIndex(
                name: "IX_Image.Images_JobId",
                table: "Image.Images",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Image.Images_WorkshopId",
                table: "Image.Images",
                column: "WorkshopId");

            migrationBuilder.CreateIndex(
                name: "IX_Job.Jobs_CreatorId",
                table: "Job.Jobs",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Referral.Referral_ApplicationId",
                table: "Referral.Referral",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Referral.Referral_ReferredUserId",
                table: "Referral.Referral",
                column: "ReferredUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Workshop.Workshops_AddressId",
                table: "Workshop.Workshops",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Workshop.Workshops_LogoId",
                table: "Workshop.Workshops",
                column: "LogoId");

            migrationBuilder.CreateIndex(
                name: "IX_Workshop.Workshops_UserAccountId",
                table: "Workshop.Workshops",
                column: "UserAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopCapability.WorkshopCapabilities_WorkshopId",
                table: "WorkshopCapability.WorkshopCapabilities",
                column: "WorkshopId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopCertification.WorkshopCertifications_WorkshopId",
                table: "WorkshopCertification.WorkshopCertifications",
                column: "WorkshopId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopEquipment.WorkshopEquipments_WorkshopId",
                table: "WorkshopEquipment.WorkshopEquipments",
                column: "WorkshopId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopSpecialization.WorkshopSpecializations_WorkshopId",
                table: "WorkshopSpecialization.WorkshopSpecializations",
                column: "WorkshopId");

            migrationBuilder.AddForeignKey(
                name: "FK_Application.Applications_AspNetUsers_UserId",
                table: "Application.Applications",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Identity.Identities_IdentityId",
                table: "AspNetUsers",
                column: "IdentityId",
                principalTable: "Identity.Identities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Creator.Creators_Image.Images_ProfilePictureId",
                table: "Creator.Creators",
                column: "ProfilePictureId",
                principalTable: "Image.Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Identity.Identities_Image.Images_ProfilePictureId",
                table: "Identity.Identities",
                column: "ProfilePictureId",
                principalTable: "Image.Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Image.Images_Workshop.Workshops_WorkshopId",
                table: "Image.Images",
                column: "WorkshopId",
                principalTable: "Workshop.Workshops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Creator.Creators_AspNetUsers_UserAccountId",
                table: "Creator.Creators");

            migrationBuilder.DropForeignKey(
                name: "FK_Workshop.Workshops_AspNetUsers_UserAccountId",
                table: "Workshop.Workshops");

            migrationBuilder.DropForeignKey(
                name: "FK_Creator.Creators_Image.Images_ProfilePictureId",
                table: "Creator.Creators");

            migrationBuilder.DropForeignKey(
                name: "FK_Workshop.Workshops_Image.Images_LogoId",
                table: "Workshop.Workshops");

            migrationBuilder.DropTable(
                name: "ApplicationSubscription.ApplicationSubscriptions");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Referral.Referral");

            migrationBuilder.DropTable(
                name: "WorkshopCapability.WorkshopCapabilities");

            migrationBuilder.DropTable(
                name: "WorkshopCertification.WorkshopCertifications");

            migrationBuilder.DropTable(
                name: "WorkshopEquipment.WorkshopEquipments");

            migrationBuilder.DropTable(
                name: "WorkshopSpecialization.WorkshopSpecializations");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Application.Applications");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Identity.Identities");

            migrationBuilder.DropTable(
                name: "Image.Images");

            migrationBuilder.DropTable(
                name: "Job.Jobs");

            migrationBuilder.DropTable(
                name: "Workshop.Workshops");

            migrationBuilder.DropTable(
                name: "Creator.Creators");

            migrationBuilder.DropTable(
                name: "Address.Addresses");
        }
    }
}
