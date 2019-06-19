using Microsoft.EntityFrameworkCore.Migrations;

namespace lab2_restapi_1205_taskmgmt.Migrations
{
    public partial class added_owner_and_role : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "History",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Comments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_OwnerId",
                table: "Tasks",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_OwnerId",
                table: "Comments",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_OwnerId",
                table: "Comments",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_OwnerId",
                table: "Tasks",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Users_OwnerId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_OwnerId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_OwnerId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Comments_OwnerId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "History",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Comments");
        }
    }
}
