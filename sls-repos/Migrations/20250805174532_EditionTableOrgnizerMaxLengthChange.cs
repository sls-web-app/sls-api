using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sls_repos.Migrations
{
    /// <inheritdoc />
    public partial class EditionTableOrgnizerMaxLengthChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Edition_EditionId",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "FK_Tournaments_Edition_EditionId",
                table: "Tournaments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Edition",
                table: "Edition");

            migrationBuilder.RenameTable(
                name: "Edition",
                newName: "Editions");

            migrationBuilder.AlterColumn<string>(
                name: "Organizer",
                table: "Editions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Editions",
                table: "Editions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Editions_EditionId",
                table: "Teams",
                column: "EditionId",
                principalTable: "Editions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tournaments_Editions_EditionId",
                table: "Tournaments",
                column: "EditionId",
                principalTable: "Editions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Editions_EditionId",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "FK_Tournaments_Editions_EditionId",
                table: "Tournaments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Editions",
                table: "Editions");

            migrationBuilder.RenameTable(
                name: "Editions",
                newName: "Edition");

            migrationBuilder.AlterColumn<string>(
                name: "Organizer",
                table: "Edition",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Edition",
                table: "Edition",
                column: "Id");

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
    }
}
