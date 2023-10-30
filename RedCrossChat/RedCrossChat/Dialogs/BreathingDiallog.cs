using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Newtonsoft.Json;
using RedCrossChat.Contracts;
using RedCrossChat.Objects;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RedCrossChat.Dialogs
{
    public class BreathingDialog : CancelAndHelpDialog
    {
        private readonly IStatePropertyAccessor<ResponseDto> _userProfileAccessor;

        protected readonly UserState _userState;


        protected string iterations = "user-iterations";
        protected string UserInfo = "client-iterations";

        public BreathingDialog(UserState userState, BaseDialog baseDialog, IRepositoryWrapper wrapper) : base(nameof(BreathingDialog), baseDialog,wrapper)
        {
            _userState = userState;

            _userProfileAccessor = userState.CreateProperty<ResponseDto>(DialogConstants.ProfileAssesor);

            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));


            var waterFallSteps = new WaterfallStep[]
            {
                InitialDialogTest, 
                TakeUserThroughExerciseAsync ,
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

            var question = user.language? "Would you like me to take you through some breathing exercises or tips on managing mental health? ":
                "Je, ungependa nikupitishe baadhi ya mazoezi ya kupumua au vidokezo kuhusu kudhibiti afya ya akili?";

            if (user.Iteration !=0)
            {
                stepContext.Values[iterations] = user.Iteration;

                return await stepContext.NextAsync(user);
            }
            else
            {
                user.Iteration++;
                stepContext.Values[iterations] = 1;
            }

            await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, question, stepContext, _userProfileAccessor, _userState);


            var prompts = new PromptOptions
            {
                Prompt = MessageFactory.Text(question),
                Choices = user.language ? RedCrossLists.choices : RedCrossLists.choicesKiswahili,
                Style = ListStyle.HeroCard
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), prompts, cancellationToken);
        }



        public async Task<DialogTurnResult> TakeUserThroughExerciseAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client user = (Client)stepContext.Options;

            if(stepContext.Result.GetType() == typeof(FoundChoice))
            {
                if (!WantsToContinue(stepContext))
                {
                    return await stepContext.EndDialogAsync(user);
                }
            }

           

            string stringValue = stepContext.Values[iterations].ToString();

            int _exerciseIndex = user.Iteration;

            if (_exerciseIndex ==0) _exerciseIndex++;

            var question = user.language? "Do you wish to continue ?": "Je, ungependa kuendelea?";

            var prompts = new PromptOptions
            {
                Prompt = MessageFactory.Text(question),
                Choices = user.language? RedCrossLists.choices : RedCrossLists.choicesKiswahili,
                Style = ListStyle.HeroCard
            };

            var feelings = GetFeelingToExerciseMap();

            if (_exerciseIndex < feelings.Count)
            {
                var currentExercise = feelings[_exerciseIndex];
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(user.language? currentExercise.Exercise : currentExercise.Kiswahili));

                stepContext.Values[iterations]=_exerciseIndex+1;

                user.Iteration=user.Iteration+1;

                if (_exerciseIndex >= feelings.Count)
                {
                    stepContext.Values[iterations]  = _exerciseIndex;
                }
                else
                {
                    stepContext.Values[iterations] = _exerciseIndex++;
                }

                await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, currentExercise.Exercise +" \n"+question, stepContext, _userProfileAccessor, _userState);


                return await stepContext.PromptAsync(nameof(ChoicePrompt), prompts, cancellationToken);
            }

            else
            {
                return await stepContext.EndDialogAsync(user);
            }
        }

        private bool WantsToContinue(WaterfallStepContext stepContext)
        {
            var userChoice = ((FoundChoice)stepContext.Result)?.Value;

            if (userChoice ==null)
            {
                return false;
            }

            if (userChoice == Validations.NO || userChoice == ValidationsSwahili.NO)
            {
                return false;
            }
            else if (userChoice == Validations.YES || userChoice == ValidationsSwahili.YES)
            {
                return true;
                
            }
            return false;

        }

        private async Task<DialogTurnResult> BreathingExcercisesAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var objectType = stepContext.Result.GetType();

            Client user = (Client)stepContext.Options;

            if(user.Iteration == 0)
            {
                user.Iteration = 1;
            }
            

            var userChoice = ((FoundChoice)stepContext.Result)?.Value;

            if (WantsToContinue(stepContext))
            {
                return await stepContext.BeginDialogAsync(nameof(WaterfallDialog), user, cancellationToken);
               
            }else 
            {
                return await stepContext.NextAsync(user);
            }

        }


        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //var userChoice = ((FoundChoice)stepContext.Result)?.Value;

            var user = (Client)stepContext.Options;

            //user.Iteration += user.Iteration;


            //if (userChoice !=null && userChoice==Validations.YES) return await stepContext.BeginDialogAsync(nameof(WaterfallDialog), user, cancellationToken);

            return await stepContext.EndDialogAsync(user, cancellationToken);

        }


        private static Dictionary<int, BreathingTip> GetFeelingToExerciseMap()
        {
            var paths = new[] { ".", "Cards", "Exercise.json" };


            using StreamReader reader = new(Path.Combine(paths));
            var json = reader.ReadToEnd();
            List<BreathingTip> breathingTips = JsonConvert.DeserializeObject<List<BreathingTip>>(json);


            Dictionary<int, BreathingTip> items = new();

            foreach (var item in breathingTips)
            {
                items[item.Value] = item;
            }

            return items;
        }




    }
}