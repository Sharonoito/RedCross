using Microsoft.Bot.Schema;
using RedCrossChat.Domain;
using RedCrossChat.Objects;
using System;
using System.Collections.Generic;

namespace RedCrossChat.Cards
{
    public class PersonalDialogCard
    {
        private const string APP_TITLE="Chat-Care";
        //Usechoice prompt instead of herocard
        public static HeroCard GetKnowYouCard(bool language = true)
        {
            return new HeroCard
            {
                Title = APP_TITLE,
                Subtitle = language ? "Terms and Conditions" : "Masharti na Sheria",
                Text = language ? "For us to be able to help you better, we would like to know more about you! Please accept terms and conditions below to continue." : "Ili tuweze kukusaidia vizuri, tungependa kujua zaidi kukuhusu! Tafadhali kubali masharti na sheria hapa chini kabla ya kuendelea.\r\n\r\n",
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, language ? "Terms and Conditions" : "Sheria na Masharti", value: "https://www.redcross.or.ke/privacy-policy/") },
            };

        }

        public static HeroCard GetKnowledgeBaseCard(bool language = true)
        {
            var heroCard = new HeroCard
            {
                Title = APP_TITLE,
                Subtitle = language? InitialActions.VolunteerAndMemberShip : InitialActionsKiswahili.VolunteerAndMemberShip,
                Text = language? "To volunteer or be involved in our activities please follow the link.": "Kujitolea na kuwa mshirika tafadhali fuata kiungo hiki.",
                Buttons = new List<CardAction> {
                    new CardAction(ActionTypes.OpenUrl,language? "Get Started":"Anza", value: "https://wema.redcross.or.ke/")

                },
            };

            return heroCard;
        }

        public static HeroCard GetMembershipCard(bool language = true)
        {
            var heroCard = new HeroCard
            {
                Title = APP_TITLE,
                Subtitle = language ? InitialActions.VolunteerOpportunities : InitialActionsKiswahili.VolunteerOpportunities,
                Text = language ? "To volunteer or be involved in our activities please follow the link." : "Kujitolea na kuwa mshirika tafadhali fuata kiungo hiki.",
                Buttons = new List<CardAction> {
                    new CardAction(ActionTypes.OpenUrl,language? "Get Started":"Anza", value: "https://wema.redcross.or.ke/")

                },
            };

            return heroCard;
        }

        public static HeroCard GetKnowledgeCareerCard(Boolean language)
        {
            var heroCard = new HeroCard
            {
                Title= APP_TITLE,
                Subtitle = language ? InitialActions.Careers: InitialActionsKiswahili.Careers,      
                Text = language ?  "For career opportunities please follow this link." : "Kwa taaluma tafadhali fuata kiungo hiki",
                Buttons = new List<CardAction> {
                    new CardAction(ActionTypes.OpenUrl, language? "Career":"Taaluma" , value: "https://www.redcross.or.ke/careers")
                },
            };

            return heroCard;
        }

      

        public static HeroCard GetHotlineCard(Boolean language)
        {
            var heroCard = new HeroCard
            {
                Title = language? "Hotline numbers ": "Nambari za dharura ",
                Subtitle = language?"Referral pathways": "Maeneo ya rufaa",
                Text = language? "For any other advanced support feel free to check our detailed referral pathways":
                                 "Kwa msaada wa kiwango cha juu zaidi, tafadhali jisikie huru kuangalia njia zetu za rufaa zilizodhibitiwa kwa undani.",

                 Buttons = new List<CardAction> {
                    new CardAction(ActionTypes.OpenUrl, language ? "Preview": "Angalia kwa hakika.", value: "https://referraldirectories.redcross.or.ke/")

                },
            };

            return heroCard;
        }
    }
}
