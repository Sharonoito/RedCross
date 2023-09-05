using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossChat.Entities
{
    [Serializable]
    public class BaseEntity : AbstractEntity
    {
        protected BaseEntity()
        {
            DateCreated = DateTime.Now;
        }

        public override Guid Id { get; set; }

        public override bool IsNew()
        {
            return Id == Guid.Empty;
        }
    }
}
