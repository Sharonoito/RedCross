using Microsoft.Bot.Schema;
using System.Collections.Generic;

namespace RedCrossChat.Cards
{
    public class PersonalDialogCard
    {
        //Usechoice prompt instead of herocard
        public static HeroCard GetKnowYouCard(bool language = true)
        {
            return new HeroCard
            {

                Title = "Chat-care",
                Subtitle = language ? "Terms and Conditions" : "Sheria na Masharti",
                Text = language ? "We would like to know more about you ? Please accept terms and conditions below to continue." : "Tungependa kukufahamu zaidi ya ulivyotueleza. Ili kuendelea, tafadhali kubali sheria na masharti yafuatayo",
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, language ? "Terms and Conditions" : "Sheria na Masharti", value: "https://www.redcross.or.ke/ASSETS/DATA-PROTECTION-POLICY.pdf") },
            };

        }

        public static HeroCard GetKnowledgeBaseCard()
        {
            var heroCard = new HeroCard
            {
                Title = "Chat-care",
                Subtitle = "Volunteers & MemberShip",
                Text = "To volunteer or be involved in our activities please follow the link.",
                Buttons = new List<CardAction> {
                    new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://wema.redcross.or.ke/")

                },
            };

            return heroCard;
        }

        public static HeroCard GetKnowledgeBaseCardSwahili()
        {
            var heroCard = new HeroCard
            {
                Title = "Chat-care",
                Subtitle = "Kujitolea na kuwa mshirika",
                Text = "Kujitolea na kuwa mshirika tafadhali fuata kiungo hiki.",
                Buttons = new List<CardAction> {
                    new CardAction(ActionTypes.OpenUrl, "Anza", value: "https://www.redcross.or.ke/careers")
                },
            };
            return heroCard;
        }
        public static HeroCard GetKnowledgeCareerCard()
        {
            var heroCard = new HeroCard
            {
                Title = "Chat-care |  Careers",
                
                Text = "For career opportunities please follow this link.",
                Buttons = new List<CardAction> {
                    new CardAction(ActionTypes.OpenUrl, "Career", value: "https://www.redcross.or.ke/careers")
                },
            };

            return heroCard;
        }

        public static HeroCard GetKnowledgeCareerCardSwahili()
        {
            var heroCard = new HeroCard
            {
                Title = "Chat-care |  Taaluma",

                Text = "Kwa taaluma tafadhali fuata kiungo hiki",
                Buttons = new List<CardAction> {
                    new CardAction(ActionTypes.OpenUrl, "Taaluma", value: "https://www.redcross.or.ke/careers")
                },
            };

            return heroCard;
        }

        public static HeroCard GetHotlineCard()
        {
            var heroCard = new HeroCard
            {
                Title = "Hotline numbers ",
                Subtitle = "Referral pathways",
                Text = "For any other advanced support feel free to check our detailed referral pathways",

                 Buttons = new List<CardAction> {
                    new CardAction(ActionTypes.OpenUrl, "Preview", value: "https://referraldirectories.redcross.or.ke/")

                },
            };

            return heroCard;
        }

       
        public static HeroCard GetHotlineCardKiswahili()
        {
            var heroCard = new HeroCard
            {
                Title = "Nambari za dharura ",
                Subtitle = "Maeneo ya rufaa",
                Text = "Kwa msaada wa kiwango cha juu zaidi, tafadhali jisikie huru kuangalia njia zetu za rufaa zilizodhibitiwa kwa undani.",

                Buttons = new List<CardAction> {
                    new CardAction(ActionTypes.OpenUrl, "Angalia kwa hakika.", value: "https://referraldirectories.redcross.or.ke/")

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
