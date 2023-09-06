using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedCrossChat.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RelationShipForConversationsAndAI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AiConversation_Conversation_ConversationId",
                table: "AiConversation");

            migrationBuilder.AlterColumn<string>(
                name: "Response",
                table: "AiConversation",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Question",
                table: "AiConversation",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "ConversationId",
                table: "AiConversation",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AiConversation_Conversation_ConversationId",
                table: "AiConversation",
                column: "ConversationId",
                principalTable: "Conversation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AiConversation_Conversation_ConversationId",
                table: "AiConversation");

            migrationBuilder.AlterColumn<string>(
                name: "Response",
                table: "AiConversation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Question",
                table: "AiConversation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ConversationId",
                table: "AiConversation",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_AiConversation_Conversation_ConversationId",
                table: "AiConversation",
                column: "ConversationId",
                principalTable: "Conversation",
                principalColumn: "Id");
        }
    }
}
