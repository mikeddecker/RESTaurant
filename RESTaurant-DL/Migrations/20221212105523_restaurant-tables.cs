using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RESTaurantDL.Migrations
{
    /// <inheritdoc />
    public partial class restauranttables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TableEF",
                columns: table => new
                {
                    UnusedFakeTableId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestaurantId = table.Column<int>(type: "INT", nullable: false),
                    Tablenumber = table.Column<int>(type: "INT", nullable: false),
                    Seats = table.Column<int>(type: "INT", nullable: false),
                    RestaurantEFRestaurantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableEF", x => x.UnusedFakeTableId);
                    table.ForeignKey(
                        name: "FK_TableEF_Restaurant_RestaurantEFRestaurantId",
                        column: x => x.RestaurantEFRestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "RestaurantId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TableEF_RestaurantEFRestaurantId",
                table: "TableEF",
                column: "RestaurantEFRestaurantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TableEF");
        }
    }
}
