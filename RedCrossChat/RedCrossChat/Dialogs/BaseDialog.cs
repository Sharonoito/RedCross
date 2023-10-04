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
            stepContext.Values[UserInfo] = new Client();

            return await stepContext.BeginDialogAsync(nameof(MainDialog), null, token);
        }


        public async Task<DialogTurnResult> CheckFeedBackAsync(WaterfallStepContext stepContext, CancellationToken token)
        {
            Conversation conversation = await _repository.Conversation
                    .FindByCondition(x => x.ConversationId == stepContext.Context.Activity.Conversation.Id).FirstOrDefaultAsync();

            stepContext.Values[UserInfo] = new Client()
            {
                language = conversation.Language,
            };

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

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(me.language ? "Thank you for your feedback. We value your input!" : " Asante kwa maoni yako. Tunathamini mchango wako"));


            return await stepContext.EndDialogAsync(null,token);
        }
    }
}
