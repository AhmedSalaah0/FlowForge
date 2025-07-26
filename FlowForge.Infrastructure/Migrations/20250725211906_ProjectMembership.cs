using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoList.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProjectMembership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProjectRole",
                table: "ProjectMembers",
                newName: "MembershipStatus");

            migrationBuilder.AddColumn<int>(
                name: "MemberRole",
                table: "ProjectMembers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemberRole",
                table: "ProjectMembers");

            migrationBuilder.RenameColumn(
                name: "MembershipStatus",
                table: "ProjectMembers",
                newName: "ProjectRole");
        }
    }
}
