﻿using RedCrossChat.Entities._base;

namespace RedCrossChat.Entities
{
    public class Persona : DefaultEntity
    {
        public string SenderId { get; set; }

        public string? CodeName {  get; set; }
        public string? Name { get; set; }
        public string? FromId { get; set; }

        public Guid? FeelingId { get; set; }
        public DBFeeling Feeling { get; set; }

        public Guid? CountyId { get; set; }
        public DBCounty County { get; set; }

        public Guid? AgeBandId { get; set; }
        public AgeBand AgeBand { get; set; }

        public Guid? GenderId { get; set; }
        public Gender Gender { get; set; }

        public Guid? ProfessionId { get; set; }
        public Profession Profession { get; set; }

        public Guid? MaritalStateId { get; set; }
        public MaritalState MaritalState { get; set; }

        public string? ChatID { get; set; }

        public string IsAwareOfFeelings { get; set; } = "";
        
        public string Country { get; set; } = "";

        public bool AcceptedTermsAndConditions { get; set; } = false;

        public string ProfessionalStatus { get; set; } = "";

        public string MaritalStatus { get; set; } = "";

        public bool HasTalkedTOSomeone { get; set; } = false;

        public bool WantsToTalkToSomeone { get; set; } = false;

        public bool WantsBreathingExcercises { get; set; } = false;

    }
}
