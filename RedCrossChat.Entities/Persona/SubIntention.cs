

namespace RedCrossChat.Entities
{
    public class SubIntention: BaseEntity
    {
        public string Name { get; set; }

        public bool IsActive { get; set; } = true;

        public Intention Intention { get; set; }

        public Guid IntentionId { get; set; }
    }
}
