using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialMedia.Data.Migrations
{
    public partial class ImageColsForDifferentSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Images");

            migrationBuilder.AddColumn<byte[]>(
                name: "MediumImageDate",
                table: "Images",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "OriginalImageData",
                table: "Images",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ThumbnailImageData",
                table: "Images",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediumImageDate",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "OriginalImageData",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "ThumbnailImageData",
                table: "Images");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Images",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
