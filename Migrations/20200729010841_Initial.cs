using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GGStream.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Collection",
                table => new
                {
                    URL = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    BaseColor = table.Column<string>(nullable: true),
                    Private = table.Column<bool>(nullable: false),
                    InstructionType = table.Column<int>(nullable: false),
                    CallLink = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Collection", x => x.URL); });

            migrationBuilder.CreateTable(
                "Stream",
                table => new
                {
                    ID = table.Column<string>(nullable: false),
                    StreamKey = table.Column<string>(nullable: true),
                    CollectionURL = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stream", x => x.ID);
                    table.ForeignKey(
                        "FK_Stream_Collection_CollectionURL",
                        x => x.CollectionURL,
                        "Collection",
                        "URL",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                "IX_Stream_CollectionURL",
                "Stream",
                "CollectionURL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Stream");

            migrationBuilder.DropTable(
                "Collection");
        }
    }
}