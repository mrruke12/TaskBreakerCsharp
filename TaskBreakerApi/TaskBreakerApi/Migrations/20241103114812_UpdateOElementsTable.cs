using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskBreakerApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOElementsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "OElements",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "OElements");
        }
    }
}
