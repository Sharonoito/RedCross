using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossChat.Entities
{
    public class AiConversation :BaseEntity
    {
        public string Question { get; set; }

        public string Response { get; set; }

        public int Iteration { get; set; } = 0;
    }
}
