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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RedCrossChat.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly ILogger _logger;
        private readonly FlightBookingRecognizer _luisRecognizer;
        private readonly string UserInfo = "Clien-info";
        // private bool personalDialogComplete = false;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(FlightBookingRecognizer luisRecognizer,

            CounselorDialog counselorDialog,
            PersonalDialog personalDialog,
            //AwarenessDialog awarenessDialog,
            ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            _luisRecognizer = luisRecognizer;
            _logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            AddDialog(counselorDialog);
            AddDialog(personalDialog);
            // AddDialog(awarenessDialog);

            var waterfallSteps = new WaterfallStep[]
            {
                    IntroStepAsync,
                    ActStepAsync,
                    ConfirmTermsAndConditionsAsync,
                    ValidateTermsAndConditionsAsync,
                    FinalStepAsync,
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var messageText = stepContext.Options?.ToString() ?? "Hello Welcome to Kenya Red Cross Society. How can I help you today?";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

            stepContext.Values[UserInfo] = new Client();

            var termsAndConditionsCard = PersonalDialogCard.GetIntendedActivity();
            var attachment = new Attachment
            {

                ContentType = HeroCard.ContentType,
                Content = termsAndConditionsCard
            };

            var message = MessageFactory.Attachment(attachment);

            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
            {
                Prompt = promptMessage,
                Choices = new List<Choice>
                        {
                            new Choice() { Value = "Careers", Synonyms = new List<string> { "1", "Careers", "careers" } },
                            new Choice() { Value = "Volunteer and Membership", Synonyms = new List<string> { "2", "Membership" } },
                            new Choice() { Value = "Volunteer Opportunities", Synonyms = new List<string> { "3", "Volunteer", "Opportunities" } },
                            new Choice() { Value = "Mental Health", Synonyms = new List<string> { "4", "Mental", "mental", "mental Health", "Mental Health", "Help" } }
                        },
                Style = stepContext.Context.Activity.ChannelId == "facebook" ? ListStyle.SuggestedAction : ListStyle.HeroCard,
            }, cancellationToken);


        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var turnContext = stepContext.Context;

            var termsAndConditionsCard = PersonalDialogCard.GetKnowledgeBaseCard();

            var career = PersonalDialogCard.GetKnowledgeCareerCard();

            var choiceValues = ((FoundChoice)stepContext.Result).Value;

            var VolunteerAttachmentMessage = MessageFactory.Attachment(new Attachment { Content = termsAndConditionsCard, ContentType = HeroCard.ContentType });

            var CareerAttachmentMessage = MessageFactory.Attachment(new Attachment { Content = career, ContentType = HeroCard.ContentType });

            var choices = new List<Choice>
            {
                new Choice { Value = "Membership", Action = new CardAction { Title = "Membership", Type = ActionTypes.OpenUrl, Value = "https://www.redcross.or.ke/individualmember" } },
                new Choice { Value = "Volunteer", Action = new CardAction { Title = "Volunteer", Type = ActionTypes.OpenUrl, Value = "https://www.redcross.or.ke/volunteer" } },
                // Add more choices as needed : todo :
            };

            var options = new PromptOptions
            {
                Prompt = MessageFactory.Text("To access our Volunteer or membership opportunities click on the links below"),
                Choices = choices,
                Style = ListStyle.SuggestedAction,
            };

            if (stepContext.Result != null)
            {

                var choiceValue = ((FoundChoice)stepContext.Result).Value;

                if (choiceValue == null)
                {
                    if (turnContext.Activity.ChannelId == "telegram")
                    {
                        await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);
                    }
                    else
                    {
                        await stepContext.Context.SendActivityAsync(VolunteerAttachmentMessage, cancellationToken);
                    }

                }

                if (choiceValue == InitialActions.MentalHealth)
                {
                    return await stepContext.NextAsync(null);
                }

                if (choiceValue == InitialActions.Careers && turnContext.Activity.ChannelId != "telegram")
                {
                    await stepContext.Context.SendActivityAsync(CareerAttachmentMessage, cancellationToken);

                    return await stepContext.EndDialogAsync(null);
                }

                if (choiceValue == InitialActions.Careers && turnContext.Activity.ChannelId == "telegram")
                {
                    /*await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
                    {
                        Prompt = MessageFactory.Text("To access our career or membership opportunities click on the links below"),
                        Choices = new List<Choice>
                        {
                              new Choice { Value = InitialActions.Careers, Action = new CardAction { Title = "Careers", Type = ActionTypes.OpenUrl, Value = "https://www.redcross.or.ke/careers" } },
                              new Choice { Value = InitialActions.VolunteerAndMemberShip, Action = new CardAction { Title = "Volunteer", Type = ActionTypes.OpenUrl, Value = "https://www.redcross.or.ke/volunteer" } },

                        },
                        Style = ListStyle.HeroCard,
                    }, cancellationToken);*/

                    //todo 

                     var reply = MessageFactory.Text("Here's a link for you:");
                        reply.SuggestedActions = new SuggestedActions
                        {
                            Actions = new List<CardAction>
                            {
                                new CardAction { Title = "Open Link", Type = ActionTypes.OpenUrl, Value = "https://www.example.com" }
                            }
                        };

                     await stepContext.Context.SendActivityAsync(reply, cancellationToken);


                    return await stepContext.EndDialogAsync(null);
                }

            }


            if (turnContext.Activity.ChannelId == "telegram")
            {
                await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(VolunteerAttachmentMessage, cancellationToken);
            }

            return await stepContext.EndDialogAsync(null);

        }


        private async Task<DialogTurnResult> ConfirmTermsAndConditionsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

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
                Choices = new List<Choice>
                        {
                            new Choice() { Value = "Yes", Synonyms = new List<string> { "y", "Y", "YES", "YE", "ye", "yE", "1" } },
                            new Choice() { Value = "No", Synonyms = new List<string> { "n", "N", "no" } }
                        },
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
                return await stepContext.BeginDialogAsync(nameof(PersonalDialog), null, cancellationToken);
            }
            else
            {
                // If the user does not confirm, end the dialog
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("You need to agree to the data protection policy to proceed."));
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
        }


        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Check if the result is of type BookingDetails (result from PersonalDialog)

            //add ai todo

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thank you for reaching out good bye 😀."));

            // The result is null or of an unexpected type, return an empty response
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }


    }
}