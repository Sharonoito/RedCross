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

        public PersonalDialog( AwarenessDialog awarenessDialog,
                               BreathingDialog breathingDialog,AiDialog aiDialog,
                               ILogger<PersonalDialog> logger,  
                               IRepositoryWrapper wrapper,
                               UserState userState
            ) : base(nameof(PersonalDialog))
        {
            _logger = logger;

            _repository=wrapper;

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
            var persona = new Persona();

            if (stepContext.Result is Persona)
            {
                persona = stepContext.Result as Persona;
            }
            else
            {
                persona = await _repository.Persona.FindByCondition(x => x.SenderId == stepContext.Context.Activity.From.Id).FirstAsync();
            }



            var question = "What is your Country of Origin";

            if (persona != null)
            {
                persona.Feeling = stepContext.Context.Activity.Text;

                _repository.Persona.Update(persona);

                await _repository.SaveChangesAsync();

                await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, question, stepContext, _userProfileAccessor, _userState);

            }

            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text(question),
                RetryPrompt = MessageFactory.Text("Please select a Choose between the two"),
                Choices = new List<Choice>
                {
                    new Choice() { Value ="Kenya"},
                    new Choice() { Value="Other"},

                },
                Style = ListStyle.HeroCard,

            };

            // Prompt the user with the configured PromptOptions.
            return await stepContext.PromptAsync("select-country", options, cancellationToken);

        }

        private async Task<DialogTurnResult> PrivateDetailsCountyDropdownAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //List<string> counties = GetListOfCounties();

            var hint = "( hint: type in Kiambu or 022,Nairobi or 047 )";

            Persona persona = await _repository.Persona.FindByCondition(x => x.SenderId == stepContext.Context.Activity.From.Id).FirstAsync(cancellationToken: cancellationToken);


            var question = "Which county are you located in?";

            if (persona != null)
            {
                persona.Country = stepContext.Context.Activity.Text;

                _repository.Persona.Update(persona);

                await _repository.SaveChangesAsync();

                if (((FoundChoice)stepContext.Result).Value != "Kenya")
                {
                    question = "Which Country are you from ?";
                }

                await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, question, stepContext, _userProfileAccessor, _userState);

            }

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"{question} \n {hint} "),
                RetryPrompt = MessageFactory.Text($"Please input a county \n {hint} "),
            };

            if (((FoundChoice)stepContext.Result).Value == "Kenya")
            {

                return await stepContext.PromptAsync(RedCrossDialogTypes.SelectCounty, promptOptions, cancellationToken);
            }
            else
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(question) }, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> PrivateDetailsAgeBracketAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            Persona persona = await _repository.Persona.FindByCondition(x => x.SenderId == stepContext.Context.Activity.From.Id).FirstAsync();

            var question = "How old are you?";

            if (stepContext.Values != null)
            {
                persona.County = stepContext.Context.Activity.Text;

                _repository.Persona.Update(persona);

                await _repository.SaveChangesAsync();

                await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, question, stepContext, _userProfileAccessor, _userState);

            }

            var options = new PromptOptions()
                {
                    Prompt = MessageFactory.Text(question),
                    RetryPrompt = MessageFactory.Text("Please select a valid age-group"),
                    Choices = RedCrossLists.AgeGroups,

                };


            return await stepContext.PromptAsync("select-age", options, cancellationToken);

        }

        private async Task<DialogTurnResult> RelationShipStatusAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var question = "What is your relationship status ?";

            await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, question, stepContext, _userProfileAccessor, _userState);

            Persona persona = await _repository.Persona.FindByCondition(x => x.SenderId == stepContext.Context.Activity.From.Id).FirstAsync();

            persona.AgeGroup = stepContext.Context.Activity.Text;

            _repository.Persona.Update(persona);

            bool result = await _repository.SaveChangesAsync();

            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text("What is your relationship status ?"),
                
                Choices = new List<Choice>()
                        {
                            new Choice  { Value ="Single",Synonyms=new List<string>{"Single","S"}},
                            new Choice  { Value ="Married",Synonyms=new List<string>{"married"}},
                            new Choice  { Value ="Divorced",Synonyms=new List<string>{"divorced"}},
                            new Choice  { Value ="In A relationship",Synonyms=new List<string>{"dating","relations","casual"}},
                            new Choice  { Value ="Widow /Widower",Synonyms=new List<string>{"widow","widower"}},
                            new Choice  { Value ="Complicated",Synonyms=new List<string>{"complicated","comp","it's complicated"}},
                            new Choice  { Value ="none",Synonyms=new List<string>{"none","no"}},

                        },
                         Style = ListStyle.HeroCard,

            }, cancellationToken);
        }

        private async Task<DialogTurnResult> ProfessionalStatusAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var question = "What is your professional status ?";

            await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, question, stepContext, _userProfileAccessor, _userState);

            Persona persona = await _repository.Persona.FindByCondition(x => x.SenderId == stepContext.Context.Activity.From.Id).FirstAsync();

            persona.MaritalStatus = ((FoundChoice)stepContext.Result).Value;

            _repository.Persona.Update(persona);

            bool result = await _repository.SaveChangesAsync();


            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text(question),
                Choices = new List<Choice>()
                        {
                            new Choice  { Value ="Student",},
                            new Choice  { Value ="Employed",},
                            new Choice  { Value ="Entrepreneur"},
                            new Choice  { Value ="Retired"},
                            new Choice  { Value ="Unemployed"},
                            new Choice  { Value ="Complicated"},
                            new Choice  { Value ="none",Synonyms=new List<string>{"none","no"}},
                        },
                        Style = ListStyle.HeroCard,
        
            }, cancellationToken);

        }

      
        private async Task<DialogTurnResult> PrivateDetailsGenderAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            Persona persona = await _repository.Persona.FindByCondition(x => x.SenderId == stepContext.Context.Activity.From.Id).FirstAsync();

            await EvaluateDialog.ProcessStepAsync(stepContext, cancellationToken);

            var question = "What is your Gender";


            if (stepContext.Values != null)
            {

                await DialogExtensions.UpdateDialogAnswer(((FoundChoice)stepContext.Result).Value.ToString(), question, stepContext, _userProfileAccessor, _userState);

                persona.ProfessionalStatus = ((FoundChoice)stepContext.Result).Value;

                _repository.Persona.Update(persona);

                bool result = await _repository.SaveChangesAsync();
            }

            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text(question),
                RetryPrompt = MessageFactory.Text("Please select a valid Gender"),
                Choices = new List<Choice>
                {
                    new Choice() { Value = Gender.Male,Synonyms=new List<string>{"M","Man","MALE","y"}},
                    new Choice() { Value= Gender.Female,Synonyms=new List<string>{"f","fE","FEMALE","female"}},
                    new Choice() { Value= Gender.Other,Synonyms=new List<string>{"o","other"}},
                },
                Style = ListStyle.HeroCard,

            };

            // Prompt the user with the configured PromptOptions.
            return await stepContext.PromptAsync("select-gender", options, cancellationToken);
        }

        private static Task<bool> ValidateCountyAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var response = promptContext.Context.Activity.Text.Trim();

            List<County> counties = ReadCountyFromFile();

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

                if (county.Title.ToLower() == validatedResponse || county.Value == code)

                    {
                        status = true; break;
                }

            }

            return Task.FromResult(status);
        }

        //validation for mental awarenesss
        private async Task<DialogTurnResult> LaunchAwarenessDialogAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
           
            return await stepContext.BeginDialogAsync(nameof(AwarenessDialog), null, cancellationToken);

        }
        private async Task<DialogTurnResult> HandleBreathingStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken) {
            if (stepContext.Result != null)
            {
                User user = (User)(stepContext.Result);

                if (user.WantsBreathingExercises)
                {
                    return await stepContext.BeginDialogAsync(nameof(BreathingDialog), user, cancellationToken);
                }

                if (user.hasTalkedToSomeone == false && user.isAwareOfFeeling == false)
                {
                    return await stepContext.BeginDialogAsync(nameof(BreathingDialog), user, cancellationToken);
                }

                if (user.Iteration == 1 && user.WantsBreathingExercises)
                {
                    //handover to ui
                    return await stepContext.BeginDialogAsync(nameof(BreathingDialog), user, cancellationToken);
                }
                return await stepContext.EndDialogAsync(user, cancellationToken);
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            User user = (User)(stepContext.Result);

            //if (user !=null && user.Iteration > 1)
               // return await stepContext.EndDialogAsync(user, cancellationToken);

            return await stepContext.BeginDialogAsync(nameof(AiDialog), user, cancellationToken);
        }

        public List<County> ReadJsonFile()
        {
            //string sampleJsonFilePath = "counties.json";

            var paths = new[] { ".", "Cards", "counties.json" };
          
            using StreamReader reader = new(Path.Combine(paths));
            var json = reader.ReadToEnd();
            List<County> counties = JsonConvert.DeserializeObject<List<County>>(json);
            return counties;
        }

        public static List<County> ReadCountyFromFile()
        {

            var paths = new[] { ".", "Cards", "counties.json" };
            

            using StreamReader reader = new(Path.Combine(paths));
            var json = reader.ReadToEnd();
            List<County> counties = JsonConvert.DeserializeObject<List<County>>(json);
            return counties;
        }

        public List<string> GetListOfCounties()
        {
            List<County> counties = ReadJsonFile();
            List<string> countyNames = counties.Select(county => county.Title).ToList();
            return countyNames;
        }

        private void AddDialogs()
        {
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            AddDialog(new TextPrompt(nameof(TextPrompt)));

            AddDialog(new ChoicePrompt("select-choice"));

            AddDialog(new ChoicePrompt("select-gender"));

            AddDialog(new ChoicePrompt("select-feeling"));

            AddDialog(new ChoicePrompt("select-terms"));

            AddDialog(new NumberPrompt<int>("select-age"));

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

                Client = new Persona() { SenderId = stepContext.Context.Activity.From.Id },

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

    }
}