using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.EntityFrameworkCore;
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
        private readonly string UserInfo = "Client-info";

        private readonly IRepositoryWrapper _repository;

          private readonly UserState _userState;
        private readonly IStatePropertyAccessor<ResponseDto> _userProfileAccessor;

        public BaseDialog(IRepositoryWrapper repository,UserState userState):base(nameof(BaseDialog)) {
        
            _repository = repository;

            _userState = userState;

            _userProfileAccessor = userState.CreateProperty<ResponseDto>(DialogConstants.ProfileAssesor);

            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));


            AddDialog(new WaterfallDialog(nameof(WaterfallDialog),new WaterfallStep[]
            {
                InitialStepAsync,
               // HandleChoiceResultAsync,
                CheckFeedBackAsync,
                EvaluateFeedbackAsync,
                EndConversationAsync,
            }));

            InitialDialogId=nameof(WaterfallDialog);
        }

        //check if the user is using facebook channel 

       /* public async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken token) 
        {
            if (stepContext.Context.Activity.ChannelId == "facebook")
            {
                var question = "Do you want to talk to an agent?";

                var options = new PromptOptions()
                {
                    Prompt=MessageFactory.Text(question),
                    RetryPrompt = MessageFactory.Text("Please select a valid option ('Yes' or 'No')."),
                    Choices = RedCrossLists.choices ,
                   // Style = ListStyle.HeroCard
                };
                return await stepContext.PromptAsync(nameof(ChoicePrompt), options,token);
            }
            else
            {
                var me = new Client();
                return await stepContext.BeginDialogAsync(nameof(MainDialog), me, token);
            }
        }*/


        public async Task<DialogTurnResult> HandleChoiceResultAsync(WaterfallStepContext stepContext, CancellationToken token)
        {
            var choiceValues = ((FoundChoice)stepContext.Result).Value;

            if (choiceValues != null && choiceValues == "Yes")
            {
                return await stepContext.BeginDialogAsync(nameof(HumanHandOverDialog));
            }
            else
            {
                return await stepContext.EndDialogAsync(null);
            }

        }



        public async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext,CancellationToken token)
        {
            var me = new Client();



            if (stepContext.Reason.ToString() == "BeginCalled")
            {
                return await stepContext.NextAsync(null);
            }
            else
            {
                return await stepContext.BeginDialogAsync(nameof(MainDialog), me, token);
            }
            
        }


        public async Task<DialogTurnResult> CheckFeedBackAsync(WaterfallStepContext stepContext, CancellationToken token)
        {

            var conversations = await _repository.Conversation
                    .FindByCondition(x => x.ConversationId == stepContext.Context.Activity.Conversation.Id).ToListAsync();

            var conversation = conversations.Last();

            Client me = new Client()
            {
                language = conversation.Language,
                ConversationId = conversation.Id,
            };

            stepContext.Values[UserInfo] =me;


            if (conversation.IsReturnClient  && conversations.Count % 10 !=0)
            {
                //ask chat gpt for an encouraging quote
                //todo : make the goodbye dynamic 
                
                var resp=await ChatGptDialog.GetChatGPTResponses("Give me a random encouraging quote",new List<AiConversation>(),conversation.Language);

                await stepContext.Context.SendActivityAsync(MessageFactory.Text(conversation.Language ? $"Thank you , have a lovely day  {resp}" : $" Asante,kuwa na siku njema  {resp}"));

                return await stepContext.EndDialogAsync(null);
            }

            string question = conversation.Language ? "How would you rate your experience.with the bot?" : "Je, unaweza kukadiria vipi uzoefu wako ? ";

            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text(question),
                RetryPrompt = MessageFactory.Text(conversation.Language ? "Please select a valid option ('Yes' or 'No')." : "Tafadhali fanya chaguo sahihi"),
                Choices = RedCrossLists.GetRating(conversation.Language),
                Style = ListStyle.HeroCard,
            };

            await DialogExtensions.UpdateDialogAnswer("", question, stepContext, _userProfileAccessor, _userState);

            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, token);
        }

        public async Task<DialogTurnResult> EvaluateFeedbackAsync(WaterfallStepContext stepContext, CancellationToken token)
        {
            Client me = (Client)stepContext.Values[UserInfo];

            string question = me.language ? "please give us a reason why so that we can improve your experience ": "tafadhali tupe sababu kwa nini ili tuweze kuboresha matumizi yako";


            await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, question, stepContext, _userProfileAccessor, _userState);

            if(stepContext.Context.Activity.Text.ToLower()  == "excellent")
            {
                return await stepContext.NextAsync(null);
            }


            return await stepContext.PromptAsync(nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text(question),
                },token);


        }


        public async Task<DialogTurnResult> EndConversationAsync(WaterfallStepContext stepContext, CancellationToken token)
        {
            Client me = (Client)stepContext.Values[UserInfo];

           var feedbackResponse = stepContext.Result.ToString();


            var resp = await ChatGptDialog.GetChatGPTResponses("Give me a random encouraging quote", new List<AiConversation>(), me.language);


            var conversations = await _repository.Conversation
               .FindByCondition(x => x.Id == me.ConversationId)

               .FirstOrDefaultAsync();

            conversations.RatingReason = feedbackResponse;

            _repository.Conversation.Update(conversations);

            await _repository.SaveChangesAsync();



            string question = me.language
                ? $"Thank you for your feedback. We value your input! {resp}"
                : $"Asante kwa maoni yako. Tunathamini mchango wako {resp}";

            //await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, "Feedback", stepContext, _userProfileAccessor, _userState);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(question));

            return await stepContext.EndDialogAsync(null);
        }

    }
}
