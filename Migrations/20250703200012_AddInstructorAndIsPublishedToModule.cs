using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIPROYECT.Migrations
{
    public partial class AddInstructorAndIsPublishedToModule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InstructorId",
                table: "modules",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "modules",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_modules_InstructorId",
                table: "modules",
                column: "InstructorId");

            migrationBuilder.AddForeignKey(
                name: "FK_modules_instructors_InstructorId",
                table: "modules",
                column: "InstructorId",
                principalTable: "instructors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_modules_instructors_InstructorId",
                table: "modules");

            migrationBuilder.DropIndex(
                name: "IX_modules_InstructorId",
                table: "modules");

            migrationBuilder.DropColumn(
                name: "InstructorId",
                table: "modules");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "modules");
        }
    }
}
