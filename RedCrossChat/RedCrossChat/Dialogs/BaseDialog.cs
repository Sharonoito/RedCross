using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using NuGet.Protocol.Core.Types;
using RedCrossChat.Contracts;
using RedCrossChat.Entities;
using RedCrossChat.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RedCrossChat.Dialogs
{
    public class BaseDialog :ComponentDialog
    {
        private readonly string UserInfo = "Clien-info";

        private readonly IRepositoryWrapper _repository;

        public BaseDialog(IRepositoryWrapper repository):base(nameof(BaseDialog)) {
        
            _repository = repository;

            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));


            AddDialog(new WaterfallDialog(nameof(WaterfallDialog),new WaterfallStep[]
            {
                
                CheckFeedBackAsync,
                EndConversationAsync,
            }));

            InitialDialogId=nameof(WaterfallDialog);
        }

        public async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext,CancellationToken token)
        {
            var me = new Client();

            return await stepContext.BeginDialogAsync(nameof(MainDialog), me, token);
        }


        public async Task<DialogTurnResult> CheckFeedBackAsync(WaterfallStepContext stepContext, CancellationToken token)
        {

            var conversations = await _repository.Conversation
                    .FindByCondition(x => x.ConversationId == stepContext.Context.Activity.Conversation.Id).ToListAsync();

            var conversation = conversations.Last();

            stepContext.Values[UserInfo] = new Client()
            {
                language = conversation.Language,
            };


            if (conversation.IsReturnClient  && conversations.Count % 10 !=0)
            {
                //ask chat gpt for an encouraging quote
                //todo : make the goodbye dynamic 
                
                var resp=await ChatGptDialog.GetChatGPTResponses("Give me a random encouraging quote",new List<AiConversation>(),conversation.Language);

                await stepContext.Context.SendActivityAsync(MessageFactory.Text(conversation.Language ? $"Thank you , have a lovely day  {resp}" : $" Asante,kuwa na siku njema  {resp}"));

                return await stepContext.EndDialogAsync(null);
            }

            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text(conversation.Language ? "How would you rate your experience.with the bot?" : "Je, unaweza kukadiria vipi uzoefu wako ? "),
                RetryPrompt = MessageFactory.Text(conversation.Language ? "Please select a valid option ('Yes' or 'No')." : "Tafadhali fanya chaguo sahihi"),
                Choices = RedCrossLists.GetRating(conversation.Language),
                Style = ListStyle.HeroCard,
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, token);
        }

        public async Task<DialogTurnResult> EndConversationAsync(WaterfallStepContext stepContext, CancellationToken token)
        {
            var ratingChoice = ((FoundChoice)stepContext.Result).Value;

            Client me = (Client)stepContext.Values[UserInfo];

            var resp = await ChatGptDialog.GetChatGPTResponses("Give me a random encouraging quote", new List<AiConversation>(), me.language);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(me.language ? $"Thank you for your feedback. We value your input! {resp}" : $" Asante kwa maoni yako. Tunathamini mchango wako {resp}"));


            return await stepContext.EndDialogAsync(null);
        }

        
    }
}
