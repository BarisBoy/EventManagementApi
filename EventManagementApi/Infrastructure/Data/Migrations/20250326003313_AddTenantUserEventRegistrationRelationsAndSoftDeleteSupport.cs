using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantUserEventRegistrationRelationsAndSoftDeleteSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Tenants_TenantId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Events_EventId",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Tenants_TenantId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "UpdateUser",
                table: "Users",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "Users",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreateUser",
                table: "Users",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdateUser",
                table: "Tenants",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "Tenants",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreateUser",
                table: "Tenants",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "Tenants",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdateUser",
                table: "Registrations",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "Registrations",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "ParticipantName",
                table: "Registrations",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "ParticipantEmail",
                table: "Registrations",
                newName: "AttendeeName");

            migrationBuilder.RenameColumn(
                name: "CreateUser",
                table: "Registrations",
                newName: "AttendeeEmail");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "Registrations",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdateUser",
                table: "Events",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "Events",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreateUser",
                table: "Events",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "Events",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Tenants",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "Domain",
                table: "Tenants",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Registrations",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Registrations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Domain",
                table: "Tenants",
                column: "Domain",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_UserId",
                table: "Registrations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Tenants_TenantId",
                table: "Events",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Events_EventId",
                table: "Registrations",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Users_UserId",
                table: "Registrations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Tenants_TenantId",
                table: "Users",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Tenants_TenantId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Events_EventId",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Users_UserId",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Tenants_TenantId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_Domain",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Registrations_UserId",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "Domain",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Registrations");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Users",
                newName: "UpdateUser");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Users",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Users",
                newName: "CreateUser");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Users",
                newName: "CreateDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Tenants",
                newName: "UpdateUser");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Tenants",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Tenants",
                newName: "CreateUser");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Tenants",
                newName: "CreateDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Registrations",
                newName: "UpdateUser");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Registrations",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Registrations",
                newName: "ParticipantName");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Registrations",
                newName: "CreateDate");

            migrationBuilder.RenameColumn(
                name: "AttendeeName",
                table: "Registrations",
                newName: "ParticipantEmail");

            migrationBuilder.RenameColumn(
                name: "AttendeeEmail",
                table: "Registrations",
                newName: "CreateUser");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Events",
                newName: "UpdateUser");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Events",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Events",
                newName: "CreateUser");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Events",
                newName: "CreateDate");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Users",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Tenants",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Registrations",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Events",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Tenants_TenantId",
                table: "Events",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Events_EventId",
                table: "Registrations",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Tenants_TenantId",
                table: "Users",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
