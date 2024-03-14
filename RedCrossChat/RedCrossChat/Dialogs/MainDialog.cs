using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Connector;
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
using System.Linq;
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
            BreathingDialog breathingDialog,
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
            AddDialog(breathingDialog);

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
                    //HandleBreathingStop,
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

            var conv = await CreateConversationDBInstance(stepContext);

            client.ConversationId = conv.Id;

            var question = client.language ?

               //"Hello dear friend!! Welcome to Kenya Red Cross Society, we're are offering tele - mental health and counseling services to the public at no costs. How can we help you today?" :
               "Hello Welcome to Kenya Red Cross Society. How can I help you today?" :
               "Habari rafiki mpendwa, karibu kwenye jukwaa la chatbot la Shirika la Msalaba Mwekundu Kenya. Tunatoa huduma ya afya ya akili na ushauri kupitia simu kwa umma bila malipo yoyote. Leo ungependa tukusaidie vipi?";

            var attachment = new Attachment
            {
                ContentType = HeroCard.ContentType,

                Content = PersonalDialogCard.GetFAQCard(client.language),
            };

            var message = MessageFactory.Attachment(attachment);

            await stepContext.Context.SendActivityAsync(message, cancellationToken);

            var choicez = new List<Choice>
            {
                new Choice() { Value = "faq", Action = new CardAction() {
                    Title = "faq", Type = ActionTypes.OpenUrl,
                    Value = "https://referraldirectories.redcross.or.ke/" }
                }
            };


            var messageText = stepContext.Options?.ToString() ?? question;

            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);



            var actions = await _repository.IntroductionChoice.GetAll();

            var choices = new List<Choice>();

            foreach (var choice in actions)
            {
                choices.Add(new Choice() { Value = client.language ? choice.Name : choice.Kiswahili });
            }

            var options = new PromptOptions()
            {
                Prompt = promptMessage,
                Choices = choices,
                Style = ListStyle.HeroCard,

            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);

        }


        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client client = (Client)stepContext.Values[UserInfo];

            var choiceValues = ((FoundChoice)stepContext.Result).Value;

            if (choiceValues.ToString().ToLower().Trim() == InitialActions.MentalHealth.ToString().ToLower().Trim() || choiceValues == InitialActionsKiswahili.MentalHealth)
            {
                return await stepContext.NextAsync(client);
            }

            var message = await GetAttachment(choiceValues, client.language);

            await stepContext.Context.SendActivityAsync(message, cancellationToken);

            client.DialogClosed = true;

            var question = client.language ? "Do you want to continue or exit?" : "Je, ungependa kuendelea au kuondoka?";

            var continueChoice = client.language ? "Continue" : "Endelea";
            var continueExit = client.language ? "Exit" : "Ondoka";

            var options = new PromptOptions

            {
                Prompt = MessageFactory.Text(question),
                Choices = new List<Choice>
                {
                    new Choice { Value = continueChoice },
                    new Choice { Value = continueExit }
                },
                Style = ListStyle.HeroCard,
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);

        }


        private async Task<DialogTurnResult> ConfirmTermsAndConditionsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client me = (Client)stepContext.Values[UserInfo];

            if (me.DialogClosed)
            {
                var clientChoice = ((FoundChoice)stepContext.Result)?.Value;

                if (clientChoice == "Continue" || clientChoice == "Endelea")
                {
                    return await IntroStepAsync(stepContext, cancellationToken);

                }
                else
                {
                    if (clientChoice == "Exit" || clientChoice == "Ondoka")
                    {
                        return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                    }
                }


                return await stepContext.EndDialogAsync(null); 

            }
  
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
                Prompt = MessageFactory.Text(me.language ?"Do you agree to the Terms and Conditions? Please select 'Yes' or 'No'.": "Je, unakubali Sheria na Masharti? Tafadhali chagua 'Ndio' au 'La'."),
                RetryPrompt = MessageFactory.Text(me.language? "Please select a valid option ('Yes' or 'No').": "Tafadhali chagua chaguo sahihi ('Ndio' au 'La')"),
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

            if (confirmation.Equals(Validations.YES, StringComparison.OrdinalIgnoreCase) || confirmation.Equals(ValidationsSwahili.YES, StringComparison.OrdinalIgnoreCase))
            {
                    string message = me.language ? 
                        "**🔴To exit the bot type exit or cancel at any point 🔴**" :
                        "**🔴Ili kuondoka kwenye aina ya roboti ondoka au ghairi wakati wowote 🔴**";

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(message), cancellationToken);

                return await stepContext.NextAsync(null);
            }
            else
            {
                // If the user does not confirm, end the dialog
                string messageText = me.language ? "You need to agree to the data protection policy to proceed." : "Unahitaji kukubaliana na sera ya ulinzi wa data ili uendelee";
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(messageText), cancellationToken);

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

            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);
        }

        private async Task<DialogTurnResult> EvaluateFeelingAsync(WaterfallStepContext stepContext, CancellationToken token)
        {
            Client me = (Client)stepContext.Values[UserInfo];

            var question =me.language? "Please specify the feeling "  : "Tafadhali bainisha hisia";


            //This is where the selected feeling is stored
            var response = stepContext.Context.Activity.Text;

            Conversation conversation = await _repository.Conversation
                .FindByCondition(x => x.Id == me.ConversationId)
                .Include(x => x.Persona)
                .FirstOrDefaultAsync();
          
            var feeling = await _repository.Feeling.FindByCondition(x => x.Description == response || x.Kiswahili == response).FirstOrDefaultAsync();

            ChatMessage chatMessage = new()
            {
                Message = response,
                Type = Constants.User,
                ConversationId = me.ConversationId
            };

            if (response.ToLower().Trim() == "other" || response.ToLower() == "zinginezo" || response.ToLower().Trim() == "others")
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

                conversation.FeelingId = feeling.Id;

                _repository.Conversation.Update(conversation);

                await _repository.SaveChangesAsync();

                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(question) }, token);
            }
            else
            {

                _repository.ChatMessage.CreateRange(new List<ChatMessage>
                { chatMessage });

            }
          
            
        
            if (conversation != null)
            {
                me.ConversationId = conversation.Id;

                Persona persona = conversation?.Persona;

                persona.FeelingId = feeling.Id;

                conversation.FeelingId = feeling.Id;

                // _repository.Persona.Update(persona);

                _repository.Conversation.Update(conversation);

                await _repository.SaveChangesAsync();
            }

            return await stepContext.NextAsync(me); 

        }



        private async Task<DialogTurnResult> HandleFeelingAsync(WaterfallStepContext stepContext,CancellationToken token)
        {
            Client me = (Client)stepContext.Values[UserInfo];

            Conversation conversation = await _repository.Conversation
                    .FindByCondition(x => x.Id==me.ConversationId)
                    .Include(x => x.Persona)
                    .FirstOrDefaultAsync();

            string response = stepContext.Context.Activity.Text;

            if (stepContext.Reason.ToString() !="NextCalled")
            {
             
                ChatMessage chatMessage = new()
                {
                    Message = response,
                    Type = Constants.User,
                    ConversationId = me.ConversationId
                };

                _repository.ChatMessage.Create(chatMessage);
            
                conversation.FeelingDetail = response;

                _repository.Conversation.Update(conversation);

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

        private async Task<DialogTurnResult> HandleBreathingStop(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client me = (Client)stepContext.Values[UserInfo];

            return await stepContext.BeginDialogAsync(nameof(BreathingDialog), me, cancellationToken);
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
            Client me = (Client)stepContext.Values[UserInfo];

            Conversation conversation = await _repository.Conversation
                    .FindByCondition(x => x.Id == me.ConversationId)
                    .Include(x => x.Persona)
                    .FirstOrDefaultAsync();

            var resp = await ChatGptDialog.GetChatGPTResponses("Give me a random encouraging quote",conversation, conversation.Language);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(conversation.Language ? $"Thank you , have a lovely day  {resp}" : $" Asante,kuwa na siku njema  {resp}"));

            return await stepContext.EndDialogAsync(null);
        }

        private async Task<String> GetPersonaName()
        {
            var personas = await _repository.Persona.FindAll().ToListAsync();

            decimal count = personas.Count;

            decimal value = 100000;

            decimal code = count / value;

            int counter = 5;

            string codeValue = code.ToString("N" + counter);

            if (code > 1)
            {
                counter = codeValue.Split(".")[0].Length + counter;
                
                value = value * Decimal.Parse(Math.Pow(10, codeValue.Split(".")[0].Length).ToString());
                
                code = count / value;
                
                codeValue = code.ToString("N" + counter);
            }
            return codeValue.Split(".")[1] ;
        }

        private async Task<Conversation> CreateConversationDBInstance(WaterfallStepContext stepContext)
        {
            Client me = (Client)stepContext.Values[UserInfo];
 
            //String name=await getPersonaName();

            var persona= await  _repository.Persona.FindByCondition(x => x.SenderId == stepContext.Context.Activity.From.Id).FirstOrDefaultAsync();

            Conversation conversation;

            String code = await GetPersonaName();

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

                    IsActive=true,
                };

                if(persona.CodeName == null)
                {
                    persona.CodeName = code;

                    _repository.Persona.Update(persona);
                }

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
                        Name = stepContext.Context.Activity.From.Name,
                        CodeName =code
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

        private async Task<IMessageActivity> GetAttachment(String choiceValue, Boolean language)
        {
            IntroductionChoice action = await _repository.IntroductionChoice.FindByCondition(x => x.Name == choiceValue || x.Kiswahili == choiceValue).FirstOrDefaultAsync();

            if (action != null)
            {
                if (action.ActionType == 1)
                {
                    var item = await _repository.InitialActionItem.FindByCondition(x => x.IntroductionChoiceId == action.Id && x.Language == (language? 1:0)).FirstOrDefaultAsync();

                    var card = new HeroCard
                    {
                        Title = "ChatCare",
                        Subtitle = item.SubTitle,
                        Text = item.ActionMessage,
                        Buttons = new List<CardAction>
                    {
                    new CardAction(ActionTypes.OpenUrl, item.ActionMessage, value: item.Value)
                   }
                    };

                    return MessageFactory.Attachment(card.ToAttachment());
                }
            }

            return MessageFactory.Text("No valid action found");
        }


    }
}



 