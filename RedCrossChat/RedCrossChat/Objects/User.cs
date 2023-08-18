namespace RedCrossChat.Objects
{
    public class User
    {
        private string selectedFeeling;

        public int Iteration = 1;
        public bool isAwareOfFeeling { get; set; } = false;
        public bool hasTalkedToSomeone { get; set; } = false;
        public bool wantsToTalkToSomeone { get; set; } = false;
        public bool wantstoTalkToAProfessional { get; set; } = false;
        public bool handOverToUser { get; set; } = false;
        public string SelectedFeeling { get => selectedFeeling; set => selectedFeeling=value; }

    }
}
