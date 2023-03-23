using Microsoft.EntityFrameworkCore.Migrations;

namespace EMovie.Migrations
{
    public partial class recreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MovieCategories",
                table: "Movies",
                newName: "MovieCategory");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MovieCategory",
                table: "Movies",
                newName: "MovieCategories");
        }
    }
}
