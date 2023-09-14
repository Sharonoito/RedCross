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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace RedCrossChat.Dialogs
{
    public class AwarenessDialog : CancelAndHelpDialog
    {
        protected readonly ILogger _logger;

        private readonly IRepositoryWrapper _repository;

        private readonly IStatePropertyAccessor<ResponseDto> _userProfileAccessor;

        protected readonly UserState _userState;

        private const string UserInfo = "user-info";

        protected string CurrentQuestion = "CurrentQuestion";
        public AwarenessDialog(ILogger<AwarenessDialog> logger, IRepositoryWrapper wrapper, UserState userState) : base(nameof(AwarenessDialog))
        {

            _repository = wrapper;

            _userState = userState;

            _userProfileAccessor = userState.CreateProperty<ResponseDto>(DialogConstants.ProfileAssesor);

            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            AddDialog(new ChoicePrompt("select-choice"));

            AddDialog(new ChoicePrompt("select-option"));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), CreateWaterFallSteps()));
   
            InitialDialogId = nameof(WaterfallDialog);
        }

        private WaterfallStep[] CreateWaterFallSteps()
        {
            return new WaterfallStep[]
            {
                InitialStepAsync,
                HandleMentalValuationAsync,
                ProcessMentalEvaluationChoice,
                HandleCaregiverChoiceAsync,
                EvaluateDialogTurnAsync,
                RelationShipStatusAsync,
                ProfessionalStatusAsync,
                CheckFeelingAware,
                CheckProfessionalSwitchAsync,
                FinalStepAsync
            };
        }


        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // You can create the 'User' object here or retrieve it from somewhere else.
            User user = new();

            // Set the 'User' object in the stepContext.Values dictionary.
            stepContext.Values[UserInfo] = user;


            // Move to the next step in the waterfall.
            return await stepContext.NextAsync(null, cancellationToken);

        }


        private async Task<DialogTurnResult> HandleMentalValuationAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // await stepContext.BeginDialogAsync("mental-valuation", stepContext, cancellationToken);

            var question = "Are you aware of what could have resulted to that feeling?";

            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text(question),

                Choices = RedCrossLists.choices

            };
            await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, question, stepContext, _userProfileAccessor, _userState);


            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);


        }
        private async Task<DialogTurnResult> ProcessMentalEvaluationChoice(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            User user;


            var question = "Have you shared with someone how you feel?";

            if (!stepContext.Values.TryGetValue(UserInfo, out var userValue) || !(userValue is User user1))
            {
                user = new User();
                stepContext.Values[UserInfo] = user;
            }
            else
            {
                user = user1;
            }

            if (stepContext.Result != null && stepContext.Result is FoundChoice choiceResult)
            {

                
                await _repository.SaveChangesAsync();

                switch (choiceResult.Value)
                {
                    case "Yes":
                        user.isAwareOfFeeling = true;

                        break;
                    default:
                        user.isAwareOfFeeling = false;

                         question = " It's normal for one to feel not comfortable to share with others, however remember that a problem shared is half solved. Would you like to have a trusted person to talk to?";
                        break;
                        
                }

                await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, question, stepContext, _userProfileAccessor, _userState);


                return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
                            {
                                Prompt = MessageFactory.Text(question),
                                Choices = RedCrossLists.choices
                            }, cancellationToken);
            }
            else
            {
                // Handle the case where stepContext.Result is null or not of the correct type.
                // For example, you can prompt the user to repeat their response or handle the case accordingly.
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thank you for contacting us"), cancellationToken);

                return await stepContext.EndDialogAsync(new DialogTurnResult(DialogTurnStatus.Waiting), cancellationToken);
            }
        }


        private async Task<DialogTurnResult> HandleCaregiverChoiceAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            User user = (User)stepContext.Values[UserInfo];

            var question = "It's always relieving talking to someone trusted about what we are feeling. Would you want to speak to a professional therapist from Kenya Red Cross Society?";

            await _repository.SaveChangesAsync();

            switch (((FoundChoice)stepContext.Result).Value)
            {
                case "Yes":
                    user.hasTalkedToSomeone =true;
                    break;
                default:
                   
                    user.hasTalkedToSomeone =false;

                    if (!user.hasTalkedToSomeone && !user.isAwareOfFeeling)
                    {
                        return await stepContext.EndDialogAsync(user);
                    }
                    break;
                    

            }

            await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, question, stepContext, _userProfileAccessor, _userState);


            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                               new PromptOptions()
                               {
                                   Prompt = MessageFactory.Text(question),
                                   Choices = RedCrossLists.choices
                               });
        }


        private async Task<DialogTurnResult> EvaluateDialogTurnAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            User user = (User)stepContext.Values[UserInfo];

            if (stepContext.Result == null)
            {
                return await stepContext.EndDialogAsync(user, cancellationToken);
            }

            switch (((FoundChoice)stepContext.Result).Value)
            {
                case "Yes":
                    return await stepContext.NextAsync(user, cancellationToken);
                default:
                    user.WantsBreathingExercises = true;
                    return await stepContext.EndDialogAsync(user, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> RelationShipStatusAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var question = "What is your relationship status ?";

            await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, question, stepContext, _userProfileAccessor, _userState);

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

                        }
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> ProfessionalStatusAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var question = "What is your professional status ?";
            
            await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, question, stepContext, _userProfileAccessor, _userState);

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

                        }
            }, cancellationToken);

        }

        public async Task<DialogTurnResult> CheckFeelingAware(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var question = "What makes you seek our psychological support?";

            await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, question, stepContext, _userProfileAccessor, _userState);

            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text(question),
                Choices = new List<Choice>()
                        {
                            new Choice  { Value ="Suicidal ideations",},
                            new Choice  { Value ="Feelings of hopelessness",},
                            new Choice  { Value ="Financial distress"},
                            new Choice  { Value ="Childhood trauma"},
                            new Choice  { Value ="Work related stress and burnout"},
                           

                        }
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> CheckProfessionalSwitchAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var question = "Would you wish to talk to a Professional Counselor?";

            await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, question, stepContext, _userProfileAccessor, _userState);

            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text(question),
                Choices = RedCrossLists.choices,
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            // Evaulate if to push the user to the health ai bot
            User user = (User)stepContext.Values[UserInfo];

            if (stepContext.Result != null)
            {
                switch (((FoundChoice)stepContext.Result).Value)
                {
                    case Validations.YES:

                        user.wantstoTalkToAProfessional= true;

                        user.handOverToUser = true;

                        // Send the message to the user about the next available agent or calling 1199.
                        var agentMessage = "The next available counsellor will call you shortly, you can also contact us directly by dialing 1199, request to speak to a psychologist.";
                        await stepContext.Context.SendActivityAsync(MessageFactory.Text(agentMessage), cancellationToken);


                        var hotline = PersonalDialogCard.GetHotlineCard();

                        var attachment = new Attachment
                        {
                            ContentType = HeroCard.ContentType,
                            Content =   hotline
                        };

                        var message = MessageFactory.Attachment(attachment);
                        await stepContext.Context.SendActivityAsync(message, cancellationToken);

                        var choices = new List<Choice>
                        {
                            new Choice() { Value = "hotline", Action = new CardAction() { Title = "hotline", Type = ActionTypes.OpenUrl, Value = "https://referraldirectories.redcross.or.ke/" } }
                        };


                        return await stepContext.EndDialogAsync(user, cancellationToken);


                    case Validations.NO:
                        user.handOverToUser = false;
                        // Start the BreathingDialog
                        return await stepContext.BeginDialogAsync(nameof(BreathingDialog), user, cancellationToken);
                    default:
                        user.handOverToUser = false;
                        break;
                }
            }

            return await stepContext.EndDialogAsync(user);
        }

    }
}


