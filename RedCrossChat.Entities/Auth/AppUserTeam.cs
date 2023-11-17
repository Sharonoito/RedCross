

namespace RedCrossChat.Entities
{
    public class AppUserTeam : BaseEntity
    {
        public AppUser? AppUser { get; set; }

        public Guid UserId { get; set;}

        public Team? Team { get; set; }

        public Guid TeamId { get; set; }
    }
}




