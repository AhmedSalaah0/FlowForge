using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoList.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_groupTasks_AspNetUsers_UserId",
                table: "groupTasks");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "groupTasks",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_groupTasks_UserId",
                table: "groupTasks",
                newName: "IX_groupTasks_CreatedById");

            migrationBuilder.CreateTable(
                name: "groupUsers",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groupUsers", x => new { x.UserId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_groupUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_groupUsers_groupTasks_GroupId",
                        column: x => x.GroupId,
                        principalTable: "groupTasks",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_groupUsers_GroupId",
                table: "groupUsers",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_groupTasks_AspNetUsers_CreatedById",
                table: "groupTasks",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_groupTasks_AspNetUsers_CreatedById",
                table: "groupTasks");

            migrationBuilder.DropTable(
                name: "groupUsers");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "groupTasks",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_groupTasks_CreatedById",
                table: "groupTasks",
                newName: "IX_groupTasks_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_groupTasks_AspNetUsers_UserId",
                table: "groupTasks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
