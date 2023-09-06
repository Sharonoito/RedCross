using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossChat.Entities._base
{

    [Serializable]
    public class DefaultEntity : BasicEntity
    {
        protected DefaultEntity()
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
