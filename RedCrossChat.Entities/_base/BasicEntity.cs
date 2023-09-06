using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossChat.Entities
{
    public abstract class BasicEntity
    {
        public abstract bool IsNew();
        public abstract Guid Id { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
