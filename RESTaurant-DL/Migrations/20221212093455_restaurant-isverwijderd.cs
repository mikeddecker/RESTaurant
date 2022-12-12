using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RESTaurantDL.Migrations
{
    /// <inheritdoc />
    public partial class restaurantisverwijderd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Restaurant",
                columns: table => new
                {
                    RestaurantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Kitchen = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    PostalCode = table.Column<int>(type: "INT", nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    HousenumberLabel = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restaurant", x => x.RestaurantId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Restaurant");
        }
    }
}
