using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutonomousMarketingPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Eliminar foreign keys si existen (idempotente)
            var sql = @"
                DO $$
                BEGIN
                    -- Eliminar foreign keys a User si existen
                    IF EXISTS (
                        SELECT 1 FROM information_schema.table_constraints 
                        WHERE constraint_name = 'FK_Consents_User_UserId1'
                        AND table_name = 'Consents'
                    ) THEN
                        ALTER TABLE ""Consents"" DROP CONSTRAINT ""FK_Consents_User_UserId1"";
                    END IF;
                    
                    IF EXISTS (
                        SELECT 1 FROM information_schema.table_constraints 
                        WHERE constraint_name = 'FK_UserPreferences_User_UserId1'
                        AND table_name = 'UserPreferences'
                    ) THEN
                        ALTER TABLE ""UserPreferences"" DROP CONSTRAINT ""FK_UserPreferences_User_UserId1"";
                    END IF;
                    
                    -- Eliminar índices si existen
                    IF EXISTS (
                        SELECT 1 FROM pg_indexes 
                        WHERE indexname = 'IX_UserPreferences_UserId1'
                    ) THEN
                        DROP INDEX IF EXISTS ""IX_UserPreferences_UserId1"";
                    END IF;
                    
                    IF EXISTS (
                        SELECT 1 FROM pg_indexes 
                        WHERE indexname = 'IX_Consents_UserId1'
                    ) THEN
                        DROP INDEX IF EXISTS ""IX_Consents_UserId1"";
                    END IF;
                    
                    -- Eliminar columnas si existen
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'UserPreferences' 
                        AND column_name = 'UserId1'
                    ) THEN
                        ALTER TABLE ""UserPreferences"" DROP COLUMN ""UserId1"";
                    END IF;
                    
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Consents' 
                        AND column_name = 'UserId1'
                    ) THEN
                        ALTER TABLE ""Consents"" DROP COLUMN ""UserId1"";
                    END IF;
                    
                    -- Eliminar foreign key de User a Tenants si existe
                    IF EXISTS (
                        SELECT 1 FROM information_schema.table_constraints 
                        WHERE constraint_name = 'FK_User_Tenants_TenantId'
                        AND table_name = 'User'
                    ) THEN
                        ALTER TABLE ""User"" DROP CONSTRAINT ""FK_User_Tenants_TenantId"";
                    END IF;
                    
                    -- Eliminar tabla User si existe
                    IF EXISTS (
                        SELECT 1 FROM information_schema.tables 
                        WHERE table_name = 'User'
                    ) THEN
                        DROP TABLE ""User"";
                    END IF;
                END $$;
            ";
            
            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "UserPreferences",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Consents",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EmailVerificationToken = table.Column<string>(type: "text", nullable: true),
                    EmailVerified = table.Column<bool>(type: "boolean", nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_UserId1",
                table: "UserPreferences",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Consents_UserId1",
                table: "Consents",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_User_TenantId",
                table: "User",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_User_TenantId_Email",
                table: "User",
                columns: new[] { "TenantId", "Email" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Consents_User_UserId1",
                table: "Consents",
                column: "UserId1",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferences_User_UserId1",
                table: "UserPreferences",
                column: "UserId1",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
