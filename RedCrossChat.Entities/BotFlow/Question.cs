using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossChat.Entities
{
    public class Question : BaseEntity
    {
        public string Value {  get; set; }

        public int Type { get; set; }

        public int Position { get; set; } = 1;

        public bool IsActive { get; set; } = true;

        public Guid? NextQuestion {  get; set; }
    }
}
