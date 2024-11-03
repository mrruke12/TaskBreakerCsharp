using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskBreakerApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTablesFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "TaskId",
                table: "OElements",
                newName: "ObjectiveId");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Objectives",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Objectives");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ObjectiveId",
                table: "OElements",
                newName: "TaskId");
        }
    }
}
