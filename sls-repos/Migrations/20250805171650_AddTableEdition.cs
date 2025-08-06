using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sls_repos.Migrations
{
    /// <inheritdoc />
    public partial class AddTableEdition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EditionId",
                table: "Tournaments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "EditionId",
                table: "Teams",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Edition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nuber = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Organizer = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Edition", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_EditionId",
                table: "Tournaments",
                column: "EditionId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_EditionId",
                table: "Teams",
                column: "EditionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Edition_EditionId",
                table: "Teams",
                column: "EditionId",
                principalTable: "Edition",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tournaments_Edition_EditionId",
                table: "Tournaments",
                column: "EditionId",
                principalTable: "Edition",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Edition_EditionId",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "FK_Tournaments_Edition_EditionId",
                table: "Tournaments");

            migrationBuilder.DropTable(
                name: "Edition");

            migrationBuilder.DropIndex(
                name: "IX_Tournaments_EditionId",
                table: "Tournaments");

            migrationBuilder.DropIndex(
                name: "IX_Teams_EditionId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "EditionId",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "EditionId",
                table: "Teams");
        }
    }
}
