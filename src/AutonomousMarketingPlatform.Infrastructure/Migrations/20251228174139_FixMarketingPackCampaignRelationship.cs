using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutonomousMarketingPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixMarketingPackCampaignRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Verificar si la columna CampaignId1 existe antes de eliminarla
            var sql = @"
                DO $$
                BEGIN
                    -- Eliminar foreign key si existe
                    IF EXISTS (
                        SELECT 1 FROM information_schema.table_constraints 
                        WHERE constraint_name = 'FK_MarketingPacks_Campaigns_CampaignId1'
                        AND table_name = 'MarketingPacks'
                    ) THEN
                        ALTER TABLE ""MarketingPacks"" DROP CONSTRAINT ""FK_MarketingPacks_Campaigns_CampaignId1"";
                    END IF;
                    
                    -- Eliminar índice si existe
                    IF EXISTS (
                        SELECT 1 FROM pg_indexes 
                        WHERE indexname = 'IX_MarketingPacks_CampaignId1'
                    ) THEN
                        DROP INDEX IF EXISTS ""IX_MarketingPacks_CampaignId1"";
                    END IF;
                    
                    -- Eliminar columna si existe
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MarketingPacks' 
                        AND column_name = 'CampaignId1'
                    ) THEN
                        ALTER TABLE ""MarketingPacks"" DROP COLUMN ""CampaignId1"";
                    END IF;
                END $$;
            ";
            
            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CampaignId1",
                table: "MarketingPacks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketingPacks_CampaignId1",
                table: "MarketingPacks",
                column: "CampaignId1");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketingPacks_Campaigns_CampaignId1",
                table: "MarketingPacks",
                column: "CampaignId1",
                principalTable: "Campaigns",
                principalColumn: "Id");
        }
    }
}
