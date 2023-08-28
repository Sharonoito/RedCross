using Microsoft.Bot.Schema;
using System.Collections.Generic;

namespace RedCrossChat.Cards
{
    public class PersonalDialogCard
    {
        public static HeroCard GetKnowYouCard()
        {
            return new HeroCard
            {

                Title = "Redcross Chat bot",
                Subtitle = "Terms and Conditions",
                Text = "We would like to know more about you ? Please accept terms and conditions below to continue.",
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Terms and Conditions", value: "https://www.redcross.or.ke/ASSETS/DATA-PROTECTION-POLICY.pdf") },
            };
        }

        public static HeroCard GetKnowledgeBaseCard()
        {
            var heroCard = new HeroCard
            {
                Title = "RedCross",
                Subtitle = "Volunteers & MemberShip",
                Text = "To volunteer or be involved in our activities please follow the link.",
                Buttons = new List<CardAction> {
                    new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://www.redcross.or.ke/careers")
                },
            };

            return heroCard;
        }

        public static HeroCard GetKnowledgeCareerCard()
        {
            var heroCard = new HeroCard
            {
                Title = "RedCross",
                Subtitle = "Careers",
                Text = "For career opportunities please follow this link.",
                Buttons = new List<CardAction> {
                    new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://www.redcross.or.ke/careers")
                },
            };

            return heroCard;
        }

        public static HeroCard GetHotlineCard()
        {
            var heroCard = new HeroCard
            {
                Title = "Hotline Numbers",
                Subtitle = "Hotline",
                Text = "Here's our hotline numbers should you require any assistance.",
                 Buttons = new List<CardAction> {
                    new CardAction(ActionTypes.OpenUrl, "Get Started", value: "http://localhost:3978/api/messages")
                },
            };

            return heroCard;
        }


        public static HeroCard GetIntendedActivity()
        {
            return new HeroCard
            {
                Text = "You can upload an image or select one of the following choices",
                Buttons = new List<CardAction>
                {
                    // Note that some channels require different values to be used in order to get buttons to display text.
                    // In this code the emulator is accounted for with the 'title' parameter, but in other channels you may
                    // need to provide a value for other parameters like 'text' or 'displayText'.
                    new CardAction(ActionTypes.ImBack, title: "1. Inline Attachment", value: "1"),
                    new CardAction(ActionTypes.ImBack, title: "2. Internet Attachment", value: "2"),
                    new CardAction(ActionTypes.ImBack, title: "3. Uploaded Attachment", value: "3"),
                },
            };
        }

    }
}
