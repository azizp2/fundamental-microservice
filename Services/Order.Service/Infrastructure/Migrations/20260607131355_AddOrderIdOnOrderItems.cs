using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Service.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderIdOnOrderItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "order_date",
                table: "orders",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<decimal>(
                name: "discount",
                table: "order_items",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<Guid>(
                name: "order_id",
                table: "order_items",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "orders_id",
                table: "order_items",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_order_items_orders_id",
                table: "order_items",
                column: "orders_id");

            migrationBuilder.AddForeignKey(
                name: "fk_order_items_orders_orders_id",
                table: "order_items",
                column: "orders_id",
                principalTable: "orders",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_order_items_orders_orders_id",
                table: "order_items");

            migrationBuilder.DropIndex(
                name: "ix_order_items_orders_id",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "order_id",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "orders_id",
                table: "order_items");

            migrationBuilder.AlterColumn<DateTime>(
                name: "order_date",
                table: "orders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "discount",
                table: "order_items",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);
        }
    }
}
