using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EA.UsageTracking.Infrastructure.Migrations
{
    public partial class AddIDsToUsageItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsageItems_ApplicationEvents_ApplicationEventId",
                table: "UsageItems");

            migrationBuilder.DropForeignKey(
                name: "FK_UsageItems_Applications_ApplicationId",
                table: "UsageItems");

            migrationBuilder.DropForeignKey(
                name: "FK_UsageItems_ApplicationUsers_ApplicationUserId",
                table: "UsageItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "ApplicationUserId",
                table: "UsageItems",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationId",
                table: "UsageItems",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationEventId",
                table: "UsageItems",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UsageItems_ApplicationEvents_ApplicationEventId",
                table: "UsageItems",
                column: "ApplicationEventId",
                principalTable: "ApplicationEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsageItems_Applications_ApplicationId",
                table: "UsageItems",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsageItems_ApplicationUsers_ApplicationUserId",
                table: "UsageItems",
                column: "ApplicationUserId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsageItems_ApplicationEvents_ApplicationEventId",
                table: "UsageItems");

            migrationBuilder.DropForeignKey(
                name: "FK_UsageItems_Applications_ApplicationId",
                table: "UsageItems");

            migrationBuilder.DropForeignKey(
                name: "FK_UsageItems_ApplicationUsers_ApplicationUserId",
                table: "UsageItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "ApplicationUserId",
                table: "UsageItems",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationId",
                table: "UsageItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationEventId",
                table: "UsageItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_UsageItems_ApplicationEvents_ApplicationEventId",
                table: "UsageItems",
                column: "ApplicationEventId",
                principalTable: "ApplicationEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsageItems_Applications_ApplicationId",
                table: "UsageItems",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsageItems_ApplicationUsers_ApplicationUserId",
                table: "UsageItems",
                column: "ApplicationUserId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
