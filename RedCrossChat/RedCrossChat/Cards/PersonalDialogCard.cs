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
                Images = new List<CardImage> { new CardImage("https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg") },
                Buttons = new List<CardAction> {
                    new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://www.redcross.or.ke/careers")
                },
            };

            return heroCard;
        }

    }
}
