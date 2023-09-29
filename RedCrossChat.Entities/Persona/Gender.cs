using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossChat.Entities
{
    public class Gender: BaseEntity
    {
        public string Name { get; set; }

        public string Kiswahili { get;set; }
        
        public bool IsActive { get; set; } = true;
    }
    
}
