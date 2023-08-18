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
            new Choice() { Value = "Yes", Synonyms = new List<string> { "y", "Y", "YES", "YE", "ye", "yE", "1" } },
            new Choice() { Value = "No", Synonyms = new List<string> { "n", "N", "no" } }
        };
    }


    public static class Validations
    {
        public static readonly string YES="Yes";
        public static readonly string NO="No";


    }
}
