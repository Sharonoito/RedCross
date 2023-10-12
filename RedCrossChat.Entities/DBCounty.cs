using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossChat.Entities
{
    public class DBCounty : BaseEntity
    {
        public string Name { get; set; }    

        public int Code { get; set; }

        public string prefix { get;set; }

    }
}
