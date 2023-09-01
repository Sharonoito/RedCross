using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Sentry.Protocol;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RedCrossChat.Dialogs
{
    public class AiDialog : CancelAndHelpDialog
    {
        protected readonly ILogger _logger;


        public AiDialog(ILogger<AiDialog> logger) : base(nameof(AiDialog))
        {
            _logger = logger;

            var waterFallSteps = new WaterfallStep[]
               {
                    IntialTaskAsync,
                    FetchResultsAsync,
                    FinalStepAsync
               };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterFallSteps));
        }

        public async Task<DialogTurnResult> IntialTaskAsync(WaterfallStepContext stepContext,CancellationToken token)
        {

            var options=new PromptOptions();

            var promptMessage = MessageFactory.Text("Ask me a question", null, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, token);
        }

        public async Task<DialogTurnResult>  FetchResultsAsync(WaterfallStepContext stepContext, CancellationToken token)
        {
            //show results and request for another 

            var options = new PromptOptions();

            try
            {
                
                var response = await ChatGptDialog.GetChatGPTResponses(stepContext.Context.Activity.Text);

                await stepContext.Context.SendActivityAsync(MessageFactory.Text(response));

                return await stepContext.EndDialogAsync();
            }
            catch (Exception ex)
            {

                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Our AI servers seems to be down please try later"));


                _logger.LogError(ex.Message);

                return await stepContext.EndDialogAsync();
            }

           
           // return await stepContext.PromptAsync(nameof(TextPrompt), options, token);
        }

        public async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken token)
        {
            //show results and request for another 

            var options = new PromptOptions();

            return await stepContext.EndDialogAsync(null);
        }

    }
}
