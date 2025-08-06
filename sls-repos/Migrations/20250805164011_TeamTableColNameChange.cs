using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sls_repos.Migrations
{
    /// <inheritdoc />
    public partial class TeamTableColNameChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Adress",
                table: "Teams",
                newName: "Address");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Teams",
                newName: "Adress");
        }
    }
}
