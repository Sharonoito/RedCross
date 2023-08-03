// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.18.1

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using RedCrossChat;
using RedCrossChat.CognitiveModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RedCrossChat.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly ILogger _logger;
        private readonly FlightBookingRecognizer _luisRecognizer;
        private bool personalDialogComplete = false;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(FlightBookingRecognizer luisRecognizer, 
            BookingDialog bookingDialog,
            CounselorDialog counselorDialog,
            PersonalDialog personalDialog,
            AwarenessDialog awarenessDialog,
            ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            _luisRecognizer = luisRecognizer;
            _logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(bookingDialog);
            AddDialog(counselorDialog);
            AddDialog(personalDialog);
            AddDialog(awarenessDialog);

            var waterfallSteps = new WaterfallStep[]
            {
                    IntroStepAsync,
                    ActStepAsync,
                    //PersonalStepAsync,
                    AwarenessStepAsync,
                    FinalStepAsync,
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Use the text provided in FinalStepAsync or the default if it is the first time.
            var messageText = stepContext.Options?.ToString() ?? "Hello Welcome to Kenya Red Cross Society. How can I help you today?";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }
        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Check if PersonalDialog is not yet complete
            if (!personalDialogComplete)
            {
                //return await PersonalStepAsync(stepContext, cancellationToken);
                return await stepContext.BeginDialogAsync(nameof(PersonalDialog), null, cancellationToken);

            }

            // Reset the flag for the next turn
            personalDialogComplete = false;

            // Rest of the existing code for handling LUIS intents
            // ...

            // If the flow reaches here, it means we should continue to the next step in the waterfall.
            return await AwarenessStepAsync(stepContext, cancellationToken);
        }


        private async Task<DialogTurnResult> AwarenessStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Call the AwarenessDialog
            return await stepContext.BeginDialogAsync(nameof(AwarenessDialog), null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Check if the result is of type BookingDetails (result from PersonalDialog)
            if (stepContext.Result is BookingDetails result)
            {
                // Now we have all the booking details call the booking service.

                // If the call to the booking service was successful tell the user.

                var timeProperty = new TimexProperty(result.TravelDate);
                var travelDateMsg = timeProperty.ToNaturalLanguage(DateTime.Now);
                var messageText = $"I have you booked to {result.Destination} from {result.Origin} on {travelDateMsg}";
                var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
                await stepContext.Context.SendActivityAsync(message, cancellationToken);

                // Continue to the next step in the waterfall
                return await stepContext.NextAsync(null, cancellationToken);
            }
            else if (stepContext.Result is DialogTurnResult dialogResult && dialogResult.Result is BookingDetails)
            {
                // The result is from the AwarenessDialog, restart the main dialog with a different message
                var promptMessage = "What else can I do for you?";
                return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
            }

            // The result is null or of an unexpected type, return an empty response
            return await stepContext.EndDialogAsync(cancellationToken);
        }


    }
}
