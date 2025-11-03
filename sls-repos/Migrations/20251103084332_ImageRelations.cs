using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sls_repos.Migrations
{
    /// <inheritdoc />
    public partial class ImageRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EditionId",
                table: "Images",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TournamentId",
                table: "Images",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Images_EditionId",
                table: "Images",
                column: "EditionId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_TournamentId",
                table: "Images",
                column: "TournamentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Editions_EditionId",
                table: "Images",
                column: "EditionId",
                principalTable: "Editions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Tournaments_TournamentId",
                table: "Images",
                column: "TournamentId",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Editions_EditionId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Tournaments_TournamentId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_EditionId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_TournamentId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "EditionId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "TournamentId",
                table: "Images");
        }
    }
}
