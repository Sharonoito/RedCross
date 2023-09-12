using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DateTime;
using Newtonsoft.Json;
using NuGet.Protocol.Core.Types;
using RedCrossChat.Cards;
using RedCrossChat.Contracts;
using RedCrossChat.Entities;
using RedCrossChat.Objects;
using Sentry;
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

        private readonly IRepositoryWrapper _repository;

        protected readonly ILogger _logger;

        private List<Choice> _choices;

        private const string UserInfo = "Client-info";

        protected string iterations = "user-iterations";
        protected string CurrentQuestion = "CurrentQuestion";

        public PersonalDialog( AwarenessDialog awarenessDialog, BreathingDialog breathingDialog,AiDialog aiDialog,
                               ILogger<PersonalDialog> logger,  IRepositoryWrapper wrapper
            ) : base(nameof(PersonalDialog))
        {
            _logger = logger;

            _repository=wrapper;

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

            AddDialog(awarenessDialog);

            AddDialog(breathingDialog);

            AddDialog(aiDialog);

            var mainDialog = new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]

            {
                InitialStepAsync,
                PrivateDetailsGenderAsync,
                PrivateDetailsAgeBracketAsync,
                PrivateDetailsCountryBracketAsync,
                PrivateDetailsCountyDropdownAsync,
                LaunchAwarenessDialogAsync,
                HandleBreathingStepAsync,
                
                FinalStepAsync,

            });

       

            AddDialog(mainDialog);

        
            _choices = new List<Choice>()
            {
                new Choice() { Value = "Yes", Synonyms = new List<string> { "y", "Y", "YES", "YE", "ye", "yE", "1" } },
                new Choice() { Value = "No", Synonyms = new List<string> { "n", "N", "no" } }
            };

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }
   
        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values[UserInfo] = new Client();

            Conversation conversation = new Conversation();

            conversation.ChannelId = stepContext.Context.Activity.ChannelId;

            conversation.ChannelName = stepContext.Context.Activity.ChannelId;

            var textWriter = new StringWriter();

           // var items=JsonSerializer.Serialize(textWriter,stepContext);

            // var channelObject=JsonConvert.SerializeObject(stepContext);

            conversation.SenderId = stepContext.Context.Activity.From.Id;

            conversation.ConversationId = stepContext.Context.Activity.Conversation.Id;

            conversation.Client = new Persona() { SenderId = stepContext.Context.Activity.From.Id };         

            conversation.AiConversations=new List<AiConversation>();

            _repository.Conversation.Create(conversation);

            bool result= await _repository.SaveChangesAsync();


            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text("How would you describe how you are feeling today?"),
                RetryPrompt = MessageFactory.Text("Please select a valid feeling"),
                Choices = new List<Choice>
                {
                    new Choice() { Value =Feelings.Happy,Synonyms=new List<string>{"happy","HAPPY","Happy"}},
                    new Choice() { Value=Feelings.Angry,Synonyms=new List<string>{"Angry","angry","ANGRY"}},
                    new Choice() { Value=Feelings.Anxious},
                    new Choice() { Value=Feelings.FlatEffect},
                    new Choice() { Value=Feelings.Expressionless},
                    new Choice() { Value=Feelings.Sad},
                },
            };

            stepContext.Values[CurrentQuestion] = "How would you describe how you are feeling today?";

            // Prompt the user with the configured PromptOptions.
            return await stepContext.PromptAsync("select-feeling", options, cancellationToken);
        }


        private async Task<bool> AddQuestionResponse(WaterfallStepContext stepContext, IRepositoryWrapper _repository)
        {

            Conversation conversation = await _repository.Conversation
                .FindByCondition(x => x.ConversationId == stepContext.Context.Activity.Conversation.Id).FirstAsync();

            var question = stepContext.Values[CurrentQuestion];

            if (conversation != null)
            {
                var rawConversation = new RawConversation { ConversationId = conversation.Id, Question = question.ToString(), Message = stepContext.Context.Activity.Text };

                _repository.RawConversation.Create(rawConversation);
            }
            return true;
        }

        private async Task<DialogTurnResult> PrivateDetailsGenderAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

           Persona persona = await _repository.Persona.FindByCondition(x => x.SenderId == stepContext.Context.Activity.From.Id).FirstAsync();

           
           
           await EvaluateDialog.ProcessStepAsync(stepContext, cancellationToken);

            if (stepContext.Values != null)
            {
                var client=(Client)stepContext.Values[UserInfo];

                
                client.Feeling= ((FoundChoice)stepContext.Result).Value;

                persona.Feeling = ((FoundChoice)stepContext.Result).Value;

                _repository.Persona.Update(persona);

                await AddQuestionResponse(stepContext, _repository);

                bool result = await _repository.SaveChangesAsync();
            }

            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text("What is your Gender"),
                RetryPrompt = MessageFactory.Text("Please select a valid Gender"),
                Choices = new List<Choice>
                {
                    new Choice() { Value = Gender.Male,Synonyms=new List<string>{"M","Man","MALE","y"}},
                    new Choice() { Value= Gender.Female,Synonyms=new List<string>{"f","fE","FEMALE","female"}},
                    new Choice() { Value= Gender.Other,Synonyms=new List<string>{"o","other"}},
                },
            };

            stepContext.Values[CurrentQuestion] = "What is your Gender?";

            // Prompt the user with the configured PromptOptions.
            return await stepContext.PromptAsync("select-gender", options, cancellationToken);
        }

        private async Task<DialogTurnResult> PrivateDetailsAgeBracketAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            Persona persona = await _repository.Persona.FindByCondition(x => x.SenderId == stepContext.Context.Activity.From.Id).FirstAsync();

            if (stepContext.Values != null)
            {
                persona.Gender = ((FoundChoice)stepContext.Result).Value;

                _repository.Persona.Update(persona);

                await AddQuestionResponse(stepContext, _repository);

                await _repository.SaveChangesAsync();
            }

                var options = new PromptOptions()
                {
                    Prompt = MessageFactory.Text("How old are you?"),
                    RetryPrompt = MessageFactory.Text("Please select a valid age-group"),
                    Choices = RedCrossLists.AgeGroups,

                };

            stepContext.Values[CurrentQuestion] = "How old are you?";

            // Prompt the user with the configured PromptOptions.
            return await stepContext.PromptAsync("select-age", options, cancellationToken);

        }

        private async Task<DialogTurnResult> PrivateDetailsCountryBracketAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            Persona persona = await _repository.Persona.FindByCondition(x => x.SenderId == stepContext.Context.Activity.From.Id).FirstAsync();

            if(persona != null)
            {
                persona.AgeGroup= stepContext.Context.Activity.Text;

                _repository.Persona.Update(persona);

                await AddQuestionResponse(stepContext, _repository);

                await _repository.SaveChangesAsync();
            }

            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text("What is your Country of Origin"),
                RetryPrompt = MessageFactory.Text("Please select a Choose between the two"),
                Choices = new List<Choice>
                {
                    new Choice() { Value ="Kenya"},
                    new Choice() { Value="Other"},

                },
            };

            stepContext.Values[CurrentQuestion] = "What is your Country of Origin?";

            // Prompt the user with the configured PromptOptions.
            return await stepContext.PromptAsync("select-country", options, cancellationToken);

        }

      
        private async Task<DialogTurnResult> PrivateDetailsCountyDropdownAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //List<string> counties = GetListOfCounties();

            var hint = "( hint: type in Kiambu or 022,Nairobi or 047 )";

            Persona persona = await _repository.Persona.FindByCondition(x => x.SenderId == stepContext.Context.Activity.From.Id).FirstAsync();

            if (persona != null)
            {
                persona.Country = stepContext.Context.Activity.Text;

                _repository.Persona.Update(persona);

                await AddQuestionResponse(stepContext, _repository);

                await _repository.SaveChangesAsync();
            }

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Which county are you located in? \n {hint} "),
                RetryPrompt = MessageFactory.Text($"Please input a county \n {hint} "),
            };

            if (((FoundChoice)stepContext.Result).Value == "Kenya")
            {
                stepContext.Values[CurrentQuestion] = "Which county are you located in?";

                return await stepContext.PromptAsync(RedCrossDialogTypes.SelectCounty, promptOptions, cancellationToken);
            }
            else
            {
                stepContext.Values[CurrentQuestion] = "Which Country are you from ??";

                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt=MessageFactory.Text("Which Country are you from ?") }, cancellationToken); 
            }  
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

            if (user !=null && user.Iteration > 1)
                return await stepContext.EndDialogAsync(user);

            return await stepContext.BeginDialogAsync(nameof(AiDialog), user, cancellationToken);
        }

        public List<County> ReadJsonFile()
        {
            //string sampleJsonFilePath = "counties.json";

            var paths = new[] { ".", "Cards", "counties.json" };
            var adaptiveCardJson = File.ReadAllText(Path.Combine(paths));


            using StreamReader reader = new(Path.Combine(paths));
            var json = reader.ReadToEnd();
            List<County> counties = JsonConvert.DeserializeObject<List<County>>(json);
            return counties;
        }

        public static List<County> ReadCountyFromFile()
        {

            var paths = new[] { ".", "Cards", "counties.json" };
            var adaptiveCardJson = File.ReadAllText(Path.Combine(paths));


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

    }



}





























//private async Task<DialogTurnResult> HandleCasesWithAI(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//{

//    if(stepContext.Result !=null)
//    {
//        User user = (User)stepContext.Result;

//        if (!user.wantsToTalkToSomeone  && !user.handOverToUser)
//        {
//            return await stepContext.BeginDialogAsync(nameof(BreathingDialog), user, cancellationToken);
//        }

//        if (user.handOverToUser)
//        {
//            ///todo human hand over task
//        }

//    }
//    var promptOptions = new PromptOptions
//    {
//        Prompt = MessageFactory.Text("Handle with ai "),
//        RetryPrompt = MessageFactory.Text("The county you entered is not valid. Please try again."),
//    };

//    //validation to check if its breathing extercices or its handle to ai

//    return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
//}
