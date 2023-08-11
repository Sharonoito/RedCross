using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Identity.Client;
using System.Threading;
using System.Threading.Tasks;

namespace RedCrossChat.Dialogs
{
    public class CounselorDialog  :CancelAndHelpDialog
    {

        public CounselorDialog():base(nameof(CounselorDialog)) {

            AddDialog(new TextPrompt(nameof(TextPrompt)));

            var waterfallSteps = new WaterfallStep[]
            {
                IntroStepAsync,
                EvaluateStepAsync,
                FinalStepAsync,
            };


            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));    

            InitialDialogId= nameof(WaterfallDialog);
        }

        public async Task<DialogTurnResult>  IntroStepAsync(WaterfallStepContext stepContext,CancellationToken cancellationToken)
        {

            var promptMessage = MessageFactory.Text("Hellow and welcome", null, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }
        public async Task<DialogTurnResult> EvaluateStepAsync(WaterfallStepContext stepContext,CancellationToken cancellationToken)
        {

            var promptMessage = MessageFactory.Text("Hellow and again and again", null, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        public async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

    }
}
