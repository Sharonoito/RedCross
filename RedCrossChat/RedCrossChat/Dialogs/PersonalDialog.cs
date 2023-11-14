using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RedCrossChat.Contracts;
using RedCrossChat.Entities;
using RedCrossChat.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using User = RedCrossChat.Objects.User;

namespace RedCrossChat.Dialogs
{
    public class PersonalDialog : CancelAndHelpDialog
    {
        private readonly UserState _userState;
        private readonly IRepositoryWrapper _repository;
        private readonly IStatePropertyAccessor<ResponseDto> _userProfileAccessor;
        protected readonly ILogger _logger;

        private const string UserInfo = "Client-info";
        protected string iterations = "user-iterations";
        protected string CurrentQuestion = "CurrentQuestion";

        public PersonalDialog(AwarenessDialog awarenessDialog,
                               BreathingDialog breathingDialog, AiDialog aiDialog,
                               ILogger<PersonalDialog> logger,
                               IRepositoryWrapper wrapper,
                               UserState userState,
                               BaseDialog baseDialog
            ) : base(nameof(PersonalDialog), baseDialog, wrapper)
        {
            _logger = logger;

            _repository = wrapper;

            _userState = userState;

            _userProfileAccessor = userState.CreateProperty<ResponseDto>(DialogConstants.ProfileAssesor);

            AddDialogs();

            AddDialog(awarenessDialog);

            AddDialog(breathingDialog);

            AddDialog(aiDialog);

            var mainDialog = new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                PrivateDetailsCountryBracketAsync,
                PrivateDetailsCountyDropdownAsync,
                PrivateDetailsAgeBracketAsync,
                RelationShipStatusAsync,
                ProfessionalStatusAsync,
                PrivateDetailsGenderAsync,
                LaunchAwarenessDialogAsync,
                HandleBreathingStepAsync,
                FinalStepAsync,

            });



            AddDialog(mainDialog);

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> PrivateDetailsCountryBracketAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var me = (Client)stepContext.Options;

            stepContext.Values[UserInfo] = me;

            var persona = await _repository.Persona.FindByCondition(x => x.SenderId == stepContext.Context.Activity.From.Id).FirstAsync();

            Question quiz = await _repository.Question.FindByCondition(x => x.Code == 11).FirstAsync();

            ChatMessage chatMessage = new ChatMessage
            {
                QuestionId = quiz.Id,
                Type = Constants.Bot,
                ConversationId = me.ConversationId
            };

            var question = me.language ? quiz.question : quiz.Kiswahili;

            _repository.ChatMessage.Create(chatMessage);

            await _repository.SaveChangesAsync();

            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text(question),
                RetryPrompt = MessageFactory.Text(me.language ? "Please select a Choose between the two" : "Tafadhali chagua Chaguo kati ya hizo mbili"),
                Choices = me.language ? RedCrossLists.Countrys : RedCrossLists.CountryKiswahili,
                Style = ListStyle.HeroCard,

            };

            // Prompt the user with the configured PromptOptions.
            return await stepContext.PromptAsync("select-country", options, cancellationToken);

        }

        private async Task<DialogTurnResult> PrivateDetailsCountyDropdownAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client me = (Client)stepContext.Values[UserInfo];

            var hint = me.language ? "( hint: type in Kiambu or 022,Nairobi or 047 )" : "( dokezo: andika Kiambu au 022, Nairobi au 047 )";

            Conversation conv = await _repository.Conversation.FindByCondition(x => x.Id == me.ConversationId).Include(x => x.Persona).FirstAsync();

            Persona persona = conv.Persona;

            var question = me.language ? "Which county?" : "Unapatikana kaunti gani?";

            var questionRetry = me.language ? "Please input a county" : "Tafadhali andika katika kaunti";


            question = $"{question} \n {hint} ";

            var select = ((FoundChoice)stepContext.Result).Value;



            if (persona != null)
            {
                persona.Country = stepContext.Context.Activity.Text;

                _repository.Persona.Update(persona);

                await _repository.SaveChangesAsync();

            }

            ChatMessage chatMessage = new ChatMessage
            {
                Message = stepContext.Context.Activity.Text,
                Type = Constants.User,
                ConversationId = me.ConversationId
            };

            var promptId = RedCrossDialogTypes.SelectCounty;

            if (select != CountryValidation.Kenya || select != CountrySwahili.Kenya)
            {
                question = me.language ? "Which Country are you from ?" : "Unatoka Nchi gani ?";

                promptId = nameof(TextPrompt);
            }

            _repository.ChatMessage.CreateRange(new List<ChatMessage> { chatMessage,new ChatMessage
            {
                Message = question,
                Type = Constants.Bot,
                ConversationId = conv.Id,
            }
            });

            await _repository.SaveChangesAsync();

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(question),
                RetryPrompt = MessageFactory.Text($"${questionRetry} \n {hint} "),
            };

            return await stepContext.PromptAsync(promptId, promptOptions, cancellationToken);
        }

        private async Task<Conversation> getConversation(Client me)
        {
            return await _repository.Conversation.FindByCondition(x => x.Id == me.ConversationId).Include(x => x.Persona).FirstAsync();
        }

        private async Task<DialogTurnResult> PrivateDetailsAgeBracketAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client me = (Client)stepContext.Values[UserInfo];

            Conversation conversation = await getConversation(me);

            Question quiz=await _repository.Question.FindByCondition(x=>x.Code==2).FirstAsync();

            Persona persona = conversation.Persona;

            var ageGroups = await _repository.AgeBand.GetAll();

            var choices = new List<Choice>();

            foreach (var ageGroup in ageGroups)
            {
                choices.Add(new Choice() { Value = me.language ? ageGroup.Name : ageGroup.Kiswahili });
            }  

            var response = stepContext.Context.Activity.Text;

            var question = me.language? quiz.question : quiz.Kiswahili;

            if (stepContext.Values != null)
            {
                var county = await GetCountyResponse(response);

                if (county != null)
                {
                    persona.CountyId = county.Id;

                    _repository.Persona.Update(persona);
                }  
            }
            var list = new List<ChatMessage>() {
                new ChatMessage
                {
                    Message= response,
                    Type=Constants.User,
                    ConversationId=conversation.Id,
                },
                new ChatMessage
                {
                    QuestionId = quiz.Id,
                    Type = Constants.Bot,
                    ConversationId = conversation.Id,
                }
            };

            _repository.ChatMessage.CreateRange(list);

            await _repository.SaveChangesAsync();

            var options = new PromptOptions(){
                Prompt = MessageFactory.Text(question),
                RetryPrompt = MessageFactory.Text(me.language? "Please select a valid age-group or type a valid age" : "Tafadhali chagua kikundi halali cha umri au andika umri sahihi"),
                Choices =choices,
                Style = ListStyle.HeroCard,
            };


            return await stepContext.PromptAsync("select-age", options, cancellationToken);

        }

        private async Task<DialogTurnResult> RelationShipStatusAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client me = (Client)stepContext.Values[UserInfo];

            Conversation conversation = await getConversation(me);

            Question quiz = await _repository.Question.FindByCondition(x => x.Code == 12).FirstAsync();

            var question = me.language? quiz.question : quiz.Kiswahili;

            var response = stepContext.Context.Activity.Text;
        
            var status = await _repository.MaritalState.GetAll();

            var choices = new List<Choice>();

            foreach (var choice in status)
            {
                choices.Add(new Choice { Value = me.language ? choice.Name : choice.Kiswahili });
            }
            
            var ageGroup=await _repository.AgeBand.FindByCondition(x => x.Name==response || x.Kiswahili ==response).FirstOrDefaultAsync();

            if(ageGroup != null)
            {
                Persona persona = conversation.Persona;

                persona.AgeBandId = ageGroup.Id;

                _repository.Persona.Update(persona);

                bool result = await _repository.SaveChangesAsync();

            }

            var list = new List<ChatMessage>() {
                new ChatMessage
                {
                    Message= response,
                    Type=Constants.User,
                    ConversationId=conversation.Id,
                },
                new ChatMessage
                {
                    QuestionId = quiz.Id,
                    Type = Constants.Bot,
                    ConversationId = conversation.Id,
                }
            };

            _repository.ChatMessage.CreateRange(list);

            await _repository.SaveChangesAsync();

            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text(question),
                Choices = choices,
                Style = ListStyle.HeroCard,

            }, cancellationToken);
        }

        private async Task<DialogTurnResult> ProfessionalStatusAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client me = (Client)stepContext.Values[UserInfo];

            Conversation conversation = await getConversation(me);

            Question quiz = await _repository.Question.FindByCondition(x => x.Code == 3).FirstAsync();

            var question = me.language? quiz.question : quiz.Kiswahili;
    
            Persona persona = conversation.Persona;

            var response = stepContext.Context.Activity.Text;

            MaritalState state=await _repository.MaritalState.FindByCondition(x => x.Name == response || x.Kiswahili == response).FirstAsync();

            persona.MaritalStatus = ((FoundChoice)stepContext.Result).Value;

            persona.MaritalStateId=state.Id;

            _repository.Persona.Update(persona);

            var list = new List<ChatMessage>() {
                new ChatMessage
                {
                    Message= response,
                    Type=Constants.User,
                    ConversationId=conversation.Id,
                },
                new ChatMessage
                {
                    QuestionId = quiz.Id,
                    Type = Constants.Bot,
                    ConversationId = conversation.Id,
                }
            };

            _repository.ChatMessage.CreateRange(list);

            bool result = await _repository.SaveChangesAsync();

            var dboProf = await _repository.Profession.GetAll();

            var professions = new List<Choice>();

            foreach (var choice in dboProf)
            {
                professions.Add(new Choice { Value = me.language ? choice.Name : choice.Kiswahili });
            }


            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text(question),
                Choices = professions,
                Style = ListStyle.HeroCard,
            }, cancellationToken);

        }

      
        private async Task<DialogTurnResult> PrivateDetailsGenderAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client me = (Client)stepContext.Values[UserInfo];

            Conversation conversation = await getConversation(me);

            Question quiz = await _repository.Question.FindByCondition(x => x.Code == 13).FirstAsync();

            await EvaluateDialog.ProcessStepAsync(stepContext, cancellationToken);

            var question = me.language? quiz.question : quiz.Kiswahili;

            var response = stepContext.Context.Activity.Text;

            if (stepContext.Values != null)
            {
                Persona persona = conversation.Persona;

                Profession prof=await _repository.Profession.FindByCondition(x=>x.Kiswahili==response || x.Name==response).FirstAsync();
             
                persona.ProfessionalStatus = ((FoundChoice)stepContext.Result).Value;

                persona.ProfessionId = prof.Id;

                _repository.Persona.Update(persona);
            }
            var list = new List<ChatMessage>() {
                new ChatMessage
                {
                    Message= response,
                    Type=Constants.User,
                    ConversationId=conversation.Id,
                },
                new ChatMessage
                {
                    QuestionId = quiz.Id,
                    Type = Constants.Bot,
                    ConversationId = conversation.Id,
                }
            };

            _repository.ChatMessage.CreateRange(list);

            bool result = await _repository.SaveChangesAsync();

            var DboGenders = await _repository.Gender.GetAll();

            var genders = new List<Choice> { };

            foreach (var choice in DboGenders)
            {
                genders.Add(new Choice { Value = me.language ? choice.Name : choice.Kiswahili });
            }

            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text(question),
                RetryPrompt = MessageFactory.Text(question),
                Choices = genders,
                Style = ListStyle.HeroCard,

            };

            // Prompt the user with the configured PromptOptions.
            return await stepContext.PromptAsync("select-gender", options, cancellationToken);
        }

        private async Task<bool> ValidateAgeAsync(PromptValidatorContext<FoundChoice> promptContext, CancellationToken cancellationToken)
        {

           // promptContext.Context.Activity.Text
            var age=await _repository.AgeBand.FindByCondition(x=>x.Name== promptContext.Context.Activity.Text).FirstOrDefaultAsync();

            if (age == null)
            {
                try
                {
                   int ageText= Int32.Parse(promptContext.Context.Activity.Text);
                }
                catch (FormatException)
                {
                    return false;
                }

            }
            else
            {
                return true;
            }


            return true;
        }

        private  async Task<bool> ValidateCountyAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var response = promptContext.Context.Activity.Text.Trim();

            var counties = await _repository.County.GetAll();

            var validatedResponse = response.ToLower();

            var status = false;

            var code = 0;
            try
            {
                code = int.Parse(response);
            }
            catch (FormatException)
            {
                // _logger.LogError(exception);
                //_logger.LogError(e, $"Exception caught on attempting to Delete ConversationState : {e.Message}");

            }

            foreach (var county in counties) {

                if (county.Name.ToLower() == validatedResponse || county.Code == code){
                        status = true; break;
                }

            }

            return status;
        }

        //validation for mental awarenesss
        private async Task<DialogTurnResult> LaunchAwarenessDialogAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var response = stepContext.Context.Activity.Text;

            Client me = (Client)stepContext.Values[UserInfo];

            Conversation conv =await getConversation(me);

            _repository.ChatMessage.Create(new ChatMessage
            {
                Message = response,
                Type = Constants.User,
                ConversationId = conv.Id,
            });

            await _repository.SaveChangesAsync();


            return await stepContext.BeginDialogAsync(nameof(AwarenessDialog), me, cancellationToken);

        }
        private async Task<DialogTurnResult> HandleBreathingStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken) {

            if (stepContext.Result != null)
            {
                Client user = (Client)(stepContext.Result);

                if (user.WantsBreathingExercises)
                {
                    return await stepContext.BeginDialogAsync(nameof(BreathingDialog), user, cancellationToken);
                }

                if (user.HasTalkedToSomeone == false && user.IsAwareOfFeeling == false)
                {
                    return await stepContext.BeginDialogAsync(nameof(BreathingDialog), user, cancellationToken);
                }

                if (user.WantsBreathingExercises)
                {
                    //handover to ui
                    return await stepContext.BeginDialogAsync(nameof(BreathingDialog), user, cancellationToken);
                }
                
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client user = (Client)(stepContext.Result);

            return await stepContext.EndDialogAsync(user);

            // return await stepContext.BeginDialogAsync(nameof(AiDialog), user, cancellationToken);
        }

        private void AddDialogs()
        {
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            AddDialog(new TextPrompt(nameof(TextPrompt)));

            AddDialog(new ChoicePrompt("select-choice"));

            AddDialog(new ChoicePrompt("select-gender"));

            AddDialog(new ChoicePrompt("select-feeling"));

            AddDialog(new ChoicePrompt("select-terms"));

            AddDialog(new ChoicePrompt("select-age", ValidateAgeAsync));

            AddDialog(new ChoicePrompt("select-country"));

            AddDialog(new ChoicePrompt("select-awareness"));

            AddDialog(new TextPrompt(RedCrossDialogTypes.SelectCounty, ValidateCountyAsync));
        }

        private async Task<Conversation> CreateConversationDBInstance(WaterfallStepContext stepContext)
        {
            Conversation conversation = new()
            {
                ChannelId = stepContext.Context.Activity.ChannelId,

                ChannelName = stepContext.Context.Activity.ChannelId,

                SenderId = stepContext.Context.Activity.From.Id,

                ConversationId = stepContext.Context.Activity.Conversation.Id,

                Persona = new Persona() { SenderId = stepContext.Context.Activity.From.Id },

                AiConversations = new List<AiConversation>()
            };

            _repository.Conversation.Create(conversation);

            bool result = await _repository.SaveChangesAsync();

            if (result)
            {

            }

            await DialogExtensions.UpdateDialogConversationId(conversation.Id, stepContext, _userProfileAccessor, _userState);

            return conversation;
        }


        private async Task<DBCounty> GetCountyResponse(string text)
        {
            try
            {
                int code = int.Parse(text);

                return await _repository.County.FindByCondition(x => x.Code == code ).FirstOrDefaultAsync();
            }
            catch (FormatException)
            {
                return await _repository.County.FindByCondition(x=>x.Name == text).FirstOrDefaultAsync();
            }
            
        }
    }
}