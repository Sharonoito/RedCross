using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using RedCrossChat.Contracts;
using RedCrossChat.Entities;
using RedCrossChat.Objects;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RedCrossChat.Dialogs
{
    public class HumanHandOverDialog : ComponentDialog
    {
        private IRepositoryWrapper repository;

        public HumanHandOverDialog(IRepositoryWrapper _repository, BaseDialog baseDialog):base(nameof(HumanHandOverDialog)) { 

            repository = _repository;


            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InitialAction,
                EvaluateClientQuestion,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

       

        public async Task<DialogTurnResult> InitialAction(WaterfallStepContext stepContext,CancellationToken token)
        {
            //Client me = (Client)stepContext.Values[UserInfo];
            Client me= (Client)stepContext.Options;

            me.HandOverToUser = me.Iteration != 0;

            var conversation= await repository.Conversation.FindByCondition(x => x.Id==me.ConversationId).FirstOrDefaultAsync();

            if (!me.HandOverToUser)
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

                        me.HandOverToUser = true;

                        me.ActiveRawConversation = lastConv.Id;

                        var promptMessage = MessageFactory.Text(lastConv.Question, null, InputHints.ExpectingInput);

                        return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, token);
                    }
                }

                await Task.Delay(request.HasBeenReceived ? 1000  : 10000); // Delay for 2 seconds (2000 milliseconds)

                if(iterations % 10 == 0 && !me.HandOverToUser  && iterations !=30)
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("An agent will be getting in touch with you shortly"), token);
                }

                if(iterations  >=  30)
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("it seems we are facing a number of requests, let's connect later"), token);

                    return await stepContext.EndDialogAsync(null);
                }


                iterations++;
            }

            return await stepContext.EndDialogAsync(null);
        }


        public async Task<DialogTurnResult> EvaluateClientQuestion(WaterfallStepContext stepContext,CancellationToken token)
        {
            //save the response 

            Client me = (Client)stepContext.Options;

            me.LastResponse= stepContext.Result.ToString();

            me.Iteration++;

            var lastConv= await repository.RawConversation.FindByCondition(x=>x.Id==me.ActiveRawConversation).FirstOrDefaultAsync();

            if(lastConv != null && lastConv.HasReply)
            {
                lastConv.Message=me.LastResponse.ToString();

                lastConv.IsReply = true;

                lastConv.HasReply= false;

                repository.RawConversation.Update(lastConv);

                bool result=await repository.SaveChangesAsync();
            }


            return await stepContext.BeginDialogAsync(nameof(WaterfallDialog), me, token);
        }

        public async Task<Conversation> GetActiveConversation(WaterfallStepContext stepContext)
        {
            return await repository.Conversation.FindByCondition(x => x.ConversationId == stepContext.Context.Activity.Conversation.Id).FirstOrDefaultAsync();
        }
    }
}
