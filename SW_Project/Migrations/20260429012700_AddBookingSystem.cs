using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SW_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // تعديل عمود RenterId ليكون nvarchar(450) و NOT NULL
            migrationBuilder.AlterColumn<string>(
                name: "RenterId",
                table: "Bookings",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // إنشاء فهرس على RenterId
            migrationBuilder.CreateIndex(
                name: "IX_Bookings_RenterId",
                table: "Bookings",
                column: "RenterId");

            // ✅ إضافة المفتاح الأجنبي مع ON DELETE NO ACTION بدلاً من CASCADE
            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_AspNetUsers_RenterId",
                table: "Bookings",
                column: "RenterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);   // ✅ تم التعديل هنا
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // حذف المفتاح الأجنبي
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_AspNetUsers_RenterId",
                table: "Bookings");

            // حذف الفهرس
            migrationBuilder.DropIndex(
                name: "IX_Bookings_RenterId",
                table: "Bookings");

            // استعادة العمود إلى nvarchar(max)
            migrationBuilder.AlterColumn<string>(
                name: "RenterId",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}