namespace RedCrossChat.Entities
{
    public class QuestionOption:BaseEntity
    {
        public string Value { get; set; } = "";

        public int Action { get; set; }

        public Question? Question {  get; set; }

        public Guid QuestionId { get; set; }
    }
}
