using System;

namespace RedCrossChat
{
    public class RawConversationVm
    {
        public string Message {  get; set; }

        public Guid ConversationId { get; set; }

        public string Question { get; set; }
    }
}
