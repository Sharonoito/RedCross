using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossChat.Entities
{
    public class AgeBand: BaseEntity
    {
        public string Name { get; set; }

        public string Kiswahili { get; set; }

        public string Synonyms { get; set; } = "";

        public int Lowest { get; set; } = 0;

        public int Highest { get; set; }=0;

        public bool IsActive { get; set; }=true;
    }
}
