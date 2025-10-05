using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedRewardUsageUpdateRewardPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RewardPoints_AspNetUsers_ApplicationUserId",
                table: "RewardPoints");

            migrationBuilder.DropIndex(
                name: "IX_RewardPoints_ApplicationUserId",
                table: "RewardPoints");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "RewardPoints");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                table: "RewardPoints");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "RewardPoints",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                table: "RewardPoints",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_RewardPoints_ApplicationUserId",
                table: "RewardPoints",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RewardPoints_AspNetUsers_ApplicationUserId",
                table: "RewardPoints",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
