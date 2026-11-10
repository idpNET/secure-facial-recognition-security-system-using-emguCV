using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FaceAuth.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsageEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApiTokenId = table.Column<Guid>(type: "uuid", nullable: true),
                    Endpoint = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Method = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    StatusCode = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsageEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Prefix = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Scopes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RevokedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastUsedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiTokens_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EndUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ExternalUserId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PasscodeHash = table.Column<byte[]>(type: "bytea", nullable: false),
                    PasscodeSalt = table.Column<byte[]>(type: "bytea", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EndUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EndUsers_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FaceSamples",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EndUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Embedding = table.Column<float[]>(type: "real[]", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaceSamples", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaceSamples_EndUsers_EndUserId",
                        column: x => x.EndUserId,
                        principalTable: "EndUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiTokens_Prefix",
                table: "ApiTokens",
                column: "Prefix",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiTokens_TenantId",
                table: "ApiTokens",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_EndUsers_TenantId_UserName",
                table: "EndUsers",
                columns: new[] { "TenantId", "UserName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FaceSamples_EndUserId",
                table: "FaceSamples",
                column: "EndUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Name",
                table: "Tenants",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_UsageEvents_TenantId_Timestamp",
                table: "UsageEvents",
                columns: new[] { "TenantId", "Timestamp" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiTokens");

            migrationBuilder.DropTable(
                name: "FaceSamples");

            migrationBuilder.DropTable(
                name: "UsageEvents");

            migrationBuilder.DropTable(
                name: "EndUsers");

            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
