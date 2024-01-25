using Microsoft.Bot.Connector;
using RedCrossChat.Entities;
using System.Collections.Generic;

namespace RedCrossChat
{
    public class ChatResponseVm
    {
        public List<Conversation> myConversations;

        public List<HandOverRequest> handOverRequests;

        public List<HandOverRequest> myHandOverRequests;
    }
}
