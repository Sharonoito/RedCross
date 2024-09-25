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
            
            Client me= (Client)stepContext.Options;

            me.HandOverToUser = me.Iteration != 0;

            var conversation= await repository.Conversation
                .FindByCondition(x => x.Id==me.ConversationId)
                .Include(x => x.Persona)
                .FirstOrDefaultAsync();

            if (!me.HandOverToUser)
            {

                var requests=await repository.HandOverRequest.FindByCondition(x => x.ConversationId == me.ConversationId & x.HasBeenReceived==false).ToListAsync();

                if(requests.Count > 0)
                {

                    foreach(var request in requests)
                    {
                        request.HasBeenReceived = true;
                    }

                    repository.HandOverRequest.UpdateRange(requests);
                }

                repository.HandOverRequest.Create(new Entities.HandOverRequest
                {
                    Title = conversation.Persona.CodeName, // we has an error here using the Persona.Name;
                    ConversationId = conversation.Id,
                    isActive = true,
                });

                conversation.RequestedHandedOver = true;

                await repository.SaveChangesAsync();
            }
            bool skip = true;

            int iterations = 0;

            while (skip)
            {
                //Check if there is a response for the last database call

                var request=await repository.HandOverRequest.FindByCondition(x=>x.ConversationId==conversation.Id)
                    .Include(x=>x.LastChatMessage)
                    .FirstOrDefaultAsync();

                var chatMessages = await repository.ChatMessage.FindByCondition(x => x.ConversationId == conversation.Id & x.IsRead != true ).ToListAsync();

                if (request.HasBeenReceived)
                {
                    me.HandOverToUser = true;

                    if (request.HasResponse && request.LastChatMessage !=null)
                    {
                        me.HandOverToUser = true;

                        me.ActiveRawConversation = request.LastChatMessage.Id;

                        var promptMessage = MessageFactory.Text("", null, InputHints.ExpectingInput);

                        if (chatMessages == null)
                        {
                            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, token);
                        }

                        //chatMessages.Last().IsRead = true;

                       // repository.ChatMessage.Update(chatMessages.Last());

                        await repository.SaveChangesAsync();

                        // repository.ChatMessage.UpdateRange(chatMessages);

                        // await repository.SaveChangesAsync();

                        //todo make a way of presenting all messages incase you have more than one unread message

                        promptMessage = MessageFactory.Text(chatMessages.Last().Message, null, InputHints.ExpectingInput);

                        /*if (request.LastChatMessage.Message ==me.LastMessage)
                        {
                            
                        }
                        else
                        {
                            me.LastMessage = request.LastChatMessage.Message;

                        }
                       
                        if(skip == false)
                        {
                            promptMessage = MessageFactory.Text("", null, InputHints.ExpectingInput);
                        }

                        skip = false;*/

                        return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, token);
                    }                    
                }

                int delay = request.HasBeenReceived ? 1000 : 10000;

                await Task.Delay(delay); // Delay for 2 seconds (2000 milliseconds)

                if(iterations % 100 == 0 && !me.HandOverToUser  && iterations !=30)
                {
                    var message = me.language ? "One of our psychologists  will be getting in touch with you shortly" : "Mmoja wa wanasaikolojia wetu atawasiliana nawe hivi karibuni";
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(message), token);
                }


                if (iterations  >=  300 && !me.HandOverToUser)
                {

                    request = await repository.HandOverRequest.FindByCondition(x => x.ConversationId == conversation.Id).FirstOrDefaultAsync();

                    if(request != null)
                    {
                        request.isActive= false;

                        request.HasBeenReceived = true;

                        repository.HandOverRequest.Update(request);

                        await repository.SaveChangesAsync();

                    }

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("it seems we are facing a number of requests, let's connect later"), token);


                    var agentMessage = me.language ? " you can  contact us directly by dialing 1199, request to speak to a psychologist." :
                            "pia unaweza piga nambari 1199 ili kuongea na mshauri. Utaweza kupigiwa na mshauri baada ya muda mfupi, ama upige simu ili kuongea na mwanasaikolojia kupitia nambari 1199";
                    
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(agentMessage), token);


                   // string question = me.language ? "please give us a reason why so that we can improve your experience " : "tafadhali tupe sababu kwa nini ili tuweze kuboresha matumizi yako";

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

            ChatMessage chatMessage = new ChatMessage
            {
                ConversationId = me.ConversationId,

                Message = stepContext.Result.ToString(),

                Type=Constants.User

            };

            repository.ChatMessage.Create(chatMessage);

            var request = await repository.HandOverRequest
                .FindByCondition(x => x.ConversationId == me.ConversationId)
                .FirstOrDefaultAsync();

            if (request != null)
            {
                request.HasResponse = false;
            }

            repository.HandOverRequest.Update(request);

            bool result = await repository.SaveChangesAsync();

            //return await InitialAction(stepContext, token);
            return await stepContext.BeginDialogAsync(nameof(WaterfallDialog), me, token);
        }

        public async Task<Conversation> GetActiveConversation(WaterfallStepContext stepContext)
        {
            return await repository.Conversation.FindByCondition(x => x.ConversationId == stepContext.Context.Activity.Conversation.Id).FirstOrDefaultAsync();
        }
    }
}
