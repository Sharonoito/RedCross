

namespace RedCrossChat.Entities
{
    public class AuthLoginLog :BaseEntity
    {


        public DateTime LoginTime { get; set; }

        public string IpAddress { get; set; }

        public string UserAgent { get; set; }

        public AppUser? AppUser { get; set; }

        public string? AppUserId { get; set; }

    }
}
