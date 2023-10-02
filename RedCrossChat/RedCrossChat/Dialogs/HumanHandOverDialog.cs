using Microsoft.Bot.Builder.Dialogs;
using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RedCrossChat.Dialogs
{
    public class HumanHandOverDialog : CancelAndHelpDialog
    {
        private IRepositoryWrapper repository;

        public HumanHandOverDialog(IRepositoryWrapper _repository):base(nameof(HumanHandOverDialog)) { 

            repository = _repository;

        }

        public async Task<DialogTurnResult> InitialAction(WaterfallStepContext stepContext,CancellationToken token)
        {
            //this one checks if the dialog is in checking state

            //Check if the is a Notification That has been Posted 

            bool skip = true;

            while (skip)
            {
                //Check if there is a response for the last database call

               var conv=await repository.RawConversation.FindByCondition(x => x.Conversation.ConversationId =="").LastOrDefaultAsync();

                if (conv != null  && conv.IsReply)
                {
                    skip = false;

                    return await stepContext.NextAsync(null);
                }
                else
                {
                    await Task.Delay(2000); // Delay for 2 seconds (2000 milliseconds)
                }
            }

            return await stepContext.EndDialogAsync(null);
        }

    }
}
