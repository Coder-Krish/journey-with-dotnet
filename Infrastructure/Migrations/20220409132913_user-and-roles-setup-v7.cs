using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class userandrolessetupv7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Employees_EmployeesEmployeeId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "EmployeesEmployeeId",
                table: "AspNetUsers",
                newName: "EmployeesId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_EmployeesEmployeeId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_EmployeesId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Employees_EmployeesId",
                table: "AspNetUsers",
                column: "EmployeesId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Employees_EmployeesId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "EmployeesId",
                table: "AspNetUsers",
                newName: "EmployeesEmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_EmployeesId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_EmployeesEmployeeId");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Employees_EmployeesEmployeeId",
                table: "AspNetUsers",
                column: "EmployeesEmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
