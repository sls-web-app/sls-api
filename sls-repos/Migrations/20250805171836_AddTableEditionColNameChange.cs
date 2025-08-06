using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sls_repos.Migrations
{
    /// <inheritdoc />
    public partial class AddTableEditionColNameChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nuber",
                table: "Edition",
                newName: "Number");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Number",
                table: "Edition",
                newName: "Nuber");
        }
    }
}
