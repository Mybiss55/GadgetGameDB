using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GadgetIsLanding.Data.Migrations
{
    public partial class AddPriceAddActiveColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Game",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Cart",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "Cart");
        }
    }
}
