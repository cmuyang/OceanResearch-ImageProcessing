using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OceanResearch.API.Migrations
{
    /// <inheritdoc />
    public partial class s : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImageAnnotationSummaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Metadata = table.Column<string>(type: "TEXT", nullable: false),
                    TotalAnnotations = table.Column<int>(type: "INTEGER", nullable: false),
                    ClearestCount = table.Column<int>(type: "INTEGER", nullable: false),
                    ResearchValueCount = table.Column<int>(type: "INTEGER", nullable: false),
                    RemoveCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageAnnotationSummaries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageAnnotationSummaries_Metadata",
                table: "ImageAnnotationSummaries",
                column: "Metadata",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageAnnotationSummaries");
        }
    }
}
