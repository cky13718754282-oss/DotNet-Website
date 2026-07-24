using System;
using Geekspace.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Geekspace.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260724223000_AddUserLearningProgress")]
    public partial class AddUserLearningProgress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserLearningProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LearningResourceId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsSaved = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLearningProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLearningProgresses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLearningProgresses_LearningResources_LearningResourceId",
                        column: x => x.LearningResourceId,
                        principalTable: "LearningResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLearningProgresses_LearningResourceId",
                table: "UserLearningProgresses",
                column: "LearningResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLearningProgresses_UserId_LearningResourceId",
                table: "UserLearningProgresses",
                columns: new[] { "UserId", "LearningResourceId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "UserLearningProgresses");
        }
    }
}
