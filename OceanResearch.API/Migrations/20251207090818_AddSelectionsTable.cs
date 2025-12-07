using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OceanResearch.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSelectionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Selections_Images_ImageId",
                table: "Selections");

            migrationBuilder.DropForeignKey(
                name: "FK_Selections_Users_UserId",
                table: "Selections");

            migrationBuilder.DropIndex(
                name: "IX_Selections_ImageId",
                table: "Selections");

            migrationBuilder.DropIndex(
                name: "IX_Selections_UserId",
                table: "Selections");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "Selections",
                newName: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Selections",
                newName: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Selections_ImageId",
                table: "Selections",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Selections_UserId",
                table: "Selections",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Selections_Images_ImageId",
                table: "Selections",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Selections_Users_UserId",
                table: "Selections",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
