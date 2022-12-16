using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RESTaurantDL.Migrations
{
    /// <inheritdoc />
    public partial class restauranttablename2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Table_Restaurant_RestaurantEFRestaurantId",
                table: "Table");

            migrationBuilder.DropIndex(
                name: "IX_Table_RestaurantEFRestaurantId",
                table: "Table");

            migrationBuilder.DropColumn(
                name: "RestaurantEFRestaurantId",
                table: "Table");

            migrationBuilder.CreateIndex(
                name: "IX_Table_RestaurantId",
                table: "Table",
                column: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Table_Restaurant_RestaurantId",
                table: "Table",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "RestaurantId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Table_Restaurant_RestaurantId",
                table: "Table");

            migrationBuilder.DropIndex(
                name: "IX_Table_RestaurantId",
                table: "Table");

            migrationBuilder.AddColumn<int>(
                name: "RestaurantEFRestaurantId",
                table: "Table",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Table_RestaurantEFRestaurantId",
                table: "Table",
                column: "RestaurantEFRestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Table_Restaurant_RestaurantEFRestaurantId",
                table: "Table",
                column: "RestaurantEFRestaurantId",
                principalTable: "Restaurant",
                principalColumn: "RestaurantId");
        }
    }
}
