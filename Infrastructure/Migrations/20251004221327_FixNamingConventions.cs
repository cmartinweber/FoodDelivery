using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixNamingConventions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RewardUsage_RewardPoints_RewardPointsId",
                table: "RewardUsage");

            migrationBuilder.DropTable(
                name: "RewardPoints");

            migrationBuilder.CreateTable(
                name: "RewardPoint",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ThresholdPoints = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PromoCodeId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardPoint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RewardPoint_PromoCode_PromoCodeId",
                        column: x => x.PromoCodeId,
                        principalTable: "PromoCode",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RewardPoint_PromoCodeId",
                table: "RewardPoint",
                column: "PromoCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RewardUsage_RewardPoint_RewardPointsId",
                table: "RewardUsage",
                column: "RewardPointsId",
                principalTable: "RewardPoint",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RewardUsage_RewardPoint_RewardPointsId",
                table: "RewardUsage");

            migrationBuilder.DropTable(
                name: "RewardPoint");

            migrationBuilder.CreateTable(
                name: "RewardPoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromoCodeId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ThresholdPoints = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RewardPoints_PromoCode_PromoCodeId",
                        column: x => x.PromoCodeId,
                        principalTable: "PromoCode",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RewardPoints_PromoCodeId",
                table: "RewardPoints",
                column: "PromoCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RewardUsage_RewardPoints_RewardPointsId",
                table: "RewardUsage",
                column: "RewardPointsId",
                principalTable: "RewardPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
