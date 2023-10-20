using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quickfire_Bulletin.Migrations
{
    public partial class UpdateCommentDbSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Articles_NewsArticleArticleId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_NewsArticle_ArticleId",
                table: "Comment");

            migrationBuilder.DropTable(
                name: "NewsArticle");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_NewsArticleArticleId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "NewsArticleArticleId",
                table: "Comment");

            migrationBuilder.RenameTable(
                name: "Comment",
                newName: "Comments");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_ArticleId",
                table: "Comments",
                newName: "IX_Comments_ArticleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Articles_ArticleId",
                table: "Comments",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "ArticleId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Articles_ArticleId",
                table: "Comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "Comment");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_ArticleId",
                table: "Comment",
                newName: "IX_Comment_ArticleId");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Comment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NewsArticleArticleId",
                table: "Comment",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "NewsArticle",
                columns: table => new
                {
                    ArticleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PubDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SourceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourcePriority = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsArticle", x => x.ArticleId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_NewsArticleArticleId",
                table: "Comment",
                column: "NewsArticleArticleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Articles_NewsArticleArticleId",
                table: "Comment",
                column: "NewsArticleArticleId",
                principalTable: "Articles",
                principalColumn: "ArticleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_NewsArticle_ArticleId",
                table: "Comment",
                column: "ArticleId",
                principalTable: "NewsArticle",
                principalColumn: "ArticleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
