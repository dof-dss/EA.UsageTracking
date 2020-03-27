using Microsoft.EntityFrameworkCore.Migrations;

namespace EA.UsageTracking.Infrastructure.Migrations
{
    public partial class AddIndexToUasgeItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UsageItems_TenantId",
                table: "UsageItems",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UsageItems_TenantId",
                table: "UsageItems");
        }
    }
}
