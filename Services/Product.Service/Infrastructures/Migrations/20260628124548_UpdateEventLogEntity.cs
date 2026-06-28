using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Product.Service.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEventLogEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "event_id",
                table: "event_logs");

            migrationBuilder.RenameColumn(
                name: "event_name",
                table: "event_logs",
                newName: "event_type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "event_type",
                table: "event_logs",
                newName: "event_name");

            migrationBuilder.AddColumn<Guid>(
                name: "event_id",
                table: "event_logs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
