using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseBack.Migrations
{
    public partial class Addcategorycolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "price",
                table: "items",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "Нет цены",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<string>(
                name: "category",
                table: "items",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "category",
                table: "items");

            migrationBuilder.AlterColumn<string>(
                name: "price",
                table: "items",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldDefaultValue: "Нет цены");
        }
    }
}
