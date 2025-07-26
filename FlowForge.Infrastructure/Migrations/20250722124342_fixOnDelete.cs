using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoList.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixOnDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDoItems_groupTasks_GroupId",
                table: "ToDoItems");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoItems_groupTasks_GroupId",
                table: "ToDoItems",
                column: "GroupId",
                principalTable: "groupTasks",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDoItems_groupTasks_GroupId",
                table: "ToDoItems");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoItems_groupTasks_GroupId",
                table: "ToDoItems",
                column: "GroupId",
                principalTable: "groupTasks",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
