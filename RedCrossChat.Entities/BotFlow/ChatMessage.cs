

namespace RedCrossChat.Entities
{
    public class ChatMessage:BaseEntity
    {
        public string? Message { get; set; }

        public int Type { get; set; } = 0;

        public Boolean IsDisplayed { get; set; } = false;

        public Boolean IsRead { get; set; } = false;

        public Conversation Conversation { get; set; }

        public Guid ConversationId { get; set; }

        public Question? Question { get; set; }

        public Guid? QuestionId { get; set; }
    }
}
