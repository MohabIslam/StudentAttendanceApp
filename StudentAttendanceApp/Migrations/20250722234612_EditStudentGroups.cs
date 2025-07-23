using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentAttendanceApp.Migrations
{
    /// <inheritdoc />
    public partial class EditStudentGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentGroup_Tracks_TrackId",
                table: "StudentGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentGroup",
                table: "StudentGroup");

            migrationBuilder.RenameTable(
                name: "StudentGroup",
                newName: "StudentGroups");

            migrationBuilder.RenameIndex(
                name: "IX_StudentGroup_TrackId",
                table: "StudentGroups",
                newName: "IX_StudentGroups_TrackId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentGroups",
                table: "StudentGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentGroups_Tracks_TrackId",
                table: "StudentGroups",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentGroups_Tracks_TrackId",
                table: "StudentGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentGroups",
                table: "StudentGroups");

            migrationBuilder.RenameTable(
                name: "StudentGroups",
                newName: "StudentGroup");

            migrationBuilder.RenameIndex(
                name: "IX_StudentGroups_TrackId",
                table: "StudentGroup",
                newName: "IX_StudentGroup_TrackId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentGroup",
                table: "StudentGroup",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentGroup_Tracks_TrackId",
                table: "StudentGroup",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
