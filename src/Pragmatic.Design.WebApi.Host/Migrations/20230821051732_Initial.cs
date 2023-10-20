using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pragmatic.Design.WebApi.Host.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductCategory",
                columns: table =>
                    new
                    {
                        Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                        Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategory", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "ProductDetails",
                columns: table =>
                    new
                    {
                        Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                        Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                        DetailId = table.Column<int>(type: "int", nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDetails", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "EntityCategories",
                columns: table => new { CategoriesId = table.Column<int>(type: "int", nullable: false), EntitiesId = table.Column<int>(type: "int", nullable: false) },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityCategories", x => new { x.CategoriesId, x.EntitiesId });
                    table.ForeignKey(
                        name: "FK_EntityCategories_ProductCategory_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "ProductCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_EntityCategories_ProductDetails_EntitiesId",
                        column: x => x.EntitiesId,
                        principalTable: "ProductDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table =>
                    new
                    {
                        Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                        OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                        TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                        EntityId = table.Column<int>(type: "int", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Order_ProductDetails_EntityId",
                        column: x => x.EntityId,
                        principalTable: "ProductDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ProductDetail",
                columns: table =>
                    new
                    {
                        Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                        Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                        EntityId = table.Column<int>(type: "int", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductDetail_ProductDetails_EntityId",
                        column: x => x.EntityId,
                        principalTable: "ProductDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(name: "IX_EntityCategories_EntitiesId", table: "EntityCategories", column: "EntitiesId");

            migrationBuilder.CreateIndex(name: "IX_Order_EntityId", table: "Order", column: "EntityId");

            migrationBuilder.CreateIndex(name: "IX_ProductDetail_EntityId", table: "ProductDetail", column: "EntityId", unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "EntityCategories");

            migrationBuilder.DropTable(name: "Order");

            migrationBuilder.DropTable(name: "ProductDetail");

            migrationBuilder.DropTable(name: "ProductCategory");

            migrationBuilder.DropTable(name: "ProductDetails");
        }
    }
}
