

namespace RedCrossChat.Entities
{
    public class RawConversation : BaseEntity
    {
        public string? Question {  get; set; }
        
        public string? Message { get; set; }
        
        public Conversation? Conversation { get; set; }

        public bool IsReply { get; set; }=false;

        public bool IsHandOverMessage { get; set; }=false;

        public bool HasReply { get; set; } =false;

        public Guid ConversationId { get; set; }

        public DateTime ResponseTimeStamp { get; set; }

        public DateTime QuestionTimeStamp { get; set; }
    }
}
