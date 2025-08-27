using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sls_repos.Migrations
{
    /// <inheritdoc />
    public partial class Update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrganizingTeamId",
                table: "Tournaments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_OrganizingTeamId",
                table: "Tournaments",
                column: "OrganizingTeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tournaments_Teams_OrganizingTeamId",
                table: "Tournaments",
                column: "OrganizingTeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tournaments_Teams_OrganizingTeamId",
                table: "Tournaments");

            migrationBuilder.DropIndex(
                name: "IX_Tournaments_OrganizingTeamId",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "OrganizingTeamId",
                table: "Tournaments");
        }
    }
}
