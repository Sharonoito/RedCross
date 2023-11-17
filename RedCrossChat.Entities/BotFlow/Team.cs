

namespace RedCrossChat.Entities
{
    public class Team : BaseEntity
    {
        public string Name { get; set; }

        public int NotificationType { get; set; } = 1;

        public AppUser? AppUser { get; set; }

        public Guid? AppUserId { get; set; }

        public List<AppUserTeam> AppUserTeams { get; set; }
    }
}








