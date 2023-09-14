using RedCrossChat.Entities._base;

namespace RedCrossChat.Entities
{
    public class Conversation : DefaultEntity
    {
        public string ChannelId { get; set; }
        public string ChannelName { get; set; }
        public string SenderId {  get; set; }
        public string ConversationId {  get; set; }
        public List<AiConversation> AiConversations { get; set; }
        public Persona Client {  get; set; }    
    }
}