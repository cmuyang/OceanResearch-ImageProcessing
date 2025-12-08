using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OceanResearch.API.Migrations
{
    /// <inheritdoc />
    public partial class AddNewSelectionFieldsAndUserProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 添加新字段到Selection表
            migrationBuilder.AddColumn<bool>(
                name: "IsClearest",
                table: "Selections",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasResearchValue",
                table: "Selections",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShouldRemove",
                table: "Selections",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            // 创建UserProgresses表
            migrationBuilder.CreateTable(
                name: "UserProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentPage = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProgresses", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProgresses");

            migrationBuilder.DropColumn(
                name: "IsClearest",
                table: "Selections");

            migrationBuilder.DropColumn(
                name: "HasResearchValue",
                table: "Selections");

            migrationBuilder.DropColumn(
                name: "ShouldRemove",
                table: "Selections");
        }
    }
}

