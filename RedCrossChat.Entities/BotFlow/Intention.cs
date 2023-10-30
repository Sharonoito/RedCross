
namespace RedCrossChat.Entities
{
    public  class Intention : BaseEntity
    {
        public string Name { get; set; } 

        public bool IsActive { get; set; } = false;

        public List<SubIntention> SubIntentions { get; set; }
    }
}
