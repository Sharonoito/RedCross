using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RedCrossChat.Contracts;
using RedCrossChat.Entities;
using RedCrossChat.Objects;
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

        private const string UserInfo = "Client-info";

        private readonly IStatePropertyAccessor<ResponseDto> _userProfileAccessor;

        protected readonly UserState _userState;

        public AiDialog(ILogger<AiDialog> logger, IRepositoryWrapper wrapper, UserState userState, BaseDialog baseDialog) : base(nameof(AiDialog), baseDialog,wrapper)
        {
            _logger = logger;

            _repository = wrapper;

            _userState = userState;

            _userProfileAccessor = userState.CreateProperty<ResponseDto>(DialogConstants.ProfileAssesor);

            var waterFallSteps = new WaterfallStep[]
               {
                    FirstTransactionAsync,
                    IntialTaskAsync,
                    FetchResultsAsync,
                    FinalStepAsync
               };

            AddDialog(new TextPrompt("AI_PROMPT"));

            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterFallSteps));

            InitialDialogId = nameof(WaterfallDialog);  //note : without this line of code the system will raise an Exception
        }

        public async Task<DialogTurnResult> FirstTransactionAsync(WaterfallStepContext stepContext, CancellationToken token)
        {
            Client me = (Client)stepContext.Options;


            var question = me.language ? "You are now interacting with an Generative AI-powered bot, do you wish to continue?" : "Sasa unaingiliana na kijibu kiitwacho Generative AI-powered, ungependa kuendelea?";

            Conversation conversation = await _repository.Conversation
                   .FindByCondition(x => x.ConversationId == stepContext.Context.Activity.Conversation.Id)
                   .Include(x => x.AiConversations)
                   .FirstAsync();

            var options = new PromptOptions()
            {
                Prompt=MessageFactory.Text(question),
                Choices= me.language ? RedCrossLists.choices : RedCrossLists.choicesKiswahili,
                Style = ListStyle.HeroCard
            };

            if (conversation.AiConversations.Count != 0)
            {
                return await stepContext.NextAsync(me);
            }

            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, token);
        }



        public async Task<DialogTurnResult> IntialTaskAsync(WaterfallStepContext stepContext, CancellationToken token)
        {
            Client me = (Client)stepContext.Options;

            Conversation conversation = await _repository.Conversation
                    .FindByCondition(x => x.ConversationId == stepContext.Context.Activity.Conversation.Id)
                    .Include(x => x.AiConversations)
                    .FirstAsync();

            string question = me.language ? "Ask me a question":"Niulize swali";

            if (conversation.AiConversations.Count > 0)
            {
                question = conversation.AiConversations.Last().Response;
            }
            else
            {

                switch (((FoundChoice)stepContext.Result).Value)
                {
                    case Validations.NO:
                    case ValidationsSwahili.NO:
                        return await stepContext.EndDialogAsync(); 
                }


               // await stepContext.Context.SendActivityAsync(MessageFactory.Text("You are now interacting with Chatgpt to exit or opt out type exit or cancel"));
            }

            var options = new PromptOptions();

            var promptMessage = MessageFactory.Text(question, null, InputHints.ExpectingInput);

            //  await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, question, stepContext, _userProfileAccessor, _userState);


            return await stepContext.PromptAsync("AI_PROMPT", new PromptOptions { Prompt = promptMessage }, token);
        }

        public async Task<DialogTurnResult> FetchResultsAsync(WaterfallStepContext stepContext, CancellationToken token)
        {
            var options = new PromptOptions();

            Client me = (Client)stepContext.Options;

            try
            {
                int iteration = 0;

                Conversation conversation = await _repository.Conversation
                    .FindByCondition(x => x.ConversationId == stepContext.Context.Activity.Conversation.Id)
                    .Include(x => x.AiConversations)
                    .FirstAsync();

                string question = stepContext.Context.Activity.Text;

                string response = await ChatGptDialog.GetChatGPTResponses(question, conversation.AiConversations,me.language);


                await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, response, stepContext, _userProfileAccessor, _userState);


                if (conversation != null)
                {

                    if (conversation.AiConversations.Count > 0)
                    {
                        AiConversation aiConversation = conversation.AiConversations.Last();

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

                return await stepContext.BeginDialogAsync(nameof(WaterfallDialog), me, token);
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

            HandleAiListUpdate(ref list, iteration, stepContext.Context.Activity.Text, "");

            return await stepContext.BeginDialogAsync(nameof(WaterfallDialog), list, token);
        }

        private void HandleAiListUpdate(ref Dictionary<int, List<string>> list, int iteration, string question, string response)
        {
            list[iteration] = new List<string> { question, response };

            iteration++;
        }



    }
}