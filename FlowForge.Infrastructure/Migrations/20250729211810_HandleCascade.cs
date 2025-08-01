using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoList.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HandleCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_AspNetUsers_CreatedById",
                table: "ProjectTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_ProjectSections_SectionId",
                table: "ProjectTasks");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_AspNetUsers_CreatedById",
                table: "ProjectTasks",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_ProjectSections_SectionId",
                table: "ProjectTasks",
                column: "SectionId",
                principalTable: "ProjectSections",
                principalColumn: "SectionId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_AspNetUsers_CreatedById",
                table: "ProjectTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_ProjectSections_SectionId",
                table: "ProjectTasks");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_AspNetUsers_CreatedById",
                table: "ProjectTasks",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_ProjectSections_SectionId",
                table: "ProjectTasks",
                column: "SectionId",
                principalTable: "ProjectSections",
                principalColumn: "SectionId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
