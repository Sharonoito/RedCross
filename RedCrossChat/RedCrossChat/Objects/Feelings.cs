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
        public static readonly string Other = "Other";
    }

    public static class FeelingsSwahili
    {
        public static readonly string Furaha = " Unahisi kuwa na Furaha 😀";
        public static readonly string Hasira = "Unahisi kuwa na Hasira 😡";
        public static readonly string Wasiwasi = "Unahisi kuwa na Wasiwasi 🥴";
        public static readonly string Huzuni = "Unahisi kuwa na Huzuni 😪";
        public static readonly string Kuchanganyikiwa = "Unahisi kuwa na Kuchanganyikiwa 🫥";
        //public static readonly string Expressionless = "Expressionless 🫤";
        public static readonly string Zinginezo = "Zinginezo";
    }


    public static class RedCrossDialogTypes
    {
        public static readonly string SelectCounty="Select_County";
    }

    public static class RedCrossLists
    {
        public static readonly List<Choice> choices = new()
        {
            new Choice() { Value = Validations.YES, Synonyms = new List<string> { "y", "Y", "YES", "YE", "ye", "yE", "1" } },
            new Choice() { Value = Validations.NO, Synonyms = new List<string> { "n", "N", "no" } }
        };

        public static readonly List<Choice> Ratings = new()
        {
             new Choice() { Value = "Excellent", Synonyms = new List<string> {  "1" } },
             new Choice() { Value = "Average", Synonyms = new List<string> {  "3" } },
             new Choice() { Value = "Good", Synonyms = new List<string> { "2" } },
             new Choice() { Value = "Bad", Synonyms = new List<string> {  "4" } },
             new Choice() { Value = "Terrible", Synonyms = new List<string> {  "5" } },
        };

        public static readonly List<Choice> AgeGroups = new()
        {
            new Choice() { Value ="15-20",Synonyms=new List<string>{"15","16","17","19","20"}},
            new Choice() { Value="20-30",Synonyms=new List<string>{"21","22","23","24","25","26","27","28","29","30"}},
            new Choice() {Value="30-40",Synonyms=new List<string>{"31","32","33","34","35","36","37","38","39","40"}},
            new Choice() {Value="Above 40"},
        };

        public static readonly List<Choice> Actions = new()
        {
            new Choice() { Value = InitialActions.Careers, Synonyms = new List<string> { "1", "Careers", "careers" } },
            new Choice() { Value = InitialActions.VolunteerAndMemberShip, Synonyms = new List<string> { "2", "Membership" } },
            new Choice() { Value = InitialActions.VolunteerOpportunities, Synonyms = new List<string> { "3", "Volunteer", "Opportunities" } },
            new Choice() { Value = InitialActions.MentalHealth, Synonyms = new List<string> { "4", "Mental", "mental", "mental Health", "Mental Health", "Help" } }
        };


        public static readonly List<Choice> ActionKiswahili = new()
        {
            new Choice() { Value = InitialActionsKiswahili.Careers, Synonyms = new List<string> { "1", "Careers", "careers" } },
            new Choice() { Value = InitialActionsKiswahili.VolunteerAndMemberShip, Synonyms = new List<string> { "2", "Membership" } },
            new Choice() { Value = InitialActionsKiswahili.VolunteerOpportunities, Synonyms = new List<string> { "3", "Volunteer", "Opportunities" } },
            new Choice() { Value = InitialActionsKiswahili.MentalHealth, Synonyms = new List<string> { "4", "Mental", "mental", "mental Health", "Mental Health", "Help" } }
        };


 
    }

    public static class Validations
    {
        public const string YES = "Yes";
        public const string NO = "No";
    }

    public static class ValidationsSwahili
    {
        public const string NDIO="Ndio";
        public const string La="la";
    }

    public static class Gender
    {
        public const string Male = "Male";
        public const string Female = "Female";
        public const string Other = "Other";
    }
    public static class GenderSwahili
    {
        public const string Mume = "Mume";
        public const string Mke = "Mke";
        public const string Haijatajwa = "Haijatajwa";
    }

    public static class DialogConstants
    {
        public const string ProfileAssesor="PROFILE_001";
    }


    public static class LanguageOptions
    {
        public const string English = "English";
        public const string Kiswahili = "Kiswahili ";
 
    }


    public static class InitialActions
    {
        public static readonly string MentalHealth = "Mental Health";
        public static readonly string Careers = "Careers";
        public static readonly string VolunteerAndMemberShip = "Volunteer and Membership";
        public static readonly string VolunteerOpportunities = "Volunteer Opportunities";
    }

    public static class InitialActionsKiswahili
    {
        public static readonly string MentalHealth = "Afya ya kiakili";
        public static readonly string Careers = "Taaluma";
        public static readonly string VolunteerAndMemberShip = "Kujitolea na kuwa mshirika";
        public static readonly string VolunteerOpportunities = "Nafasi ya kujitolea";
    }

}
