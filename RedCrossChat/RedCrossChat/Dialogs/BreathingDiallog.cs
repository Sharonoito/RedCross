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
        private readonly IStatePropertyAccessor<ResponseDto> _userProfileAccessor;

        protected readonly UserState _userState;


        protected string iterations = "user-iterations";
        protected string UserInfo = "client-iterations";

        public BreathingDialog(UserState userState) : base(nameof(BreathingDialog))
        {
            _userState = userState;

            _userProfileAccessor = userState.CreateProperty<ResponseDto>(DialogConstants.ProfileAssesor);

            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));


            var waterFallSteps = new WaterfallStep[]
            {
                InitialDialogTest,
                BreathingExcercisesAsync,
                FinalStepAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterFallSteps));

            InitialDialogId = nameof(WaterfallDialog);
        }

        public async Task<DialogTurnResult> InitialDialogTest(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            Client user = (Client)stepContext.Options;

            stepContext.Values[UserInfo] = user;

            var question = "Would you like me to take you through some breathing exercises or tips on managing mental health?\r\n\r\n ";

            if (user.Iteration !=1)
            {
                stepContext.Values[iterations] = user.Iteration;

                return await stepContext.NextAsync(user);
            }
            else
            {
                stepContext.Values[iterations] = 1;
            }

            await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, question, stepContext, _userProfileAccessor, _userState);


            var prompts = new PromptOptions
            {
                Prompt = MessageFactory.Text(question)
                ,Choices = RedCrossLists.choices
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), prompts, cancellationToken);
        }


       
        public async Task<DialogTurnResult> TakeUserThroughExerciseAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            string stringValue = stepContext.Values[iterations].ToString();

            int _exerciseIndex = Int32.Parse(stringValue);

            Client user = (Client)stepContext.Options;

            var question = "Do you wish to continue ?";

            var prompts = new PromptOptions
            {
                Prompt = MessageFactory.Text(question),
                Choices = RedCrossLists.choices
            };

            var feelings = GetFeelingToExerciseMap();

            if (_exerciseIndex < feelings.Count)
            {
                var currentExercise = feelings[_exerciseIndex];
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(currentExercise.Exercise));

                stepContext.Values[iterations]=_exerciseIndex+1;

                if (_exerciseIndex >= feelings.Count)
                {
                    stepContext.Values[iterations]  = 1;
                }

                await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, currentExercise.Exercise +" \n"+question, stepContext, _userProfileAccessor, _userState);


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

            Client user = (Client)stepContext.Options;


            if (objectType.Name == "User")
            {

               return await TakeUserThroughExerciseAsync(stepContext, cancellationToken);
            }

            var userChoice = ((FoundChoice)stepContext.Result)?.Value;

            if (userChoice == Validations.NO)
            {
                
                return await stepContext.EndDialogAsync(user);
            }

            if (!string.IsNullOrEmpty(userChoice))
            {
               

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

            var user = (Client)stepContext.Options;

            user.Iteration += user.Iteration;


            if (userChoice !=null && userChoice==Validations.YES) return await stepContext.BeginDialogAsync(nameof(WaterfallDialog), user, cancellationToken);

            return await stepContext.EndDialogAsync(user, cancellationToken);

        }


        private static Dictionary<int, BreathingTip> GetFeelingToExerciseMap()
        {
            var paths = new[] { ".", "Cards", "Exercise.json" };
           

            using StreamReader reader = new(Path.Combine(paths));
            var json = reader.ReadToEnd();
            List<BreathingTip> breathingTips = JsonConvert.DeserializeObject<List<BreathingTip>>(json);


            Dictionary<int, BreathingTip> items =new();

            foreach(var item in breathingTips) {
                items[item.Value] = item;
            }

            return items;
        }




    }
}