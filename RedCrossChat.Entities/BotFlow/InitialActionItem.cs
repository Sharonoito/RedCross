using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossChat.Entities
{
    public class InitialActionItem
    {
        public string ActionMessage { get; set; }

        public string Value { get; set; }

        public string SubTitle { get; set; }

        public int Language  { get; set; }

        public Guid IntroductionChoiceId { get; set; }

        public IntroductionChoice IntroductionChoice { get; set;}
    }
}


