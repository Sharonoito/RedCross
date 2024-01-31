namespace RedCrossChat.Entities
{
    public class SubChat :BaseEntity
    {  
        public string Message {  get; set; }

        public RawConversation RawConversation { get; set; }

        public Guid RawConversationId { get; set; }
    }
}
