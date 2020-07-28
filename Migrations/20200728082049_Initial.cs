using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GGStream.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Collection",
                columns: table => new
                {
                    URL = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    BaseColor = table.Column<string>(nullable: true),
                    Private = table.Column<bool>(nullable: false),
                    ShowHowTo = table.Column<bool>(nullable: false),
                    TeamsLink = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collection", x => x.URL);
                });

            migrationBuilder.CreateTable(
                name: "Stream",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    StreamKey = table.Column<string>(nullable: true),
                    CollectionURL = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stream", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Stream_Collection_CollectionURL",
                        column: x => x.CollectionURL,
                        principalTable: "Collection",
                        principalColumn: "URL",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stream_CollectionURL",
                table: "Stream",
                column: "CollectionURL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stream");

            migrationBuilder.DropTable(
                name: "Collection");
        }
    }
}
