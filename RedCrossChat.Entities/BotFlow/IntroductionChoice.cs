


namespace RedCrossChat.Entities
{
    public class IntroductionChoice : BaseEntity
    {
        public string Name { get; set; } = "";
        public string Kiswahili { get; set; } = "";

        public int ActionType { get; set; } = 1;

        public List<InitialActionItem>? InitialActionItems { get; set; }


    }
}















