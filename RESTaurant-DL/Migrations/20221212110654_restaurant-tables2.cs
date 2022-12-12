using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RESTaurantDL.Migrations
{
    /// <inheritdoc />
    public partial class restauranttables2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TableEF_Restaurant_RestaurantEFRestaurantId",
                table: "TableEF");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TableEF",
                table: "TableEF");

            migrationBuilder.RenameTable(
                name: "TableEF",
                newName: "Table");

            migrationBuilder.RenameIndex(
                name: "IX_TableEF_RestaurantEFRestaurantId",
                table: "Table",
                newName: "IX_Table_RestaurantEFRestaurantId");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Table",
                type: "BIT",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Table",
                table: "Table",
                column: "UnusedFakeTableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Table_Restaurant_RestaurantEFRestaurantId",
                table: "Table",
                column: "RestaurantEFRestaurantId",
                principalTable: "Restaurant",
                principalColumn: "RestaurantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Table_Restaurant_RestaurantEFRestaurantId",
                table: "Table");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Table",
                table: "Table");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Table");

            migrationBuilder.RenameTable(
                name: "Table",
                newName: "TableEF");

            migrationBuilder.RenameIndex(
                name: "IX_Table_RestaurantEFRestaurantId",
                table: "TableEF",
                newName: "IX_TableEF_RestaurantEFRestaurantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TableEF",
                table: "TableEF",
                column: "UnusedFakeTableId");

            migrationBuilder.AddForeignKey(
                name: "FK_TableEF_Restaurant_RestaurantEFRestaurantId",
                table: "TableEF",
                column: "RestaurantEFRestaurantId",
                principalTable: "Restaurant",
                principalColumn: "RestaurantId");
        }
    }
}
