using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class ToDoListV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "groupName",
                table: "groupTasks",
                newName: "groupTitle");

            migrationBuilder.AddColumn<string>(
                name: "ColorOptions",
                table: "groupTasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "SelectedColor",
                table: "groupTasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorOptions",
                table: "groupTasks");

            migrationBuilder.DropColumn(
                name: "SelectedColor",
                table: "groupTasks");

            migrationBuilder.RenameColumn(
                name: "groupTitle",
                table: "groupTasks",
                newName: "groupName");
        }
    }
}
