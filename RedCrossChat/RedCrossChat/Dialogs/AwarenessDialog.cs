using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using NuGet.Protocol.Core.Types;
using RedCrossChat.Cards;
using RedCrossChat.Contracts;
using RedCrossChat.Entities;
using RedCrossChat.Objects;
using RedCrossChat.ViewModel;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace RedCrossChat.Dialogs
{
    public class AwarenessDialog : CancelAndHelpDialog
    {
        protected readonly ILogger _logger;

        private readonly IRepositoryWrapper _repository;

        private readonly IStatePropertyAccessor<ResponseDto> _userProfileAccessor;

        protected readonly UserState _userState;

        private const string UserInfo = "user-info";

        protected string CurrentQuestion = "CurrentQuestion";

        public AwarenessDialog(
            ILogger<AwarenessDialog> logger, 
            IRepositoryWrapper wrapper, UserState userState,
            HumanHandOverDialog humanHandOverDialog,
            BaseDialog baseDialog,
            BreathingDialog breathingDialog
            ) : base(nameof(AwarenessDialog), baseDialog,wrapper)
        {

            _repository = wrapper;

            _userState = userState;

            _userProfileAccessor = userState.CreateProperty<ResponseDto>(DialogConstants.ProfileAssesor);

            AddDialog( breathingDialog );

            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            AddDialog(new ChoicePrompt("select-choice"));

            AddDialog(new ChoicePrompt("select-option"));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), CreateWaterFallSteps()));

            AddDialog(humanHandOverDialog);
   
            InitialDialogId = nameof(WaterfallDialog);
        }

        private WaterfallStep[] CreateWaterFallSteps()
        {
            return new WaterfallStep[]
            {
                InitialStepAsync,
                
                ProcessMentalEvaluationChoice,
                HandleCaregiverChoiceAsync,
                EvaluateDialogTurnAsync,
                CheckFeelingAware,
                ValidateFeeling,
                //CheckProfessionalSwitchAsync,
                FinalStepAsync
            };
        }


        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var me = (Client)stepContext.Options;

     //       var savedFeeling = (Feeling)stepContext.Values["Feeling"];

            stepContext.Values[UserInfo] = me;


            Conversation conversation = await getConversation(me);



            Question quiz = await _repository.Question.FindByCondition(x => x.Code == 4).FirstAsync();
          
            var question = me.language ? quiz.question : quiz.Kiswahili;

            DBFeeling feeling = conversation.Feeling;

            if (feeling.Description.ToLower().Trim() == "other" || feeling.Description.ToLower().Trim() == "others")
            {
                question = (me.language ? "You said you are feeling " + conversation.FeelingDetail :
                                                    "Ulisema " + conversation.FeelingDetail) + ", " + question;
            }else
            {
                question = (me.language ? "You said you are feeling " + feeling.Description :
                                      "Ulisema " + feeling.Kiswahili) + ", " + question;
            }
            
            _repository.ChatMessage.Create(new ChatMessage
            {
                QuestionId = quiz.Id,
                Type = Constants.Bot,
                ConversationId = me.ConversationId
            });

            await _repository.SaveChangesAsync();

            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text(question),

                Choices = me.language ? RedCrossLists.choices : RedCrossLists.choicesKiswahili,
                Style=ListStyle.HeroCard

            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);

        }
        //var storedFeeling = (Feeling)stepContext.Values["Feeling"];
        private async Task<DialogTurnResult> ProcessMentalEvaluationChoice(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client me = (Client)stepContext.Values[UserInfo];

            Conversation conversation = await getConversation(me);

            Question quiz = await _repository.Question.FindByCondition(x => x.Code == 14).FirstAsync();

            var response = stepContext.Context.Activity.Text;

            var question = me.language ? quiz.question : quiz.Kiswahili;

            var chat = new ChatMessage
            {
                Message = response,
                Type = Constants.User,
                ConversationId = conversation.Id,
            };

            var list = new List<ChatMessage>() { chat};


            if (stepContext.Result != null && stepContext.Result is FoundChoice choiceResult)
            {

                Persona persona = conversation.Persona;

                persona.IsAwareOfFeelings = stepContext.Context.Activity.Text;

                _repository.Persona.Update(persona);

                await _repository.SaveChangesAsync();

                switch (choiceResult.Value)
                {
                    case Validations.YES:
                    case ValidationsSwahili.YES:
                        me.IsAwareOfFeeling = true;

                        break;
                    default:
                        quiz = await _repository.Question.FindByCondition(x => x.Code == 51).FirstAsync();

                        question =me.language? quiz.question: quiz.Kiswahili ;

                        break;   
                }

                list.Add(new ChatMessage
                {
                    QuestionId = quiz.Id,
                    Type = Constants.Bot,
                    ConversationId = conversation.Id,
                });

                _repository.ChatMessage.CreateRange(list);

                await _repository.SaveChangesAsync();
              

                return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
                            {
                                Prompt = MessageFactory.Text(question),
                                Choices = me.language ? RedCrossLists.choices : RedCrossLists.choicesKiswahili,
                                Style = ListStyle.HeroCard
                },
                            cancellationToken);
            }
            else
            {
                // Handle the case where stepContext.Result is null or not of the correct type.
                // For example, you can prompt the user to repeat their response or handle the case accordingly.

                _repository.ChatMessage.CreateRange(list);

                await _repository.SaveChangesAsync();

                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thank you for contacting us"), cancellationToken);

                return await stepContext.EndDialogAsync(new DialogTurnResult(DialogTurnStatus.Waiting), cancellationToken);
            }
        }


        private async Task<DialogTurnResult> HandleCaregiverChoiceAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            Client me = (Client)stepContext.Values[UserInfo];

            Conversation conversation = await getConversation(me);

            Question quiz = await _repository.Question.FindByCondition(x => x.Code == 5).FirstAsync();

            var response = stepContext.Context.Activity.Text;

            var question = me.language ? quiz.question : quiz.Kiswahili;

            Persona persona = conversation.Persona;

            persona.HasTalkedTOSomeone = true;

            _repository.Persona.Update(persona);

            var chat = new ChatMessage
            {
                Message = response,
                Type = Constants.User,
                ConversationId = conversation.Id,
            };

            var list = new List<ChatMessage>() { chat };

            switch (((FoundChoice)stepContext.Result).Value)
            {
                case "Yes":
                    me.HasTalkedToSomeone =true;
                    break;
                default:

                    if (!me.HasTalkedToSomeone && !me.IsAwareOfFeeling)
                    {
                        _repository.ChatMessage.CreateRange(list);

                        await _repository.SaveChangesAsync();

                        return await stepContext.EndDialogAsync(me);
                    }
                    break;
                    

            }

            list.Add(new ChatMessage
            {
                QuestionId = quiz.Id,
                Type = Constants.Bot,
                ConversationId = conversation.Id,
            });

            _repository.ChatMessage.CreateRange(list);

            await _repository.SaveChangesAsync();

            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                               new PromptOptions()
                               {
                                   Prompt = MessageFactory.Text(question),
                                   Choices = me.language? RedCrossLists.choices : RedCrossLists.choicesKiswahili,
                                   Style = ListStyle.HeroCard

                               });
        }


        private async Task<DialogTurnResult> EvaluateDialogTurnAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            Client me = (Client)stepContext.Values[UserInfo];

            Conversation conversation = await getConversation(me);

            Persona persona = conversation.Persona;
           
            var response = stepContext.Context.Activity.Text;

            var chat = new ChatMessage
            {
                Message = response,
                Type = Constants.User,
                ConversationId = conversation.Id,
            };

            _repository.ChatMessage.Create(chat);

           

            if (stepContext.Result == null)
            {
                persona.WantsToTalkToSomeone = true;

                _repository.Persona.Update(persona);


                await _repository.SaveChangesAsync();

                return await stepContext.EndDialogAsync(me, cancellationToken);
            }
            await _repository.SaveChangesAsync();

            switch (((FoundChoice)stepContext.Result).Value)
            {
                case Validations.YES:case ValidationsSwahili.YES:
                    return await stepContext.NextAsync(me, cancellationToken);
                default:
                    me.WantsBreathingExercises = true;
                    return await stepContext.EndDialogAsync(me, cancellationToken);
            }
        }

        

        public async Task<DialogTurnResult> CheckFeelingAware(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client me = (Client)stepContext.Values[UserInfo];

            Question quiz = await _repository.Question.FindByCondition(x => x.Code == 15).FirstAsync();

            var question = me.language ? quiz.question : quiz.Kiswahili;

            Conversation conversation = await getConversation(me);

            Persona persona = conversation.Persona;

            persona.WantsBreathingExcercises = true;

            _repository.Persona.Update(persona);

            var chat = new ChatMessage
            {
                QuestionId = quiz.Id,
                Type = Constants.Bot,
                ConversationId = conversation.Id,
            };

            _repository.ChatMessage.Create(chat);

            await _repository.SaveChangesAsync();

            var intentions=await _repository.Itention.GetAllAsync();

            var list =new List<Choice>();

            foreach (var choice in intentions)
            {
                list.Add(new Choice{Value = me.language? choice.Name :choice.Kiswahili});
            }
        
            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text(question),
                Choices =list,
                Style = ListStyle.HeroCard
            }, cancellationToken);
        }

        public async Task<DialogTurnResult> ValidateFeeling(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client me = (Client)stepContext.Values[UserInfo];

            string response = stepContext.Context.Activity.Text;

            var intention=await _repository.Itention.FindByCondition(x=>x.Name==response || x.Kiswahili==response).Include(x=>x.SubIntentions).FirstOrDefaultAsync();

            Conversation conv = await getConversation(me);

            conv.IntentionId=intention.Id;

            _repository.Conversation.Update(conv);


            if (intention.SubIntentions.Count == 0)
            {
                return await stepContext.NextAsync(null);
            }

            List<SubIntention> subIntentions=await _repository.SubIntention.FindByCondition(x=>x.IntentionId==intention.Id).ToListAsync();

            List<Choice> choices=new List<Choice>();

            foreach(var subIntention in subIntentions)
            {
                choices.Add(new Choice { Value = me.language ? subIntention.Name : subIntention.Kiswahili });
            }

            string question = me.language ?"Please Specify ": "Tafadhali fafanua";

            
            _repository.ChatMessage.CreateRange(new List<ChatMessage>
            {
                new ChatMessage
                {
                    Message = response,
                    Type = Constants.User,
                    ConversationId = conv.Id,
                },
                new ChatMessage
                {
                    Message = question,
                    Type = Constants.Bot,
                    ConversationId = conv.Id,
                }
            });

            await _repository.SaveChangesAsync();

            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text(question),
                Choices = choices,
                Style = ListStyle.HeroCard
            }, cancellationToken);

        }
        private async Task<Conversation> getConversation(Client me)
        {
            return await _repository.Conversation.FindByCondition(x => x.Id == me.ConversationId)
                .Include(x => x.Persona)
                .Include(x=>x.Feeling).FirstAsync();
        }
        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client me = (Client)stepContext.Values[UserInfo];

            Conversation conversation = await getConversation(me);

            string response = stepContext.Context.Activity.Text;

            _repository.ChatMessage.Create(new ChatMessage
            {
                Message = response,
                Type = Constants.User,
                ConversationId = conversation.Id,
            });


            if (stepContext.Result != null)
            {
                SubIntention subIntention = await _repository.SubIntention.FindByCondition(x => x.Name == response || x.Kiswahili == response).FirstOrDefaultAsync();

                if (subIntention != null)
                {

                    conversation.SubIntentionId = subIntention.Id;

                    _repository.Conversation.Update(conversation);
                    
                }
            }

            await _repository.SaveChangesAsync();

            var agentMessage = me.language ? "The next available psychologist will get in touch with you shortly, you can also contact us directly by dialing 1199, request to speak to a psychologist." :
                             "Utaweza kuzungumza na mhudumu baada ya muda mfupi ama pia unaweza piga nambari 1199 ili kuongea na mshauri. Utaweza kupigiwa na mshauri baada ya muda mfupi, ama upige simu ili kuongea na mwanasaikolojia kupitia nambari 1199";
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(agentMessage), cancellationToken);


            var attachment = new Attachment
            {
                ContentType = HeroCard.ContentType,

                Content = PersonalDialogCard.GetHotlineCard(me.language),
            };


            var message = MessageFactory.Attachment(attachment);
            await stepContext.Context.SendActivityAsync(message, cancellationToken);

            var choices = new List<Choice>
            {
                new Choice() { Value = "hotline", Action = new CardAction() { 
                    Title = "hotline", Type = ActionTypes.OpenUrl, 
                    Value = "https://referraldirectories.redcross.or.ke/" } 
                }
            };

            return await stepContext.BeginDialogAsync(nameof(HumanHandOverDialog), me, cancellationToken);
        }


}
}




