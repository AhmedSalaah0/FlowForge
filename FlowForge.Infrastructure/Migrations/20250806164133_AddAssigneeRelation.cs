using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowForge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssigneeRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_AspNetUsers_AssigneeId",
                table: "ProjectTasks");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTasks_AssigneeId",
                table: "ProjectTasks");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_AssigneeId_ProjectId",
                table: "ProjectTasks",
                columns: new[] { "AssigneeId", "ProjectId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_ProjectMembers_AssigneeId_ProjectId",
                table: "ProjectTasks",
                columns: new[] { "AssigneeId", "ProjectId" },
                principalTable: "ProjectMembers",
                principalColumns: new[] { "MemberId", "ProjectId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_ProjectMembers_AssigneeId_ProjectId",
                table: "ProjectTasks");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTasks_AssigneeId_ProjectId",
                table: "ProjectTasks");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_AssigneeId",
                table: "ProjectTasks",
                column: "AssigneeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_AspNetUsers_AssigneeId",
                table: "ProjectTasks",
                column: "AssigneeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
