using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRewardValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "RewardPoints");

            migrationBuilder.CreateTable(
                name: "RewardUsage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RewardPointsId = table.Column<int>(type: "int", nullable: false),
                    GrantedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notified = table.Column<bool>(type: "bit", nullable: false),
                    Redeemed = table.Column<bool>(type: "bit", nullable: false),
                    RedeemedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardUsage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RewardUsage_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RewardUsage_RewardPoints_RewardPointsId",
                        column: x => x.RewardPointsId,
                        principalTable: "RewardPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RewardUsage_ApplicationUserId",
                table: "RewardUsage",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RewardUsage_RewardPointsId",
                table: "RewardUsage",
                column: "RewardPointsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RewardUsage");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "RewardPoints",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
