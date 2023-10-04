using Microsoft.Bot.Builder;
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


            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InitialAction,

            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        public async Task<DialogTurnResult> InitialAction(WaterfallStepContext stepContext,CancellationToken token)
        {
            
            var conversation=await repository.Conversation.FindByCondition(x=>x.ConversationId == stepContext.Context.Activity.Conversation.Id).FirstOrDefaultAsync();


            if (!conversation.RequestedHandedOver)
            {
                repository.HandOverRequest.Create(new Entities.HandOverRequest
                {
                    Title="User 0001 : Requested Hand Over",
                    ConversationId=conversation.Id,
                    isActive=true,
                });

                conversation.RequestedHandedOver = true;

                //repository.Conversation.Update(conversation);

                await repository.SaveChangesAsync();

            }

            //this one checks if the dialog is in checking state

            //Check if the is a Notification That has been Posted 



            bool skip = true;

            int iterations = 0;

            while (skip)
            {
                //Check if there is a response for the last database call

                var request=await repository.HandOverRequest.FindByCondition(x=>x.ConversationId==conversation.Id).FirstOrDefaultAsync();

                if (request.HasBeenReceived)
                {
                    var conv = await repository.RawConversation.FindByCondition(x => x.Conversation.Id == conversation.Id).ToListAsync();

                    var lastConv = conv.Last();

                    if (lastConv != null && lastConv.HasReply)
                    {
                        skip = false;

                        return await stepContext.NextAsync(null);
                    }
                }
                await Task.Delay(2000); // Delay for 2 seconds (2000 milliseconds)

                if(iterations % 5 == 0)
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("An agent will be getting in touch with you shortly"), token);
                }


                iterations++;
            }

            return await stepContext.EndDialogAsync(null);
        }

    }
}
