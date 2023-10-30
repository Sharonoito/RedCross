using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossChat.Entities
{
    public class Team : BaseEntity
    {
        public string Name { get; set; }

        public int NotificationType { get; set; } = 1;

        public List<AppUserTeam> AppUserTeams { get; set; }
    }
}








