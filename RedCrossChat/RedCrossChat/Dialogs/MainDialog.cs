using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RedCrossChat.Cards;
using RedCrossChat.Contracts;
using RedCrossChat.Entities;
using RedCrossChat.Objects;
using RedCrossChat.ViewModel;
using Sentry;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Attachment = Microsoft.Bot.Schema.Attachment;
using Constants = RedCrossChat.Objects.Constants;

namespace RedCrossChat.Dialogs
{
    public class MainDialog : CancelAndHelpDialog
    {
        private readonly ILogger _logger;
        private readonly FlightBookingRecognizer _luisRecognizer;
        private readonly string UserInfo = "Client-info";

        private readonly UserState _userState;
        private readonly IRepositoryWrapper _repository;
        private readonly IStatePropertyAccessor<ResponseDto> _userProfileAccessor;

        public MainDialog(
            FlightBookingRecognizer luisRecognizer,
           
            PersonalDialog personalDialog,
            AiDialog aiDialog,
            AwarenessDialog awarenessDialog,
            ILogger<MainDialog> logger, 
            IRepositoryWrapper wrapper,
            UserState userState, BaseDialog baseDialog)
            : base(nameof(MainDialog), baseDialog, wrapper)
        {
            _luisRecognizer = luisRecognizer;
            _logger = logger;

            _repository = wrapper;

            _userState = userState;

            _userProfileAccessor = userState.CreateProperty<ResponseDto>(DialogConstants.ProfileAssesor);

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            //AddDialog(counselorDialog);
            AddDialog(personalDialog);
            AddDialog(aiDialog);
            AddDialog(awarenessDialog);

            var waterfallSteps = new WaterfallStep[]
            {
                    FirstStepAsync,
                    IntroStepAsync,
                    ActStepAsync,
                    ConfirmTermsAndConditionsAsync,
                    ValidateTermsAndConditionsAsync,
                    CheckFeelingAsync,
                    EvaluateFeelingAsync,
                    HandleFeelingAsync,
                    HandleAiHandOver,
                    FinalStepAsync,
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }


        //stepContext.Context.Activity.Conversation.Id  2bf7bf00-62c5-11ee-a514-cbe89faff2c0|livechat


        private async Task<DialogTurnResult> FirstStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values[UserInfo] = new Client();

            //reason  && Context Activity Message 

            var question = "To start Select language, Kuanza Chagua lugha";

            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text(question),
                RetryPrompt = MessageFactory.Text("Please select a language, Tafadhali Chagua Lugha"),
                Choices = new List<Choice>()
                {
                    new Choice("English"),
                    new Choice("Kiswahili")
                },
                // Style = stepContext.Context.Activity.ChannelId == "facebook" ? ListStyle.SuggestedAction : ListStyle.HeroCard,
                Style = ListStyle.HeroCard,
            }, cancellationToken);


        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client client = (Client)stepContext.Values[UserInfo];

            var choiceValues = ((FoundChoice)stepContext.Result).Value;

            if (choiceValues != null && choiceValues == "Kiswahili")
            {
                client.language = !client.language;
            }

            var conv=await CreateConversationDBInstance(stepContext);

            client.ConversationId = conv.Id;

            var question = client.language ?
                "Hello dear friend!! Welcome to the Kenya Red Cross Society, we're are offering tele - mental health and counseling services to the public at no costs. How can we help you today?" :
                "Hujambo rafiki? Karibu katika Shirika la Msalaba Mwekundu ambapo tunatoa ushauri kupitia kwenye simu bila malipo yoyote. Je, ungependa nikusaidie vipi?";


            var messageText = stepContext.Options?.ToString() ?? question;

            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
            {
                Prompt = promptMessage,
                Choices = client.language ? RedCrossLists.Actions : RedCrossLists.ActionKiswahili,
                Style =  ListStyle.HeroCard,
            }, cancellationToken);

        }

   

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client client = (Client)stepContext.Values[UserInfo];         

            var choiceValues = ((FoundChoice)stepContext.Result).Value;

            if (choiceValues != null && choiceValues == "Kiswahili")
            {
                client.language = !client.language;
            }


            var message = GetAttachment(choiceValues, client.language);

            await stepContext.Context.SendActivityAsync(message, cancellationToken);

            return await stepContext.EndDialogAsync(null);

        }

        private async Task<DialogTurnResult> ConfirmTermsAndConditionsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client me = (Client)stepContext.Values[UserInfo];

            if (me.DialogClosed) { return await stepContext.EndDialogAsync(null); }
  
            var attachment = new Attachment
            {
                ContentType = HeroCard.ContentType,
                
                Content = PersonalDialogCard.GetKnowYouCard(me.language)

            };

            var message = MessageFactory.Attachment(attachment);
            
            await stepContext.Context.SendActivityAsync(message, cancellationToken);

            // Prompt the user if they agree with the terms and conditions
            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text(me.language ?"Do you agree to the Terms and Conditions? Please select 'Yes' or 'No'.": "Je, unakubali Sheria na Masharti? Tafadhali chagua 'Ndiyo' au 'La'."),
                RetryPrompt = MessageFactory.Text(me.language? "Please select a valid option ('Yes' or 'No').": "Tafadhali chagua chaguo sahihi ('Ndiyo' au 'La')"),
                Choices = me.language ? RedCrossLists.choices : RedCrossLists.choicesKiswahili,
                Style = ListStyle.HeroCard,
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);
        }

        public async Task<DialogTurnResult> ValidateTermsAndConditionsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await EvaluateDialog.ProcessStepAsync(stepContext, cancellationToken);

            Client me = (Client)stepContext.Values[UserInfo];

            string confirmation = ((FoundChoice)stepContext.Result).Value;

            if (confirmation.Equals(Validations.YES, StringComparison.OrdinalIgnoreCase) ||  confirmation.Equals(ValidationsSwahili.YES, StringComparison.OrdinalIgnoreCase))
            {
                if (me.language)
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("To exit the bot type exit or cancel at any point ."));

                return await stepContext.NextAsync(null);
  
            }
            else
            {
                // If the user does not confirm, end the dialog
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(me.language?"You need to agree to the data protection policy to proceed.": "Unahitaji kukubaliana na sera ya ulinzi wa data ili uendelee"));
              
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> CheckFeelingAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client me = (Client)stepContext.Values[UserInfo];

            Question quiz = await _repository.Question.FindByCondition(x => x.Code == 1).FirstAsync();

            ChatMessage chatMessage = new ChatMessage
            {
                QuestionId = quiz.Id,
                Type = Constants.Bot,
                ConversationId = me.ConversationId
            };

            var question = me.language ? quiz.question : quiz.Kiswahili;

            _repository.ChatMessage.Create(chatMessage);

            await _repository.SaveChangesAsync();

            var feelings = await _repository.Feeling.GetAll();

            var choices=new List<Choice>();

            foreach (var choice in feelings)
            {
                choices.Add(new Choice() { Value = me.language ? choice.Description : choice.Kiswahili });
            }

            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text(question),
                RetryPrompt = MessageFactory.Text(me.language? "Please select a valid feeling" : "Tafadhali fanya chaguo sahihi"),
                Choices =  choices,
                Style = ListStyle.HeroCard,

            };
            await DialogExtensions.UpdateDialogQuestion(question, stepContext, _userProfileAccessor, _userState);

            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);
        }

        private async Task<DialogTurnResult> EvaluateFeelingAsync(WaterfallStepContext stepContext, CancellationToken token)
        {
            Client me = (Client)stepContext.Values[UserInfo];

            var question =me.language? "Please specify the feeling "  : "Tafadhali bainisha hisia";


            //This is where the selected feeling is stored
            var response = stepContext.Context.Activity.Text;

            ChatMessage chatMessage = new()
            {
                Message = response,
                Type = Constants.User,
                ConversationId = me.ConversationId
            };

            if (response.ToLower().Trim() == "other" || response.ToLower() == "zinginezo")
            {

                _repository.ChatMessage.CreateRange(new List<ChatMessage>
                {
                    chatMessage,

                    new ChatMessage
                    {
                        Message = question,
                        Type = Constants.Bot,
                        ConversationId = me.ConversationId
                    }
                });


                await _repository.SaveChangesAsync();

                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(question) }, token);
            }
          
            Conversation conversation = await _repository.Conversation
                .FindByCondition(x => x.Id==me.ConversationId)
                .Include(x => x.Persona)
                .FirstOrDefaultAsync();
        
            if (conversation != null)
            {
                me.ConversationId = conversation.Id;

                Persona persona = conversation?.Persona;

                var feeling = await _repository.Feeling.FindByCondition(x => x.Description == response || x.Kiswahili == response).FirstOrDefaultAsync();

                persona.FeelingId = feeling.Id;


                _repository.Persona.Update(persona);
            }

            await _repository.SaveChangesAsync();

            return await stepContext.NextAsync(me);
        }



        private async Task<DialogTurnResult> HandleFeelingAsync(WaterfallStepContext stepContext,CancellationToken token)
        {
            Client me = (Client)stepContext.Values[UserInfo];

            Conversation conversation = await _repository.Conversation
                    .FindByCondition(x => x.Id==me.ConversationId)
                    .Include(x => x.Persona)
                    .FirstOrDefaultAsync();
         

            if (stepContext.Reason.ToString() =="NextCalled")
            {
                string response = stepContext.Context.Activity.Text;

                ChatMessage chatMessage = new()
                {
                    Message = response,
                    Type = Constants.User,
                    ConversationId = me.ConversationId
                };

                _repository.ChatMessage.Create(chatMessage);

                await _repository.SaveChangesAsync();

            }

            if (stepContext.Context.Activity.ChannelId == "telegram" && !conversation.IsReturnClient)
            {
                var items=await _repository.Conversation
                    .FindByCondition(x=>x.Id== me.ConversationId)
                    .ToListAsync();

                if(items.Count >1 )
                {
                    return await stepContext.BeginDialogAsync(nameof(PersonalDialog), me, token);
                }
            }

            if (conversation.IsReturnClient)
            {
                return await stepContext.BeginDialogAsync(nameof(AwarenessDialog), me, token);
            }
            else
            {
                return await stepContext.BeginDialogAsync(nameof(PersonalDialog), me, token);
            }

        }

        private async Task<DialogTurnResult> HandleAiHandOver(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client me = (Client)stepContext.Values[UserInfo];

            return await stepContext.BeginDialogAsync(nameof(AiDialog),me,cancellationToken);
        }

        private async Task<DialogTurnResult> RateBotAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client me = (Client)stepContext.Values[UserInfo];  

            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text(me.language ? "How would you rate your experience.with the bot?": "Je, unaweza kukadiria vipi uzoefu wako ? "),
                RetryPrompt = MessageFactory.Text(me.language ?  "Please select a valid option ('Yes' or 'No')." : "Tafadhali fanya chaguo sahihi"),
                Choices = RedCrossLists.GetRating(me.language),
                Style = ListStyle.HeroCard,
            };


            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
           
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        private async Task<Conversation> CreateConversationDBInstance(WaterfallStepContext stepContext)
        {
            Client me = (Client)stepContext.Values[UserInfo];

            var persona= await  _repository.Persona.FindByCondition(x => x.SenderId == stepContext.Context.Activity.From.Id).FirstOrDefaultAsync();

            Conversation conversation;

            if (persona != null)
            {
                 conversation = new()
                {
                    ChannelId = stepContext.Context.Activity.ChannelId,

                    ChannelName = stepContext.Context.Activity.ChannelId,

                    SenderId = stepContext.Context.Activity.From.Id,

                    ConversationId = stepContext.Context.Activity.Conversation.Id,

                    AiConversations = new List<AiConversation>(),

                    IsReturnClient = true,

                    Language = me.language,

                    PersonaId = persona.Id,
                };

                _repository.Conversation.Create(conversation);

            }
            else
            {
                 conversation = new()
                {
                    ChannelId = stepContext.Context.Activity.ChannelId,

                    ChannelName = stepContext.Context.Activity.ChannelId,

                    SenderId = stepContext.Context.Activity.From.Id,

                    ConversationId = stepContext.Context.Activity.Conversation.Id,

                    AiConversations = new List<AiConversation>(),

                    IsReturnClient = false,

                    Language = me.language,

                    Persona = new Persona() { 
                        SenderId = stepContext.Context.Activity.From.Id,
                        FromId = stepContext.Context.Activity.From.Id,
                        Name = stepContext.Context.Activity.From.Name 
                    }
                };

               
            }

            try
            {
                _repository.Conversation.Create(conversation);

                bool result = await _repository.SaveChangesAsync();

                if (result)
                {

                }
            }
            catch(Exception)
            {

            }
            me.ConversationId = conversation.Id;
           
            await DialogExtensions.UpdateDialogConversationId(conversation.Id, stepContext, _userProfileAccessor, _userState);

            return conversation;
        }

        private IMessageActivity GetAttachment(String choiceValue, Boolean language)
        {

            var message = PersonalDialogCard.GetKnowledgeBaseCard(language);


            switch (choiceValue)
            {
                case InitialActions.Careers:
                case InitialActionsKiswahili.Careers:
                    message = PersonalDialogCard.GetKnowledgeCareerCard(language);
                    break;
                case InitialActions.VolunteerOpportunities:
                case InitialActionsKiswahili.VolunteerOpportunities:
                    message = PersonalDialogCard.GetMembershipCard(language);
                    break;
                case InitialActions.VolunteerAndMemberShip:
                case InitialActionsKiswahili.VolunteerAndMemberShip:
                    message = PersonalDialogCard.GetKnowledgeBaseCard(language);
                    break;

            }


            return MessageFactory.Attachment(new Attachment { ContentType = HeroCard.ContentType, Content = message });
        }

    }
}



 