using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoList.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDoItems_groupTasks_GroupId",
                table: "ToDoItems");

            migrationBuilder.AddColumn<bool>(
                name: "IsRecurring",
                table: "ToDoItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RecurringInterval",
                table: "ToDoItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledDateTime",
                table: "ToDoItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "ToDoItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "groupTasks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ToDoItems_UserId",
                table: "ToDoItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_groupTasks_UserId",
                table: "groupTasks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_groupTasks_AspNetUsers_UserId",
                table: "groupTasks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoItems_AspNetUsers_UserId",
                table: "ToDoItems",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoItems_groupTasks_GroupId",
                table: "ToDoItems",
                column: "GroupId",
                principalTable: "groupTasks",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_groupTasks_AspNetUsers_UserId",
                table: "groupTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_ToDoItems_AspNetUsers_UserId",
                table: "ToDoItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ToDoItems_groupTasks_GroupId",
                table: "ToDoItems");

            migrationBuilder.DropIndex(
                name: "IX_ToDoItems_UserId",
                table: "ToDoItems");

            migrationBuilder.DropIndex(
                name: "IX_groupTasks_UserId",
                table: "groupTasks");

            migrationBuilder.DropColumn(
                name: "IsRecurring",
                table: "ToDoItems");

            migrationBuilder.DropColumn(
                name: "RecurringInterval",
                table: "ToDoItems");

            migrationBuilder.DropColumn(
                name: "ScheduledDateTime",
                table: "ToDoItems");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ToDoItems");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "groupTasks");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoItems_groupTasks_GroupId",
                table: "ToDoItems",
                column: "GroupId",
                principalTable: "groupTasks",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
