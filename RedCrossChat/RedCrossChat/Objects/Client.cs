namespace RedCrossChat.Objects
{
    public class Client
    {
        public string Feeling { get; set; } = "";
        public string AgeGroup { get; set; } = "";
        public string Country { get; set; } = "";
        public string County { get; set; } = "";
        public string Gender { get; set; } = "";
        public string RelationShipStatus { get; set; } = "";
        public string professionlStatus { get; set; }="";
        public string User { get; set; } = null;
        public bool language { get; set; } = true;    
        public bool DialogClosed { get; set; } = false;
    }
}
