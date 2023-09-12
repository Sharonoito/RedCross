using RedCrossChat.Entities._base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossChat.Entities
{
    public class AiConversation : DefaultEntity
    {
        public string? Question { get; set; }

        public string? Response { get; set; }

        public int Iteration { get; set; } = 0;

        public Conversation? Conversation { get; set; }
        public Guid ConversationId { get; set; }
    }
}
