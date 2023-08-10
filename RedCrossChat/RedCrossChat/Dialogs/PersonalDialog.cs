using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DateTime;
using Newtonsoft.Json;
using RedCrossChat.Cards;
using RedCrossChat.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RedCrossChat.Dialogs
{
    public class PersonalDialog : CancelAndHelpDialog
    {


        protected readonly ILogger _logger;
        private List<Choice> _choices;

        private const string UserInfo = "user-info";

        public PersonalDialog(AwarenessDialog awarenessDialog, ILogger<PersonalDialog> logger) : base(nameof(PersonalDialog))
        {
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            AddDialog(new TextPrompt(nameof(TextPrompt)));

            AddDialog(new ChoicePrompt("select-choice"));

            AddDialog(new ChoicePrompt("select-gender"));

            AddDialog(new ChoicePrompt("select-feeling"));

            AddDialog(new ChoicePrompt("select-terms"));

            AddDialog(new ChoicePrompt("select-age"));

            AddDialog(new ChoicePrompt("select-country"));

            AddDialog(new ChoicePrompt("select-awareness"));

            AddDialog(awarenessDialog);




            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]

            {
                InitialStepAsync,
                PrivateDetailsGenderAsync,
                PrivateDetailsAgeBracketAsync,
                PrivateDetailsCountryBracketAsync,
                PrivateDetailsCountyDropdownAsync,
                ValidateCountyAsync,
                FinalStepAsync,
                //AwarenessStepAsync,

            }));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                // Implement the steps for 'PrivatePersonalFeelingAsync' dialog here
            }));


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

            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text("How would you describe how you are feeling today?"),
                RetryPrompt = MessageFactory.Text("Please select a valid feeling"),
                Choices = new List<Choice>
                {
                    new Choice() { Value ="Happy 😀"},
                    new Choice() { Value="Angry 😡"},
                    new Choice() {Value="Anxious 🥴"},
                    new Choice() {Value="Sad 😪"},
                    new Choice() {Value="Flat Effect 🫥"},
                    new Choice() {Value="Expressionless 🫤"},
                },
            };

            // Prompt the user with the configured PromptOptions.
            return await stepContext.PromptAsync("select-feeling", options, cancellationToken);

        }


        private async Task<DialogTurnResult> PrivateDetailsGenderAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text("What is your Gender"),
                RetryPrompt = MessageFactory.Text("Please select a valid Gender"),
                Choices = new List<Choice>
                {
                    new Choice() { Value ="Male",Synonyms=new List<string>{"M","Man","MALE","y"}},
                    new Choice() { Value="Female",Synonyms=new List<string>{"f","fE","FEMALE","female"}},
                    new Choice() {Value="Other",Synonyms=new List<string>{"o","other"}},
                },
            };

            // Prompt the user with the configured PromptOptions.
            return await stepContext.PromptAsync("select-gender", options, cancellationToken);


        }

        private async Task<DialogTurnResult> PrivateDetailsAgeBracketAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text("What is your age group"),
                RetryPrompt = MessageFactory.Text("Please select a valid age-group"),
                Choices = new List<Choice>
                {
                    new Choice() { Value ="15-20",Synonyms=new List<string>{"15","16","17","19","20"}},
                    new Choice() { Value="20-30",Synonyms=new List<string>{"21","22","23","24","25","26","27","28","29","30"}},
                    new Choice() {Value="30-40",Synonyms=new List<string>{"31","32","33","34","35","36","37","38","39","40"}},
                    new Choice() {Value="Above 40"},
                },

            };

            // Prompt the user with the configured PromptOptions.
            return await stepContext.PromptAsync("select-age", options, cancellationToken);

        }

        private static Task<bool> AgePromptValidatorAsync(PromptValidatorContext<int> promptContext, CancellationToken cancellationToken)
        {
            // This condition is our validation rule. You can also change the value at this point.
            return Task.FromResult(promptContext.Recognized.Succeeded && promptContext.Recognized.Value > 0 && promptContext.Recognized.Value < 150);
        }

        private static Task<bool> ValidateAgeAsync(PromptValidatorContext<FoundChoice> promptContext, CancellationToken cancellationToke)
        {

            //return Task.FromResult(promptContext.Recognized.Succeeded);

            string message = promptContext.Context.Activity.Text;

            if (message == null || message.Length==0)
            {
                return Task.FromResult(false);
            }

            string[] bands = message.Split('-');

            /**
             * check if the text has 15-10 or its just a single text
             *  if its a single text then assign it to a band
             *  add a counter
             *  if the user tries the age validador three times the bot should expire
             */

            if (bands.Length > 1)
            {
                return Task.FromResult(true);
            }
            else
            {
                int result = int.Parse(message);

                if (result >= 15  && result <=20)
                {
                    return Task.FromResult(!promptContext.Recognized.Succeeded);
                }
                else if (result > 20 && result <= 30)
                {
                    return Task.FromResult(!promptContext.Recognized.Succeeded);
                }
                else if (result > 30 && result <= 40)
                {
                    return Task.FromResult(!promptContext.Recognized.Succeeded);
                }
                else if (result > 40)
                {
                    return Task.FromResult(!promptContext.Recognized.Succeeded);
                }
                else if (result < 15)
                {
                    return Task.FromResult(false);
                }

            }


            return Task.FromResult(promptContext.Recognized.Succeeded);
        }

        private async Task<DialogTurnResult> PrivateDetailsCountryBracketAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

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

            // Prompt the user with the configured PromptOptions.
            return await stepContext.PromptAsync("select-country", options, cancellationToken);

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

        public List<string> GetListOfCounties()
        {
            List<County> counties = ReadJsonFile();
            List<string> countyNames = counties.Select(county => county.Title).ToList();
            return countyNames;
        }

        private async Task<DialogTurnResult> PrivateDetailsCountyDropdownAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //List<string> counties = GetListOfCounties();
            var promptOptions = new PromptOptions

            //var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text("Which county are you located in?"),
                RetryPrompt = MessageFactory.Text("Please select a county from the dropdown"),
                //Choices = counties.Select(county => new Choice(county)).ToList(),
            };

            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }



        private async Task<DialogTurnResult> ValidateCountyAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string county = (string)stepContext.Result;

            List<string> validCounties = GetListOfCounties();

            bool isValidCounty = validCounties.Contains(county, StringComparer.OrdinalIgnoreCase);

            if (!isValidCounty)
            {
                // County is not valid, reprompt the user to enter again
                var promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text("Please enter a valid county name."),
                    RetryPrompt = MessageFactory.Text("The county you entered is not valid. Please try again."),
                };

                return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
            }

            // County is valid, continue to the next step in the waterfall dialog
            return await stepContext.NextAsync(county, cancellationToken);
        }



        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //return await stepContext.EndDialogAsync(null, cancellationToken);
            return await stepContext.BeginDialogAsync(nameof(AwarenessDialog), null, cancellationToken);

        }

        private IList<Choice> GetChoices()
        {
            var cardOptions = new List<Choice>()
            {
                new Choice() { Value = "Careers", Synonyms = new List<string>() { "1" } },
                new Choice() { Value = "Volunteer and Membership", Synonyms = new List<string>() { "2" } },
                new Choice() { Value = "Volunteer Opportunities", Synonyms = new List<string>() { "3" } },
                new Choice() { Value = "Mental Health", Synonyms = new List<string>() { "4" } },

            };

            return cardOptions;
        }
    }
}
