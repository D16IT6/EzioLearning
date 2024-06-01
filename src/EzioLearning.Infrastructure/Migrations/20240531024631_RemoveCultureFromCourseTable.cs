using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EzioLearning.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCultureFromCourseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Cultures_CultureId",
                schema: "Learning",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_CultureId",
                schema: "Learning",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CultureId",
                schema: "Learning",
                table: "Courses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CultureId",
                schema: "Learning",
                table: "Courses",
                type: "varchar(5)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CultureId",
                schema: "Learning",
                table: "Courses",
                column: "CultureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Cultures_CultureId",
                schema: "Learning",
                table: "Courses",
                column: "CultureId",
                principalSchema: "System",
                principalTable: "Cultures",
                principalColumn: "Id");
        }
    }
}
