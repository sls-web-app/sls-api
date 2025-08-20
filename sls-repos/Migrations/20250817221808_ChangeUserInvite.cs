using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sls_repos.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserInvite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserInvites_Email",
                table: "UserInvites");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "UserInvites");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "UserInvites");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "UserInvites");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "UserInvites");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "UserInvites",
                newName: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserInvites",
                newName: "TeamId");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "UserInvites",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "UserInvites",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "UserInvites",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "UserInvites",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserInvites_Email",
                table: "UserInvites",
                column: "Email",
                unique: true);
        }
    }
}
