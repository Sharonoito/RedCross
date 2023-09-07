using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using RedCrossChat.Contracts;
using RedCrossChat.Entities;
using Sentry.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RedCrossChat.Dialogs
{
    public class AiDialog : CancelAndHelpDialog
    {
        protected readonly ILogger _logger;

        private readonly IRepositoryWrapper _repository;

        protected readonly string _list = "conversation_list";

        protected readonly string _iteration = "iteration";


        public AiDialog(ILogger<AiDialog> logger, IRepositoryWrapper wrapper) : base(nameof(AiDialog))
        {
            _logger = logger;

            _repository = wrapper;

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

            Conversation conversation = await _repository.Conversation
                    .FindByCondition(x => x.ConversationId == stepContext.Context.Activity.Conversation.Id)
                    .Include(x=>x.AiConversations)
                    .FirstAsync();

            string question = "Ask me a question";
            
            if(conversation.AiConversations.Count > 0)
            {
                question=conversation.AiConversations.Last().Response;
            }

            var options=new PromptOptions();

            var promptMessage = MessageFactory.Text(question, null, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, token);
        }

        public async Task<DialogTurnResult>  FetchResultsAsync(WaterfallStepContext stepContext, CancellationToken token)
        {
            var options = new PromptOptions();

            try
            {
                int iteration = 0;

                Conversation conversation = await _repository.Conversation
                    .FindByCondition(x => x.ConversationId == stepContext.Context.Activity.Conversation.Id)
                    .Include(x=>x.AiConversations)
                    .FirstAsync();

                string question = stepContext.Context.Activity.Text;

                string response = await ChatGptDialog.GetChatGPTResponses(question,conversation.AiConversations);


                if (conversation != null)
                {

                    if(conversation.AiConversations.Count > 0)
                    {
                        AiConversation aiConversation= conversation.AiConversations.Last();

                        iteration = conversation.AiConversations.Last().Iteration;
                    }

                    _repository.AiConversation.Create(new AiConversation()
                    {
                        Iteration = iteration + 1,
                        Question = question,
                        Response = response,
                   
                        ConversationId = conversation.Id
                    });

                    await _repository.SaveChangesAsync();
                }


                //await stepContext.Context.SendActivityAsync(MessageFactory.Text(response));

                return await stepContext.BeginDialogAsync(nameof(WaterfallDialog), null, token);
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

            var list = stepContext.Values[_list] as Dictionary<int, List<string>>;

            int iteration = (int)stepContext.Values[_iteration];

            HandleAiListUpdate(ref list, iteration, stepContext.Context.Activity.Text,"");

            return await stepContext.BeginDialogAsync(nameof(WaterfallDialog), list, token);
        }

        private void HandleAiListUpdate(ref Dictionary<int, List<string>> list, int iteration,string question,string response)
        {
            list[iteration] = new List<string> { question, response };

            iteration++;
        }

      

    }
}
