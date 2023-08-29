using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
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
    public class BreathingDialog : CancelAndHelpDialog
    {

        private List<Choice> _choice;
        
        //# private List<BreathingTip> _breathingTips;

        protected string iterations = "user-iterations";

        public BreathingDialog() : base(nameof(BreathingDialog))
        {


            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));


            var waterFallSteps = new WaterfallStep[]
            {
                InitialDialogTest,
                BreathingExcercisesAsync,
                FinalStepAsync
            };


            _choice = new List<Choice>()
                    {
                        new Choice() { Value = Validations.YES, Synonyms = new List<string> { "y", "Y", "YES", "YE", "ye", "yE", "1" } },
                        new Choice() { Value = Validations.NO, Synonyms = new List<string> { "n", "N", "no" } }
                    };


            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterFallSteps));

            InitialDialogId = nameof(WaterfallDialog);
        }

        public async Task<DialogTurnResult> InitialDialogTest(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            User user = (User)stepContext.Options;

            if (user.Iteration !=1)
            {
                return await stepContext.NextAsync(user);
            }
            else
            {
                stepContext.Values[iterations] = 1;
            }

           
           
            var prompts = new PromptOptions
            {
                Prompt = MessageFactory.Text("Would you like me to take you through some breathing exercises or tips on managing mental health?\r\n\r\n ")
                ,Choices
                = _choice
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), prompts, cancellationToken);
        }


        private int _exerciseIndex = 1;

        public async Task<DialogTurnResult> TakeUserThroughExerciseAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            User user = (User)stepContext.Options;

            var prompts = new PromptOptions
            {
                Prompt = MessageFactory.Text("Do you wish to continue?"),
                Choices = new List<Choice>(_choice)
            };

            var feelings = GetFeelingToExerciseMap();

            if (_exerciseIndex < feelings.Count)
            {
                var currentExercise = feelings[_exerciseIndex];
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(currentExercise.Exercise));
                _exerciseIndex++;

                if (_exerciseIndex >= feelings.Count)
                {
                    _exerciseIndex = 1;
                }

                return await stepContext.PromptAsync(nameof(ChoicePrompt), prompts, cancellationToken);
            }

            else
            {
                return await stepContext.EndDialogAsync(user);
            }
        }

        private async Task<DialogTurnResult> BreathingExcercisesAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var objectType=stepContext.Result.GetType();


            if (objectType.Name == "User")
            {

               return await TakeUserThroughExerciseAsync(stepContext, cancellationToken);
            }

            var userChoice = ((FoundChoice)stepContext.Result)?.Value;

            if (userChoice == Validations.NO)
            {
                return await stepContext.EndDialogAsync(null);
            }

            if (!string.IsNullOrEmpty(userChoice))
            {
                var user = (User)stepContext.Options;

                return await TakeUserThroughExerciseAsync(stepContext, cancellationToken);
            }
            else
            {
               return await stepContext.EndDialogAsync(null);
            }

        }


        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userChoice = ((FoundChoice)stepContext.Result)?.Value;

            var user = (User)stepContext.Options;

            user.Iteration = user.Iteration+ user.Iteration;


            if (userChoice !=null && userChoice==Validations.YES) return await stepContext.BeginDialogAsync(nameof(WaterfallDialog), user, cancellationToken);

            return await stepContext.EndDialogAsync(user, cancellationToken);

        }


        private Dictionary<int, BreathingTip> GetFeelingToExerciseMap()
        {
            var paths = new[] { ".", "Cards", "Exercise.json" };
            var adaptiveCardJson = File.ReadAllText(Path.Combine(paths));


            using StreamReader reader = new(Path.Combine(paths));
            var json = reader.ReadToEnd();
            List<BreathingTip> breathingTips = JsonConvert.DeserializeObject<List<BreathingTip>>(json);


            Dictionary<int, BreathingTip> items =new Dictionary<int, BreathingTip>();

            foreach(var item in breathingTips) {
                items[item.Value] = item;
            }

            return items;
        }




    }
}