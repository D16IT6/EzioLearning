using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EzioLearning.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update_table_name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseLectures_Attachments_AttachmentId",
                schema: "Learning",
                table: "CourseLectures");

            migrationBuilder.DropTable(
                name: "Attachments",
                schema: "Resource");

            migrationBuilder.RenameColumn(
                name: "AttachmentId",
                schema: "Learning",
                table: "CourseLectures",
                newName: "DocumentId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseLectures_AttachmentId",
                schema: "Learning",
                table: "CourseLectures",
                newName: "IX_CourseLectures_DocumentId");

            migrationBuilder.CreateTable(
                name: "Documents",
                schema: "Resource",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_CourseLectures_Documents_DocumentId",
                schema: "Learning",
                table: "CourseLectures",
                column: "DocumentId",
                principalSchema: "Resource",
                principalTable: "Documents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseLectures_Documents_DocumentId",
                schema: "Learning",
                table: "CourseLectures");

            migrationBuilder.DropTable(
                name: "Documents",
                schema: "Resource");

            migrationBuilder.RenameColumn(
                name: "DocumentId",
                schema: "Learning",
                table: "CourseLectures",
                newName: "AttachmentId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseLectures_DocumentId",
                schema: "Learning",
                table: "CourseLectures",
                newName: "IX_CourseLectures_AttachmentId");

            migrationBuilder.CreateTable(
                name: "Attachments",
                schema: "Resource",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_CourseLectures_Attachments_AttachmentId",
                schema: "Learning",
                table: "CourseLectures",
                column: "AttachmentId",
                principalSchema: "Resource",
                principalTable: "Attachments",
                principalColumn: "Id");
        }
    }
}
