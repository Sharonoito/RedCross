using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossChat.Entities
{
    [Serializable]
    public abstract class AbstractEntity: BasicEntity
    {
        
        public string CreatedById { get; set; }

        public DateTime? DateUpdated { get; set; }
        public string? UpdatedById { get; set; }

        public DateTime? DateDeleted { get; set; }
        public string? DeletedById { get; set; }

    }
}
