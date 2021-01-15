using Microsoft.EntityFrameworkCore.Migrations;

namespace VAIISemka.Data.Migrations
{
    public partial class UpravaObrazkov : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Posts_PostId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Images_ThumbnailImageId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_ThumbnailImageId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Images_PostId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "ThumbnailImageId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Images");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailImage",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailImage",
                table: "Posts");

            migrationBuilder.AddColumn<int>(
                name: "ThumbnailImageId",
                table: "Posts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PostId",
                table: "Images",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ThumbnailImageId",
                table: "Posts",
                column: "ThumbnailImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_PostId",
                table: "Images",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Posts_PostId",
                table: "Images",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Images_ThumbnailImageId",
                table: "Posts",
                column: "ThumbnailImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
