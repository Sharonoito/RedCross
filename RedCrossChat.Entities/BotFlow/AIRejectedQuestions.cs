using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossChat.Entities
{
    public class AIRejectedQuestion : BaseEntity
    {
        public String Questions { get; set; }

        public String Result { get; set; }
    }
}
