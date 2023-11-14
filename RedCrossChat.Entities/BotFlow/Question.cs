namespace RedCrossChat.Entities
{
    public class Question : BaseEntity
    {
        public string question {  get; set; }
        public int Code {  get; set; }

        public string Kiswahili { get; set; }

        public int Type { get; set; }

        public int Position { get; set; } = 1;

        public bool IsActive { get; set; } = true;

        public Guid? NextQuestion {  get; set; }
    }
}
