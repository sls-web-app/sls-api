using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sls_repos.Migrations
{
    /// <inheritdoc />
    public partial class TeamTableColsAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Short",
                table: "Teams",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Short",
                table: "Teams");
        }
    }
}
