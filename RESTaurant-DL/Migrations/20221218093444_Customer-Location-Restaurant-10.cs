using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RESTaurantDL.Migrations
{
    /// <inheritdoc />
    public partial class CustomerLocationRestaurant10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Restaurant");

            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "Restaurant",
                type: "INT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.InsertData(
                table: "Location",
                columns: new[] { "LocationId", "City", "HousenumberLabel", "IsDeleted", "PostalCode", "Street" },
                values: new object[] { 3, "MIGRATION2", null, false, 1234, null });

            migrationBuilder.CreateIndex(
                name: "IX_Restaurant_LocationId",
                table: "Restaurant",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurant_Location_LocationId",
                table: "Restaurant",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Restaurant_Location_LocationId",
                table: "Restaurant");

            migrationBuilder.DropIndex(
                name: "IX_Restaurant_LocationId",
                table: "Restaurant");

            migrationBuilder.DeleteData(
                table: "Location",
                keyColumn: "LocationId",
                keyValue: 3);

            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "Restaurant",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INT");

            migrationBuilder.AddColumn<int>(
                name: "Location",
                table: "Restaurant",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
