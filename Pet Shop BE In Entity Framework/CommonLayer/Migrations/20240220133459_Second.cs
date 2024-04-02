using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLayer.Migrations
{
    public partial class Second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CardNo",
                table: "CardDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPayment",
                table: "CardDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PaymentType",
                table: "CardDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "CardDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UpiId",
                table: "CardDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardNo",
                table: "CardDetails");

            migrationBuilder.DropColumn(
                name: "IsPayment",
                table: "CardDetails");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "CardDetails");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "CardDetails");

            migrationBuilder.DropColumn(
                name: "UpiId",
                table: "CardDetails");
        }
    }
}
