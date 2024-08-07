﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EzioLearning.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_CourseLectures_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LectureType",
                schema: "Learning",
                table: "CourseLectures",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LectureType",
                schema: "Learning",
                table: "CourseLectures");
        }
    }
}
