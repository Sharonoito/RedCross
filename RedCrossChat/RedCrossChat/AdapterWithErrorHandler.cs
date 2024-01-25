
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace RedCrossChat
{
    public class AdapterWithErrorHandler : CloudAdapter
    {
        private readonly IWebHostEnvironment environment;

        public AdapterWithErrorHandler(BotFrameworkAuthentication auth, ILogger<IBotFrameworkHttpAdapter> logger, ConversationState conversationState = default, IWebHostEnvironment environment=default)
            : base(auth, logger)
        {
            

            OnTurnError = async (turnContext, exception) =>
            {


                // Log any leaked exception from the application.
                logger.LogError(exception, $"[OnTurnError] unhandled error : {exception.Message}");

                // Send a message to the user

                if(environment.IsDevelopment())
                {
                    var errorMessageText = $"The bot encountered an error or bug. {exception.Message}";
                    var errorMessage = MessageFactory.Text(errorMessageText, errorMessageText, InputHints.ExpectingInput);
                    await turnContext.SendActivityAsync(errorMessage);

                    if (exception.InnerException != null)
                    {
                        errorMessageText = $"The bot encountered an error or bug. {exception.InnerException}";
                        errorMessage = MessageFactory.Text(errorMessageText, errorMessageText, InputHints.ExpectingInput);
                        await turnContext.SendActivityAsync(errorMessage);
                    }
                    else
                    {

                    }


                    errorMessageText = "To continue to run this bot, please fix the bot source code.";
                    errorMessage = MessageFactory.Text(errorMessageText, errorMessageText, InputHints.ExpectingInput);
                    await turnContext.SendActivityAsync(errorMessage);
                }

                

                if (conversationState != null)
                {
                    try
                    {
                        // Delete the conversationState for the current conversation to prevent the
                        // bot from getting stuck in a error-loop caused by being in a bad state.
                        // ConversationState should be thought of as similar to "cookie-state" in a Web pages.
                        await conversationState.DeleteAsync(turnContext);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, $"Exception caught on attempting to Delete ConversationState : {e.Message}");
                    }
                }

                // Send a trace activity, which will be displayed in the Bot Framework Emulator
                await turnContext.TraceActivityAsync("OnTurnError Trace", exception.Message, "https://www.botframework.com/schemas/error", "TurnError");
            };
            this.environment = environment;
        }
    }
}
