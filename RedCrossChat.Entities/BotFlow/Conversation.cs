using RedCrossChat.Entities._base;

namespace RedCrossChat.Entities
{
    public class Conversation : DefaultEntity
    {
        public string ChannelId { get; set; }

        public string ChannelName { get; set; }

        public string SenderId {  get; set; }

        public string ConversationId {  get; set; }

        public string Reason { get; set; } = "";

        public bool RequestedHandedOver { get; set; } = false;

        public bool HandedOver { get; set; } = false;

        public bool IsReturnClient { get; set; } = false;

        public bool ConversedWithAI { get; set; } = false;

        public bool Language {  get; set; } = true;

        public Guid? FeelingId { get; set; }

        public string FeelingDetail { get; set; } = "";

        public DBFeeling Feeling { get; set; }

        public Persona Persona { get; set; }

        public Guid PersonaId { get; set; }

        public AppUser? AppUser { get; set; }

        public Guid? AppUserId { get; set; }

        public Intention Intention { get; set; }

        public Guid? IntentionId { get; set; }

        public SubIntention SubIntention { get; set; }

        public Guid? SubIntentionId { get; set; }

        public List<AiConversation> AiConversations { get; set; }

        public List<RawConversation> RawConversations { get; set; }

        public List<ChatMessage> ChatMessages { get; set; }

        public string RatingReason { get; set; } = "";

        public Boolean IsActive { get; set; } = true;

       // public Boolean HasUnreadMessages { get; set; } = true;

    }
}