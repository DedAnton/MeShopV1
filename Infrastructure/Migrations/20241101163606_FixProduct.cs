using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Dimentions_Width",
                table: "Products",
                newName: "Width");

            migrationBuilder.RenameColumn(
                name: "Dimentions_Weight",
                table: "Products",
                newName: "Weight");

            migrationBuilder.RenameColumn(
                name: "Dimentions_Height",
                table: "Products",
                newName: "Height");

            migrationBuilder.RenameColumn(
                name: "Dimentions_Depth",
                table: "Products",
                newName: "Depth");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Width",
                table: "Products",
                newName: "Dimentions_Width");

            migrationBuilder.RenameColumn(
                name: "Weight",
                table: "Products",
                newName: "Dimentions_Weight");

            migrationBuilder.RenameColumn(
                name: "Height",
                table: "Products",
                newName: "Dimentions_Height");

            migrationBuilder.RenameColumn(
                name: "Depth",
                table: "Products",
                newName: "Dimentions_Depth");
        }
    }
}
