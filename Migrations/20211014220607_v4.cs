using Microsoft.EntityFrameworkCore.Migrations;

namespace EnergyScanApi.Migrations
{
    public partial class v4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Deposit",
                table: "Cans",
                type: "tinyint(1)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Deposit",
                table: "Cans",
                type: "double",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldNullable: true);
        }
    }
}
