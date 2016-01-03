using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace Restless.Database.Migrations
{
    public partial class InitialDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Api",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Body = table.Column<byte[]>(nullable: true),
                    HttpMethod = table.Column<string>(nullable: false),
                    ResponseVisualizer = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbApi", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "ApiCall",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApiId = table.Column<int>(nullable: false),
                    Body = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbApiCall", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbApiCall_DbApi_ApiId",
                        column: x => x.ApiId,
                        principalTable: "Api",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "ApiHeader",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApiId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbApiHeader", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbApiHeader_DbApi_ApiId",
                        column: x => x.ApiId,
                        principalTable: "Api",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "ApiCallRequestHeader",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApiCallId = table.Column<int>(nullable: false),
                    ApiId = table.Column<int>(nullable: true),
                    DbApiCallId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbApiCallRequestHeader", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbApiCallRequestHeader_DbApi_ApiId",
                        column: x => x.ApiId,
                        principalTable: "Api",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DbApiCallRequestHeader_DbApiCall_DbApiCallId",
                        column: x => x.DbApiCallId,
                        principalTable: "ApiCall",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "ApiCallResponseHeader",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApiCallId = table.Column<int>(nullable: false),
                    ApiId = table.Column<int>(nullable: true),
                    DbApiCallId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbApiCallResponseHeader", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbApiCallResponseHeader_DbApi_ApiId",
                        column: x => x.ApiId,
                        principalTable: "Api",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DbApiCallResponseHeader_DbApiCall_DbApiCallId",
                        column: x => x.DbApiCallId,
                        principalTable: "ApiCall",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("ApiCallRequestHeader");
            migrationBuilder.DropTable("ApiCallResponseHeader");
            migrationBuilder.DropTable("ApiHeader");
            migrationBuilder.DropTable("ApiCall");
            migrationBuilder.DropTable("Api");
        }
    }
}
