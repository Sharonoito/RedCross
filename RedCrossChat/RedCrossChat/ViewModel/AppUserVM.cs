using RedCrossChat.Entities;

using System.Collections.Generic;

namespace RedCrossChat
{
    public class AppUserVM
    {

        public List<AppUser> Users {  get; set; }

        public List<AppUserTeam> TeamUsers { get; set; }
    }
}
