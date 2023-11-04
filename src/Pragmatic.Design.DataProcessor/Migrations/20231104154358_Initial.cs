using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pragmatic.Design.DataProcessor.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dataProcessor");

            migrationBuilder.CreateTable(
                name: "TaskExecution",
                schema: "dataProcessor",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    TaskType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    DataProcessorJobStartTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    State = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EndedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskExecution", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskExecution_DataProcessorJobStartTime_Name_TaskType",
                schema: "dataProcessor",
                table: "TaskExecution",
                columns: new[] { "DataProcessorJobStartTime", "Name", "TaskType" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskExecution_State_TaskType",
                schema: "dataProcessor",
                table: "TaskExecution",
                columns: new[] { "State", "TaskType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskExecution",
                schema: "dataProcessor");
        }
    }
}
