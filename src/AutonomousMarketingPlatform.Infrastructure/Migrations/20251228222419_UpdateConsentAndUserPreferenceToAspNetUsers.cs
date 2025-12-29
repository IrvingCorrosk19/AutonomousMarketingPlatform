using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutonomousMarketingPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConsentAndUserPreferenceToAspNetUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Eliminar foreign keys antiguas a User
            migrationBuilder.DropForeignKey(
                name: "FK_Consents_User_UserId",
                table: "Consents");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferences_User_UserId",
                table: "UserPreferences");

            // Crear nuevas foreign keys a AspNetUsers usando la columna UserId existente
            migrationBuilder.AddForeignKey(
                name: "FK_Consents_AspNetUsers_UserId",
                table: "Consents",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferences_AspNetUsers_UserId",
                table: "UserPreferences",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar foreign keys a AspNetUsers
            migrationBuilder.DropForeignKey(
                name: "FK_Consents_AspNetUsers_UserId",
                table: "Consents");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferences_AspNetUsers_UserId",
                table: "UserPreferences");

            // Restaurar foreign keys antiguas a User
            migrationBuilder.AddForeignKey(
                name: "FK_Consents_User_UserId",
                table: "Consents",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferences_User_UserId",
                table: "UserPreferences",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
