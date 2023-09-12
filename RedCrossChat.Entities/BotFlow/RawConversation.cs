using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossChat.Entities
{
    public class RawConversation : BaseEntity
    {
        public string? Question {  get; set; }
        public string? Message { get; set; }
        public Conversation? Conversation { get; set; }
        public Guid ConversationId { get; set; }
    }
}
