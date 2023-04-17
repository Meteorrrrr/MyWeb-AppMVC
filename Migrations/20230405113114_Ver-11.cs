using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_1.Migrations
{
    /// <inheritdoc />
    public partial class Ver11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_Category_ParentCategoryId",
                table: "Category");

            migrationBuilder.DropTable(
                name: "Lien he");

            migrationBuilder.RenameColumn(
                name: "ParentCategoryId",
                table: "Category",
                newName: "ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Category_ParentCategoryId",
                table: "Category",
                newName: "IX_Category_ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_Category_ParentId",
                table: "Category",
                column: "ParentId",
                principalTable: "Category",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_Category_ParentId",
                table: "Category");

            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "Category",
                newName: "ParentCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Category_ParentId",
                table: "Category",
                newName: "IX_Category_ParentCategoryId");

            migrationBuilder.CreateTable(
                name: "Lien he",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "ntext", nullable: false),
                    DateSend = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lien he", x => x.id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Category_Category_ParentCategoryId",
                table: "Category",
                column: "ParentCategoryId",
                principalTable: "Category",
                principalColumn: "Id");
        }
    }
}
