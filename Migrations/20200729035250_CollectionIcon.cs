using Microsoft.EntityFrameworkCore.Migrations;

namespace GGStream.Migrations
{
    public partial class CollectionIcon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "Icon",
                "Collection",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "Icon",
                "Collection");
        }
    }
}