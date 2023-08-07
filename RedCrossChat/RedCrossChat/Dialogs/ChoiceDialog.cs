using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using System.Collections.Generic;
using System;
using Microsoft.Bot.Builder.Dialogs.Choices;
using RedCrossChat.Dialogs;
using Newtonsoft.Json.Linq;

namespace RedCrossChat.Dialogs
{
    public class ChoiceDialog : CancelAndHelpDialog
    {
        private bool personalDialogComplete = false;

        public ChoiceDialog(PersonalDialog personalDialog, AwarenessDialog awarenessDialog, ILogger<ChoiceDialog> logger) : base(nameof(ChoiceDialog))
        {
            AddDialog(new ChoicePrompt(nameof(ChoiceDialog)));
            AddDialog(personalDialog);
            AddDialog(awarenessDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[] 
            {
                InitialStepAsync,
                FinalStepAsync,
            }));

        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Check if PersonalDialog is not yet complete
            if (!personalDialogComplete)
            {
                //return await PersonalStepAsync(stepContext, cancellationToken);
                return await stepContext.BeginDialogAsync(nameof(PersonalDialog), null, cancellationToken);

            }

            // Reset the flag for the next turn
            personalDialogComplete = false;


            return await AwarenessStepAsync(stepContext, cancellationToken);
        }
        private async Task<DialogTurnResult> AwarenessStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Call the AwarenessDialog
            return await stepContext.BeginDialogAsync(nameof(AwarenessDialog), null, cancellationToken);
        }

        private Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


    }

}
            
            
            
            
            
            


