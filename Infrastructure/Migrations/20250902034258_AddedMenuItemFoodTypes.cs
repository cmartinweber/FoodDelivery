using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedMenuItemFoodTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItem_FoodType_FoodTypeID",
                table: "MenuItem");

            migrationBuilder.DropIndex(
                name: "IX_MenuItem_FoodTypeID",
                table: "MenuItem");

            migrationBuilder.DropColumn(
                name: "FoodTypeID",
                table: "MenuItem");

            migrationBuilder.CreateTable(
                name: "MenuItemFoodType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuItemId = table.Column<int>(type: "int", nullable: false),
                    FoodTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItemFoodType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItemFoodType_FoodType_FoodTypeId",
                        column: x => x.FoodTypeId,
                        principalTable: "FoodType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuItemFoodType_MenuItem_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuItemFoodType_FoodTypeId",
                table: "MenuItemFoodType",
                column: "FoodTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItemFoodType_MenuItemId",
                table: "MenuItemFoodType",
                column: "MenuItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuItemFoodType");

            migrationBuilder.AddColumn<int>(
                name: "FoodTypeID",
                table: "MenuItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MenuItem_FoodTypeID",
                table: "MenuItem",
                column: "FoodTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItem_FoodType_FoodTypeID",
                table: "MenuItem",
                column: "FoodTypeID",
                principalTable: "FoodType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
