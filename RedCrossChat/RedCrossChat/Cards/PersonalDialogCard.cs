﻿using Microsoft.Bot.Schema;
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