using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeAppUserNullableInRewardPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RewardPoints_AspNetUsers_ApplicationUserId",
                table: "RewardPoints");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "RewardPoints",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_RewardPoints_AspNetUsers_ApplicationUserId",
                table: "RewardPoints",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RewardPoints_AspNetUsers_ApplicationUserId",
                table: "RewardPoints");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "RewardPoints",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RewardPoints_AspNetUsers_ApplicationUserId",
                table: "RewardPoints",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
