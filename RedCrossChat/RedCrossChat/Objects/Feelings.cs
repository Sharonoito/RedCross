using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Collections.Generic;

namespace RedCrossChat.Objects
{
    public static class Feelings
    {
        public static readonly string Happy = "Happy 😀";
        public static readonly string Angry = "Angry 😡";
        public static readonly string Anxious = "Anxious 🥴";
        public static readonly string Sad = "Sad 😪";
        public static readonly string FlatEffect = "Flat Effect 🫥";
        public static readonly string Expressionless = "Expressionless 🫤";
    }

    public static class RedCrossDialogTypes
    {
        public static readonly string SelectCounty="Select_County";
    }

    public static class RedCrossLists
    {
        public static readonly List<Choice> choices = new List<Choice>()
        {
            new Choice() { Value = Validations.YES, Synonyms = new List<string> { "y", "Y", "YES", "YE", "ye", "yE", "1" } },
            new Choice() { Value = Validations.NO, Synonyms = new List<string> { "n", "N", "no" } }
        };

        public static readonly List<Choice> AgeGroups = new List<Choice>
        {
            new Choice() { Value ="15-20",Synonyms=new List<string>{"15","16","17","19","20"}},
            new Choice() { Value="20-30",Synonyms=new List<string>{"21","22","23","24","25","26","27","28","29","30"}},
            new Choice() {Value="30-40",Synonyms=new List<string>{"31","32","33","34","35","36","37","38","39","40"}},
            new Choice() {Value="Above 40"},
        };

        public static readonly List<Choice> Actions = new List<Choice>
        {
            new Choice() { Value = InitialActions.Careers, Synonyms = new List<string> { "1", "Careers", "careers" } },
            new Choice() { Value = InitialActions.VolunteerAndMemberShip, Synonyms = new List<string> { "2", "Membership" } },
            new Choice() { Value = InitialActions.VolunteerOpportunities, Synonyms = new List<string> { "3", "Volunteer", "Opportunities" } },
            new Choice() { Value = InitialActions.MentalHealth, Synonyms = new List<string> { "4", "Mental", "mental", "mental Health", "Mental Health", "Help" } }
        };
    }


    public static class Validations
    {
        public const string YES="Yes";
        public const string NO="No";
    }

    public static class Gender
    {
        public const string Male = "Male";
        public const string Female = "Female";
        public const string Other = "Other";
    }

   

    public static class InitialActions
    {
        public static readonly string MentalHealth = "Mental Health";
        public static readonly string Careers = "Careers";
        public static readonly string VolunteerAndMemberShip = "Volunteer and Membership";
        public static readonly string VolunteerOpportunities = "Volunteer Opportunities";
    }
}
