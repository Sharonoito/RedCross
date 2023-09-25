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

        public static readonly List<Choice> choicesKiswahili = new()
        {
            new Choice() { Value = ValidationsSwahili.YES, Synonyms = new List<string> { "y", "Y", "YES", "YE", "ye", "yE", "1" } },
            new Choice() { Value = ValidationsSwahili.NO, Synonyms = new List<string> { "n", "N", "no" } }
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
            new Choice() { Value ="Below 15",Synonyms=new List<string>{"1","2","3","4","5","6","7","8","9","10","11","12","13","14"}},
            new Choice() { Value ="15-20",Synonyms=new List<string>{"15","16","17","19","20"}},
            new Choice() { Value="21-30",Synonyms=new List<string>{"21","22","23","24","25","26","27","28","29","30"}},
            new Choice() {Value="31-40",Synonyms=new List<string>{"31","32","33","34","35","36","37","38","39","40"}},
            new Choice() {Value="Above 40"},
        };

        public static readonly List<Choice> AgeGroupKiswahili = new()
        {
            new Choice() { Value ="Chini ya miaka 15",Synonyms=new List<string>{"1","2","3","4","5","6","7","8","9","10","11","12"}},
            new Choice() { Value ="15-20",Synonyms=new List<string>{"15","16","17","19","20"}},
            new Choice() { Value="21-30",Synonyms=new List<string>{"21","22","23","24","25","26","27","28","29","30"}},
            new Choice() {Value="31-40",Synonyms=new List<string>{"31","32","33","34","35","36","37","38","39","40"}},
            new Choice() {Value="zaidi ya miaka 40"},
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


        public static List<Choice> FeelingsKiswahili = new()
        {
            new Choice() { Value=FeelingsSwahili.Furaha,Synonyms=new List<string>{"happy","HAPPY","Happy"}},
            new Choice() { Value=FeelingsSwahili.Hasira,Synonyms=new List<string>{"Angry","angry","ANGRY"}},
            new Choice() { Value=FeelingsSwahili.Kuchanganyikiwa},
            new Choice() { Value=FeelingsSwahili.Wasiwasi},
            new Choice() { Value=FeelingsSwahili.Zinginezo},
        };

        public static List<Choice> FeelingsList = new()
        {
            new Choice() { Value=Feelings.Happy,Synonyms=new List<string>{"happy","HAPPY","Happy"}},
            new Choice() { Value=Feelings.Angry,Synonyms=new List<string>{"Angry","angry","ANGRY"}},
            new Choice() { Value=Feelings.Anxious},
            new Choice() { Value=Feelings.FlatEffect},
            new Choice() { Value=Feelings.Expressionless},
            new Choice() { Value=Feelings.Sad},
            new Choice() { Value=Feelings.Other},
        };

        public static List<Choice> RelationShips = new()
        {
            new Choice  { Value ="Single",Synonyms=new List<string>{"Single","S"}},
            new Choice  { Value ="Married",Synonyms=new List<string>{"married"}},
            new Choice  { Value ="Divorced",Synonyms=new List<string>{"divorced"}},
            new Choice  { Value ="In A relationship",Synonyms=new List<string>{"dating","relations","casual"}},
            new Choice  { Value ="Widow /Widower",Synonyms=new List<string>{"widow","widower"}},
            new Choice  { Value ="Complicated",Synonyms=new List<string>{"complicated","comp","it's complicated"}},
            new Choice  { Value ="none",Synonyms=new List<string>{"none","no"}},
        };
        //Single, Married, Divorced, In a relationship, Widow / Widower, Complicated , Nimeoa/olewa, Nimetaliki, Niko kwenye mahusiano, Mjane
        public static List<Choice> RelationShipKiwahili = new()
        {
            new Choice  { Value ="Sijaoa/olewa",Synonyms=new List<string>{"Single","S"}},
            new Choice  { Value ="Nimeoa/olewa",Synonyms=new List<string>{"married"}},
            new Choice  { Value ="Nimetaliki",Synonyms=new List<string>{"divorced"}},
            new Choice  { Value ="Niko kwenye mahusiano",Synonyms=new List<string>{"dating","relations","casual"}},
            new Choice  { Value ="Mjane",Synonyms=new List<string>{"widow","widower"}},
            new Choice  { Value ="Sitaki kusema",Synonyms=new List<string>{"none","no"}},
        };

        public static List<Choice> ProfessionalOptions = new ()
        {
            new Choice  { Value ="Student",},
            new Choice  { Value ="Employed",},
            new Choice  { Value ="Entrepreneur"},
            new Choice  { Value ="Retired"},
            new Choice  { Value ="Unemployed"},
            new Choice  { Value ="Complicated"},
            new Choice  { Value ="none",Synonyms=new List<string>{"none","no"}},
        };
        //Student, Employed, Unemployed, Entrepreneur, Retired Mwanafunzi/Bado Nasoma, Nafanya kazi, Sina kazi, Mfanyabiashara, Nimestaafu

        public static List<Choice> ProfessionalOptionsKiswahili = new()
        {
            new Choice  { Value ="Mwanafunzi/Bado Nasoma",},
            new Choice  { Value ="Nafanya kazi",},
            new Choice  { Value ="Sina kazi"},
            new Choice  { Value ="Mfanyabiashara"},
            new Choice  { Value ="Nimestaafu"},
            new Choice  { Value ="Sitaki kusema",Synonyms=new List<string>{"none","no"}},
        };

        public static List<Choice> Genders = new List<Choice>
        {
            new Choice() { Value = Gender.Male,Synonyms=new List<string>{"M","Man","MALE","y"}},
            new Choice() { Value= Gender.Female,Synonyms=new List<string>{"f","fE","FEMALE","female"}},
            new Choice() { Value= Gender.Other,Synonyms=new List<string>{"o","other"}},
        };

        public static List<Choice> GenderKiswahili = new List<Choice>
        {
            new Choice() { Value = GenderSwahili.Male,Synonyms=new List<string>{"M","Man","MALE","y"}},
            new Choice() { Value = GenderSwahili.Female,Synonyms=new List<string>{"f","fE","FEMALE","female"}},
            new Choice() { Value = GenderSwahili.Other,Synonyms=new List<string>{"o","other"}},
        };

        public static List<Choice> Countrys = new List<Choice>
        {
            new Choice() { Value = CountryValidation.Kenya,Synonyms=new List<string>{"Kenya","KENYA"}},
            new Choice() { Value=  CountryValidation.Other,Synonyms=new List<string>{"o","other"}},
        };

        public static List<Choice> CountryKiswahili = new List<Choice>
        {
            new Choice() { Value = CountrySwahili.Kenya,Synonyms=new List<string>{"Kenya","KENYA"}},
            new Choice() { Value = CountrySwahili.Other,Synonyms=new List<string>{"o","other"}},
        };

        public static List<Choice> Reasons = new List<Choice>()
        {
            new Choice  { Value ="Suicidal ideations",},
            new Choice  { Value ="Feelings of hopelessness",},
            new Choice { Value = "Financial distress" },
            new Choice { Value = "Childhood trauma" },
            new Choice { Value = "Work related stress and burnout" },
        };

        public static List<Choice> ReasonsKiswahili = new List<Choice>()
        {
            new Choice  { Value ="Mawazo ya kutaka kujitoaa uhai",},
            new Choice  { Value ="Hisia za kukata tamaa",},
            new Choice { Value = "Shida za kifedha" },
            new Choice { Value = "Msongo wa utotoni wa kisaikolojia" },
            new Choice { Value = "Msongo wa kazi na kuchoka kikazi" },
        };

      
    }

    public static class Validations
    {
        public const string YES = "Yes";
        public const string NO = "No";
    }

    public static class ValidationsSwahili
    {
        public const string YES="Ndio";
        public const string NO="la";
    }

    public static class Gender
    {
        public const string Male = "Male";
        public const string Female = "Female";
        public const string Other = "Other";
    }
    public static class GenderSwahili
    {
        public const string Male = "Mume";
        public const string Female = "Mke";
        public const string Other = "Haijatajwa";
    }

    public static class CountryValidation
    {
        public const string Kenya = "Kenya";
        public const string Other = "Other";
    }

    public static class CountrySwahili
    {
        public const string Kenya = "Kenya";
        public const string Other = "Nyingine";
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
