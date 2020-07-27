using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GGStream.Migrations
{
    public partial class InitialSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Collection",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    URL = table.Column<string>(nullable: false),
                    BaseColor = table.Column<string>(nullable: true),
                    Private = table.Column<bool>(nullable: false),
                    ShowHowTo = table.Column<bool>(nullable: false),
                    TeamsLink = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collection", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stream",
                columns: table => new
                {
                    StreamKey = table.Column<Guid>(nullable: false),
                    CollectionId = table.Column<Guid>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    ShowHowTo = table.Column<bool>(nullable: false),
                    TeamsLink = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stream", x => x.StreamKey);
                    table.ForeignKey(
                        name: "FK_Stream_Collection_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stream_CollectionId",
                table: "Stream",
                column: "CollectionId");
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
