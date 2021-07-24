using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApiPractice.Persistent.Migrations
{
    public partial class AddRowVersions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RowVersion",
                table: "Notes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RowVersion",
                table: "Customers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Customers");
        }
    }
}
