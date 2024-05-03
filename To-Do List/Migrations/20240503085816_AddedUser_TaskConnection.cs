using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace To_Do_List.Migrations
{
    /// <inheritdoc />
    public partial class AddedUser_TaskConnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Executor",
                table: "MyTasks");

            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "MyTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExecutorId",
                table: "MyTasks",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MyTasks_CreatorId",
                table: "MyTasks",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_MyTasks_ExecutorId",
                table: "MyTasks",
                column: "ExecutorId");

            migrationBuilder.AddForeignKey(
                name: "FK_MyTasks_AspNetUsers_CreatorId",
                table: "MyTasks",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MyTasks_AspNetUsers_ExecutorId",
                table: "MyTasks",
                column: "ExecutorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MyTasks_AspNetUsers_CreatorId",
                table: "MyTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_MyTasks_AspNetUsers_ExecutorId",
                table: "MyTasks");

            migrationBuilder.DropIndex(
                name: "IX_MyTasks_CreatorId",
                table: "MyTasks");

            migrationBuilder.DropIndex(
                name: "IX_MyTasks_ExecutorId",
                table: "MyTasks");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "MyTasks");

            migrationBuilder.DropColumn(
                name: "ExecutorId",
                table: "MyTasks");

            migrationBuilder.AddColumn<string>(
                name: "Executor",
                table: "MyTasks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
