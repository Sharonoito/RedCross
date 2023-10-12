using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Collections.Generic;

namespace RedCrossChat.Objects
{

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

        public static List<Choice> GetRating(bool language)
        {
            return language ? Ratings : KiswahiliRating;
        }

        public static readonly List<Choice> Ratings = new()
        {
             new Choice() { Value = "Excellent", Synonyms = new List<string> {  "1" } },
             new Choice() { Value = "Average", Synonyms = new List<string> {  "3" } },
             new Choice() { Value = "Good", Synonyms = new List<string> { "2" } },
             new Choice() { Value = "Bad", Synonyms = new List<string> {  "4" } },
             new Choice() { Value = "Terrible", Synonyms = new List<string> {  "5" } },
        };


        public static readonly List<Choice> KiswahiliRating = new()
        {
             new Choice() { Value = "Bora kabisa", Synonyms = new List<string> {  "1" } },
             new Choice() { Value = "Wastani", Synonyms = new List<string> {  "3" } },
             new Choice() { Value = "Sawa", Synonyms = new List<string> { "2" } },
             new Choice() { Value = "Mbaya", Synonyms = new List<string> {  "4" } },
             new Choice() { Value = "Mbaya Kabisa", Synonyms = new List<string> {  "5" } },
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
        public const string NO="La";
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
        public const string AIDialogAssesor = "AI_DIALOG";
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
