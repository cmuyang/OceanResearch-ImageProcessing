using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OceanResearch.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMetadataToSelection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                table: "Selections",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "Selections");
        }
    }
}

