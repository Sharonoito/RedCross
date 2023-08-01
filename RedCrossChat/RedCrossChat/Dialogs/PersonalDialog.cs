using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RedCrossChat.Dialogs
{
    public class PersonalDialog : CancelAndHelpDialog
    {


        protected readonly ILogger _logger;
        private List<Choice> _choices;

        private const string UserInfo = "user-info";

        public PersonalDialog(ILogger<PersonalDialog> logger) : base(nameof(PersonalDialog))
        {

            AddDialog(new ChoicePrompt("select-choice"));

            AddDialog(new ChoicePrompt("select-gender"));

            AddDialog(new ChoicePrompt("select-feeling"));

            AddDialog(new ChoicePrompt("select-terms"));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InitialStepAsync,
                TermsAndConditionsAsync,
                FinalStepAsync,
            }));

            _choices = new List<Choice>();
            {
                new Choice() { Value = "Yes", Synonyms = new List<string> { "y", "Y", "YES", "YE", "ye", "yE", "1" } };
                new Choice() { Value = "No", Synonyms = new List<string> { "n", "N", "no" } };
            }

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text("Welcome to the the chat bot we are working on this"),

                Choices =GetChoices(),
            };

            return await stepContext.PromptAsync("select-terms", options, cancellationToken);
        }

        private async Task<DialogTurnResult> TermsAndConditionsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        private IList<Choice> GetChoices()
        {
            var cardOptions = new List<Choice>()
            {
                new Choice() { Value = "Carrers", Synonyms = new List<string>() { "1" } },
                new Choice() { Value = "Volunteer and Membership", Synonyms = new List<string>() { "2" } },
                new Choice() { Value = "Volunteer Opprtunities", Synonyms = new List<string>() { "3" } },
                new Choice() { Value = "Mental Health", Synonyms = new List<string>() { "4" } },

            };

            return cardOptions;
        }
    }
}
