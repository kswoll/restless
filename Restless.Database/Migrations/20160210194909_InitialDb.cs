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
                name: "ApiItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CollectionId = table.Column<int>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    Method = table.Column<int>(nullable: false),
                    RequestBody = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbApiItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbApiItem_DbApiItem_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "ApiItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                        name: "FK_DbApiCall_DbApiItem_ApiId",
                        column: x => x.ApiId,
                        principalTable: "ApiItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                        name: "FK_DbApiHeader_DbApiItem_ApiId",
                        column: x => x.ApiId,
                        principalTable: "ApiItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "ApiInput",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApiId = table.Column<int>(nullable: false),
                    DefaultValue = table.Column<string>(nullable: true),
                    InputType = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbApiInput", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbApiInput_DbApiItem_ApiId",
                        column: x => x.ApiId,
                        principalTable: "ApiItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "ApiOutput",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApiId = table.Column<int>(nullable: false),
                    Expression = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbApiOutput", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbApiOutput_DbApiItem_ApiId",
                        column: x => x.ApiId,
                        principalTable: "ApiItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "ApiResponseVisualizer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApiId = table.Column<int>(nullable: false),
                    ComplicationClass = table.Column<string>(nullable: true),
                    ComplicationData = table.Column<string>(nullable: true),
                    Priority = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbApiResponseComplication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbApiResponseComplication_DbApiItem_ApiId",
                        column: x => x.ApiId,
                        principalTable: "ApiItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                        name: "FK_DbApiCallRequestHeader_DbApiItem_ApiId",
                        column: x => x.ApiId,
                        principalTable: "ApiItem",
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
                        name: "FK_DbApiCallResponseHeader_DbApiItem_ApiId",
                        column: x => x.ApiId,
                        principalTable: "ApiItem",
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
            migrationBuilder.DropTable("ApiInput");
            migrationBuilder.DropTable("ApiOutput");
            migrationBuilder.DropTable("ApiResponseVisualizer");
            migrationBuilder.DropTable("ApiCall");
            migrationBuilder.DropTable("ApiItem");
        }
    }
}
