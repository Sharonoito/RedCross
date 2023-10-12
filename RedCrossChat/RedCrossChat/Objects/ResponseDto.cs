using System;

namespace RedCrossChat.Objects
{
    public class ResponseDto
    {
        public string Message { get; set; }

        public string Question { get; set; }

        public string NextQuestion { get; set; }

        public Guid ConversationId { get; set; }
        public DateTime ResponseTimeStamp { get;  set; }
        public DateTime QuestionTimeStamp { get;  set; }
    }
}
