

namespace RedCrossChat.Entities
{
    public class RawConversation : BaseEntity
    {
        public string? Question {  get; set; }

        public Question? Question1 { get; set; }

        public Guid? Question1ID { get; set; }
        
        public string? Message { get; set; }

        public int Type { get; set; } = 0; //
        
        public Conversation? Conversation { get; set; }

        public bool IsReply { get; set; }=false;

        public bool IsHandOverMessage { get; set; }=false;

        public bool HasReply { get; set; } =false;

        public Guid ConversationId { get; set; }

        public DateTime ResponseTimeStamp { get; set; }

        public DateTime QuestionTimeStamp { get; set; }

        public AppUser? AppUser { get; set; }

        public Guid? AppUserId { get; set; }
    }
}
