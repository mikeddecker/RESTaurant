using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RESTaurantDL.Migrations
{
    /// <inheritdoc />
    public partial class CustomerLocationRestaurant4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Restaurant");

            migrationBuilder.DropColumn(
                name: "HousenumberLabel",
                table: "Restaurant");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Restaurant");

            migrationBuilder.RenameColumn(
                name: "PostalCode",
                table: "Restaurant",
                newName: "Location");

            migrationBuilder.AlterColumn<int>(
                name: "RestaurantId",
                table: "Restaurant",
                type: "INT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Restaurant",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    LocationId = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostalCode = table.Column<int>(type: "INT", nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    HousenumberLabel = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.LocationId);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    LocationId = table.Column<int>(type: "INT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.CustomerId);
                    table.ForeignKey(
                        name: "FK_Customer_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Location",
                columns: new[] { "LocationId", "City", "HousenumberLabel", "IsDeleted", "PostalCode", "Street" },
                values: new object[] { 1, "MIGRATION", null, false, 1234, null });

            migrationBuilder.CreateIndex(
                name: "IX_Customer_LocationId",
                table: "Customer",
                column: "LocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Restaurant");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Restaurant",
                newName: "PostalCode");

            migrationBuilder.AlterColumn<int>(
                name: "RestaurantId",
                table: "Restaurant",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INT")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Restaurant",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HousenumberLabel",
                table: "Restaurant",
                type: "nvarchar(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Restaurant",
                type: "nvarchar(100)",
                nullable: true);
        }
    }
}
