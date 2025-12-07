using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OceanResearch.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBase64FromImageRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Base64Data",
                table: "Images");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Base64Data",
                table: "Images",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
