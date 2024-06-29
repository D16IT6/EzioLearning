using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EzioLearning.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_Student_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Confirm",
                schema: "Learning",
                table: "Students",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Confirm",
                schema: "Learning",
                table: "Students");
        }
    }
}
