using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EzioLearning.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationShipLecture_VideoLectureDocumentOneOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseLectures_Documents_DocumentId",
                schema: "Learning",
                table: "CourseLectures");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseLectures_Videos_VideoId",
                schema: "Learning",
                table: "CourseLectures");

            migrationBuilder.DropIndex(
                name: "IX_CourseLectures_DocumentId",
                schema: "Learning",
                table: "CourseLectures");

            migrationBuilder.DropIndex(
                name: "IX_CourseLectures_VideoId",
                schema: "Learning",
                table: "CourseLectures");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                schema: "Learning",
                table: "CourseLectures");

            migrationBuilder.DropColumn(
                name: "VideoId",
                schema: "Learning",
                table: "CourseLectures");

            migrationBuilder.AddColumn<Guid>(
                name: "LectureId",
                schema: "Resource",
                table: "Videos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LectureId",
                schema: "Resource",
                table: "Documents",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Videos_LectureId",
                schema: "Resource",
                table: "Videos",
                column: "LectureId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_LectureId",
                schema: "Resource",
                table: "Documents",
                column: "LectureId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_CourseLectures_LectureId",
                schema: "Resource",
                table: "Documents",
                column: "LectureId",
                principalSchema: "Learning",
                principalTable: "CourseLectures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_CourseLectures_LectureId",
                schema: "Resource",
                table: "Videos",
                column: "LectureId",
                principalSchema: "Learning",
                principalTable: "CourseLectures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_CourseLectures_LectureId",
                schema: "Resource",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Videos_CourseLectures_LectureId",
                schema: "Resource",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_Videos_LectureId",
                schema: "Resource",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_Documents_LectureId",
                schema: "Resource",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "LectureId",
                schema: "Resource",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "LectureId",
                schema: "Resource",
                table: "Documents");

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                schema: "Learning",
                table: "CourseLectures",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VideoId",
                schema: "Learning",
                table: "CourseLectures",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseLectures_DocumentId",
                schema: "Learning",
                table: "CourseLectures",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseLectures_VideoId",
                schema: "Learning",
                table: "CourseLectures",
                column: "VideoId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseLectures_Documents_DocumentId",
                schema: "Learning",
                table: "CourseLectures",
                column: "DocumentId",
                principalSchema: "Resource",
                principalTable: "Documents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseLectures_Videos_VideoId",
                schema: "Learning",
                table: "CourseLectures",
                column: "VideoId",
                principalSchema: "Resource",
                principalTable: "Videos",
                principalColumn: "Id");
        }
    }
}
