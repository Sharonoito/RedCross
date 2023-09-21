using RedCrossChat.Entities._base;

namespace RedCrossChat.Entities
{
    public class Persona : DefaultEntity
    {
        public string SenderId { get; set; }

        public string Feeling { get; set; } = "";

        public string IsAwareOfFeelings { get; set; } = "";

        public string County { get; set; } = "";

        public string Country { get;set; } = "";

        public bool AcceptedTermsAndConditions { get; set; } = false;

        public string AgeGroup { get; set; } = "";

        public string Gender { get; set; } = "";

        public string ProfessionalStatus { get; set; } = "";

        public string MaritalStatus { get; set; } = "";

        public bool HasTalkedTOSomeone { get; set; } = false;

        public bool WantsToTalkToSomeone { get; set; } = false;

        public bool WantsBreathingExcercises { get; set; } = false;

        public string Reason { get; set; } = "";

        public bool HandedOver { get; set; } = false;


    }
}
