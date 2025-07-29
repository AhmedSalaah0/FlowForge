using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoList.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSectionWithNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_AspNetUsers_MemberId",
                table: "ProjectTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_Projects_ProjectId",
                table: "ProjectTasks");

            migrationBuilder.RenameColumn(
                name: "ScheduledDateTime",
                table: "ProjectTasks",
                newName: "ScheduleDateTime");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "ProjectTasks",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectTasks_MemberId",
                table: "ProjectTasks",
                newName: "IX_ProjectTasks_CreatedById");

            migrationBuilder.AddColumn<Guid>(
                name: "SectionId",
                table: "ProjectTasks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProjectSections",
                columns: table => new
                {
                    SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSections", x => x.SectionId);
                    table.ForeignKey(
                        name: "FK_ProjectSections_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectSections_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_SectionId",
                table: "ProjectTasks",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSections_CreatedById",
                table: "ProjectSections",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSections_ProjectId",
                table: "ProjectSections",
                column: "ProjectId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_Projects_ProjectId",
                table: "ProjectTasks",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_Projects_ProjectId",
                table: "ProjectTasks");

            migrationBuilder.DropTable(
                name: "ProjectSections");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTasks_SectionId",
                table: "ProjectTasks");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "ProjectTasks");

            migrationBuilder.RenameColumn(
                name: "ScheduleDateTime",
                table: "ProjectTasks",
                newName: "ScheduledDateTime");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "ProjectTasks",
                newName: "MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectTasks_CreatedById",
                table: "ProjectTasks",
                newName: "IX_ProjectTasks_MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_AspNetUsers_MemberId",
                table: "ProjectTasks",
                column: "MemberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_Projects_ProjectId",
                table: "ProjectTasks",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
