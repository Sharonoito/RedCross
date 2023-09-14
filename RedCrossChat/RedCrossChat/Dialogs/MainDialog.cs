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

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var question = "Hello dear friend!! Welcome to Kenya Red Cross Society, we are offering tele-counselling services to public at no charges . How can I help you today?\r\n";

            stepContext.Values[UserInfo] = new Client();

            var messageText = stepContext.Options?.ToString() ?? question;

            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
            {
                Prompt = promptMessage,
                Choices =RedCrossLists.Actions,
                Style = stepContext.Context.Activity.ChannelId == "facebook" ? ListStyle.SuggestedAction : ListStyle.HeroCard,
            }, cancellationToken);

        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        { 
            var knowledgeBaseCard = PersonalDialogCard.GetKnowledgeBaseCard();

            var career = PersonalDialogCard.GetKnowledgeCareerCard();

            var choiceValues = ((FoundChoice)stepContext.Result).Value;

            var message = MessageFactory.Attachment(
                    new Attachment
                    {
                        ContentType = HeroCard.ContentType,
                        Content = knowledgeBaseCard
                    }
            );

            if (stepContext.Result != null)
            {

                if (choiceValues == InitialActions.MentalHealth)
                {
                    return await stepContext.NextAsync(null);
                }
                else if (choiceValues == InitialActions.Careers)
                {
                    message = MessageFactory.Attachment(
                        new Attachment
                        {
                            Content = career,
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
                            Content = career,
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

            if (me.DialogClosed) {

              return await stepContext.EndDialogAsync(null);       
            }

            var termsAndConditionsCard = PersonalDialogCard.GetKnowYouCard();

            var attachment = new Attachment
            {
                ContentType = HeroCard.ContentType,
                Content = termsAndConditionsCard
            };

            var message = MessageFactory.Attachment(attachment);
            await stepContext.Context.SendActivityAsync(message, cancellationToken);

            // Prompt the user if they agree with the terms and conditions
            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text("Do you agree to the Terms and Conditions? Please select 'Yes' or 'No'."),
                RetryPrompt = MessageFactory.Text("Please select a valid option ('Yes' or 'No')."),
                Choices = RedCrossLists.choices,
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
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);
        }


        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Check if the result is of type BookingDetails (result from PersonalDialog)

            //add ai todo

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("let not your struggles define your identity. Remember, there's no health without mental health. GoodBye"));

            // The result is null or of an unexpected type, return an empty response
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }


    }
}