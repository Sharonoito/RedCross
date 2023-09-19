// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.18.1

using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using RedCrossChat;
using RedCrossChat.Cards;
using RedCrossChat.CognitiveModels;
using RedCrossChat.Objects;
using Sentry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Attachment = Microsoft.Bot.Schema.Attachment;

namespace RedCrossChat.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly ILogger _logger;
        private readonly FlightBookingRecognizer _luisRecognizer;
        private readonly string UserInfo = "Clien-info";

        public MainDialog(FlightBookingRecognizer luisRecognizer,

            CounselorDialog counselorDialog,
            PersonalDialog personalDialog,
            AiDialog aiDialog,
            ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            _luisRecognizer = luisRecognizer;
            _logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            AddDialog(counselorDialog);
            AddDialog(personalDialog);
            AddDialog(aiDialog);
            // AddDialog(awarenessDialog);

            var waterfallSteps = new WaterfallStep[]
            {
                    FirstStepAsync,
                    IntroStepAsync,
                    ActStepAsync,
                    ConfirmTermsAndConditionsAsync,
                    ValidateTermsAndConditionsAsync,
                   // HandleAiInteractions,
                    RateBotAsync,
                    FinalStepAsync,
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> FirstStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values[UserInfo] = new Client();

            var question = "Hello dear friend!! Welcome to Kenya Red Cross Society.  Hujambo rafiki? Karibu katika Shirika la Msalaba Mwekundu ambapo tunatoa ushauri kupitia kwenye simu bila malipo yoyote. ?\r\n please select a language to continue, Tafadhali chagua lugha ndio tuendelee ku wasiliana";

            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text(question),
                RetryPrompt = MessageFactory.Text("Please select a valid question"),
                Choices = new List<Choice>()
                {
                    new Choice("English"),
                    new Choice("Kiswahili")
                },
                // Style = stepContext.Context.Activity.ChannelId == "facebook" ? ListStyle.SuggestedAction : ListStyle.HeroCard,
                Style = ListStyle.HeroCard,
            }, cancellationToken); ;
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            Client client = (Client)stepContext.Values[UserInfo];


            var choiceValues = ((FoundChoice)stepContext.Result).Value;

            if (choiceValues != null   && choiceValues == "Kiswahili")
            {
                client.language = !client.language;
            }


            var question = client.language ?


                "Hello dear friend!! Welcome to Kenya Red Cross Society, we are offering tele-counselling services to public at no charges . How can I help you today?\r\n" :

                "Hujambo rafiki? Karibu katika Shirika la Msalaba Mwekundu ambapo tunatoa ushauri kupitia kwenye simu bila malipo yoyote. Je, ungependa nikusaidie vipi?";


            var messageText = stepContext.Options?.ToString() ?? question;

            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
            {
                Prompt = promptMessage,
                Choices = client.language ? RedCrossLists.Actions : RedCrossLists.ActionKiswahili,
                // Style = stepContext.Context.Activity.ChannelId == "facebook" ? ListStyle.SuggestedAction : ListStyle.HeroCard,
                Style =  ListStyle.HeroCard,
            }, cancellationToken);

        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client client = (Client)stepContext.Values[UserInfo];


            //var knowledgeBaseCard = PersonalDialogCard.GetKnowledgeBaseCard();

            //var career = PersonalDialogCard.GetKnowledgeCareerCard();


            var question = client.language ?


                "Hello dear friend!! Welcome to Kenya Red Cross Society, we are offering tele-counselling services to public at no charges . How can I help you today?\r\n" :

                "Hujambo rafiki? Karibu katika Shirika la Msalaba Mwekundu ambapo tunatoa ushauri kupitia kwenye simu bila malipo yoyote. Je, ungependa nikusaidie vipi?";




            var choiceValues = ((FoundChoice)stepContext.Result).Value;

            if (choiceValues != null && choiceValues == "Kiswahili")
            {
                client.language = !client.language;
            }
 
            

            var mentalHealth = PersonalDialogCard.GetKnowYouCard();
            var mentalHealth1 = PersonalDialogCard.GetKnowYouCardKiswahili();


            var knowledgeBaseCard = PersonalDialogCard.GetKnowledgeCareerCard();
            var career1 = PersonalDialogCard.GetKnowledgeCareerCardSwahili();

            var volunteer = PersonalDialogCard.GetKnowledgeBaseCard();
            var volunteer1 = PersonalDialogCard.GetKnowledgeBaseCardSwahili();

            

            var message = MessageFactory.Attachment(
                    new Attachment
                    {
                        ContentType = HeroCard.ContentType,
                        //Content = knowledgeBaseCard
                        Content = client.language
                        ? (choiceValues == InitialActions.Careers ? volunteer : volunteer)
                        : (choiceValues == InitialActions.Careers ? volunteer1 : volunteer1)

                    }
            );

            if (stepContext.Result != null)
            {

                if (choiceValues == InitialActions.MentalHealth || choiceValues == InitialActionsKiswahili.MentalHealth)
                {
                    return await stepContext.NextAsync(null);
                }
                else if (choiceValues == InitialActions.Careers || choiceValues == InitialActionsKiswahili.Careers)
                {
                    message = MessageFactory.Attachment(
                        new Attachment
                        {
                            //Content = career,
                            // Content = client.language ? knowledgeBaseCard : career,
                            Content = client.language
                            ? (choiceValues == InitialActions.Careers ? knowledgeBaseCard : knowledgeBaseCard)
                            : (choiceValues == InitialActions.Careers ? career1 : career1),


                            ContentType = HeroCard.ContentType

                        }
                    ); 
                    
                }
            }
            else
            {
                message = MessageFactory.Attachment(
                        new Attachment
                        {
                            //Content = career,
                            //Content = client.language ? knowledgeBaseCard : career,
                            Content = client.language
                                ? (choiceValues == InitialActions.Careers ? knowledgeBaseCard : knowledgeBaseCard)
                                : (choiceValues == InitialActions.Careers ? knowledgeBaseCard : knowledgeBaseCard),
                        
                            ContentType = HeroCard.ContentType
                        }
                    );
            }

            await stepContext.Context.SendActivityAsync(message, cancellationToken);

            return await stepContext.EndDialogAsync(null);

        }


        private async Task<DialogTurnResult> ConfirmTermsAndConditionsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {


            Client me = (Client)stepContext.Values[UserInfo];

            if (me.DialogClosed)
            {

                return await stepContext.EndDialogAsync(null);
            }

            var termsAndConditionsCard = PersonalDialogCard.GetKnowYouCard();
            var termsAndConditionsCardSwahili = PersonalDialogCard.GetKnowYouCardKiswahili();



            var attachment = new Attachment
            {
                ContentType = HeroCard.ContentType,
                //Content = termsAndConditionsCard
                Content = !me.language
                  ? termsAndConditionsCardSwahili: termsAndConditionsCard

            };

            var message = MessageFactory.Attachment(attachment);
            await stepContext.Context.SendActivityAsync(message, cancellationToken);

            // Prompt the user if they agree with the terms and conditions
            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text("Do you agree to the Terms and Conditions? Please select 'Yes' or 'No'."),
                RetryPrompt = MessageFactory.Text("Please select a valid option ('Yes' or 'No')."),
                Choices = RedCrossLists.choices,
                Style = ListStyle.HeroCard,
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);
        }

        public async Task<DialogTurnResult> ValidateTermsAndConditionsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await EvaluateDialog.ProcessStepAsync(stepContext, cancellationToken);

            string confirmation = ((FoundChoice)stepContext.Result).Value;

            if (confirmation.Equals("Yes", StringComparison.OrdinalIgnoreCase))
            {
                // If the user confirms with 'Yes', proceed to TermsAndConditionsAsync

                await stepContext.Context.SendActivityAsync(MessageFactory.Text("To exit the bot \n type exit or cancel at any point ."));

                return await stepContext.BeginDialogAsync(nameof(PersonalDialog), null, cancellationToken);
            }
            else
            {
                // If the user does not confirm, end the dialog
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("You need to agree to the data protection policy to proceed."));
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> HandleAiInteractions(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(AiDialog), null, cancellationToken);
        }

        private async Task<DialogTurnResult> RateBotAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text("How would you rate your experience.with the bot?"),
                RetryPrompt = MessageFactory.Text("Please select a valid option ('Yes' or 'No')."),
                Choices = RedCrossLists.Ratings,
                Style = ListStyle.HeroCard,
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);
        }
 
        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ratingChoice = ((FoundChoice)stepContext.Result).Value;

            if (ratingChoice.Equals("PositiveChoice"))  
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thank you for your positive feedback! We're glad you had a great experience."));
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thank you for your feedback. We value your input!"));
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

    }
}