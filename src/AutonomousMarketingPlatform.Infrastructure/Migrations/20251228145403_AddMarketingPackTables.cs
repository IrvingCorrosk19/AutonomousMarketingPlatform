using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutonomousMarketingPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketingPackTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketingPacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: true),
                    Strategy = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Metadata = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketingPacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketingPacks_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MarketingPacks_Contents_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Contents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignDrafts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    MarketingPackId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Objectives = table.Column<string>(type: "text", nullable: true),
                    TargetAudience = table.Column<string>(type: "text", nullable: true),
                    SuggestedChannels = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsConverted = table.Column<bool>(type: "boolean", nullable: false),
                    ConvertedCampaignId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignDrafts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignDrafts_Campaigns_ConvertedCampaignId",
                        column: x => x.ConvertedCampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CampaignDrafts_MarketingPacks_MarketingPackId",
                        column: x => x.MarketingPackId,
                        principalTable: "MarketingPacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GeneratedCopies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    MarketingPackId = table.Column<Guid>(type: "uuid", nullable: false),
                    CopyType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Hashtags = table.Column<string>(type: "text", nullable: true),
                    SuggestedChannel = table.Column<string>(type: "text", nullable: true),
                    PublicationChecklist = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneratedCopies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneratedCopies_MarketingPacks_MarketingPackId",
                        column: x => x.MarketingPackId,
                        principalTable: "MarketingPacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketingAssetPrompts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    MarketingPackId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Prompt = table.Column<string>(type: "text", nullable: false),
                    NegativePrompt = table.Column<string>(type: "text", nullable: true),
                    Parameters = table.Column<string>(type: "text", nullable: true),
                    SuggestedChannel = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketingAssetPrompts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketingAssetPrompts_MarketingPacks_MarketingPackId",
                        column: x => x.MarketingPackId,
                        principalTable: "MarketingPacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignDrafts_ConvertedCampaignId",
                table: "CampaignDrafts",
                column: "ConvertedCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignDrafts_MarketingPackId",
                table: "CampaignDrafts",
                column: "MarketingPackId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignDrafts_TenantId",
                table: "CampaignDrafts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignDrafts_TenantId_Status",
                table: "CampaignDrafts",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedCopies_MarketingPackId",
                table: "GeneratedCopies",
                column: "MarketingPackId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedCopies_TenantId",
                table: "GeneratedCopies",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedCopies_TenantId_MarketingPackId",
                table: "GeneratedCopies",
                columns: new[] { "TenantId", "MarketingPackId" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketingAssetPrompts_MarketingPackId",
                table: "MarketingAssetPrompts",
                column: "MarketingPackId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingAssetPrompts_TenantId",
                table: "MarketingAssetPrompts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingAssetPrompts_TenantId_MarketingPackId",
                table: "MarketingAssetPrompts",
                columns: new[] { "TenantId", "MarketingPackId" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketingPacks_CampaignId",
                table: "MarketingPacks",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingPacks_ContentId",
                table: "MarketingPacks",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingPacks_TenantId",
                table: "MarketingPacks",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingPacks_TenantId_ContentId",
                table: "MarketingPacks",
                columns: new[] { "TenantId", "ContentId" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketingPacks_TenantId_Status",
                table: "MarketingPacks",
                columns: new[] { "TenantId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CampaignDrafts");

            migrationBuilder.DropTable(
                name: "GeneratedCopies");

            migrationBuilder.DropTable(
                name: "MarketingAssetPrompts");

            migrationBuilder.DropTable(
                name: "MarketingPacks");
        }
    }
}
