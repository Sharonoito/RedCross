using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossChat.Entities
{
    public class AppClaim
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int AppModuleId { get; set; }

        public AppModule AppModule { get; set; }
    }
}
