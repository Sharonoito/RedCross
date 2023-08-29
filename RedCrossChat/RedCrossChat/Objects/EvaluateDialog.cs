using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using System.Threading;

namespace RedCrossChat.Objects
{
    public static class EvaluateDialog
    {
        public static async Task<DialogTurnResult> ProcessStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userInput = stepContext.Result as string;

            if (userInput != null && userInput.ToLower().Trim() == "exit")
            {
                await stepContext.Context.SendActivityAsync("You have exited the bot. Goodbye!", cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

            // Process the user input or continue the conversation flow
            return null;
        }

    }
}
