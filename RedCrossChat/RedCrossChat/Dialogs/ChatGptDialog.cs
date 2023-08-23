using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace RedCrossChat.Dialogs
{
    public class ChatGptDialog : CancelAndHelpDialog
    {

        private readonly ILogger _logger;

        private readonly ChatGpt _chat;
        
        public ChatGptDialog(ChatGpt chat, ILogger<ChatGptDialog> logger) :base(nameof(ChatGptDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog),CreateWaterfallSteps()));

            InitialDialogId=nameof(WaterfallDialog);
        }

        public WaterfallStep[] CreateWaterfallSteps()
        {
            return new WaterfallStep[]
            {
                IntialStepAsync,
                HandleQuestionAsync,
                FinalStepAsync,
            };
        }

        public async Task<DialogTurnResult> IntialStepAsync(WaterfallStepContext stepContext,CancellationToken token)
        {

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { }, token);
        }

        public async Task<DialogTurnResult> HandleQuestionAsync(WaterfallStepContext stepContext,CancellationToken token)
        {
            var response = await _chat.GenerateGptResponseAsync("How to create pages");

            return await stepContext.PromptAsync(nameof(TextPrompt),new PromptOptions { }, token);
        }

        public async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken token)
        {
            return await stepContext.EndDialogAsync(null);
        }


    }
}
