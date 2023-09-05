namespace RedCrossChat.Entities
{
    public class Conversation :BaseEntity
    {

        public string ChannelId { get; set; }

        public string ChannelName { get; set; }

        public List<AiConversation> AiConversation { get; set; }

        public Persona Client {  get; set; }    
    }
}