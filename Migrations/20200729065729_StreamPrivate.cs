using Microsoft.EntityFrameworkCore.Migrations;

namespace GGStream.Migrations
{
    public partial class StreamPrivate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                "Private",
                "Stream",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "Private",
                "Stream");
        }
    }
}