using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EzioLearning.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateUser_Course : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Courses_CreatedBy",
                schema: "Learning",
                table: "Courses",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AppUsers_CreatedBy",
                schema: "Learning",
                table: "Courses",
                column: "CreatedBy",
                principalSchema: "Auth",
                principalTable: "AppUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AppUsers_CreatedBy",
                schema: "Learning",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_CreatedBy",
                schema: "Learning",
                table: "Courses");
        }
    }
}
