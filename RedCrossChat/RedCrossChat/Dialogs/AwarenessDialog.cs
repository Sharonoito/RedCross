﻿using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using RedCrossChat.Objects;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace RedCrossChat.Dialogs
{
    public class AwarenessDialog : CancelAndHelpDialog
    {
        protected readonly ILogger _logger;
        private List<Choice> _choices;
        protected readonly UserState _userState;
        private const string UserInfo = "user-info";

        public AwarenessDialog(BreathingDialog breathingDialog, ILogger<AwarenessDialog> logger) : base(nameof(AwarenessDialog))


        {
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            AddDialog(new ChoicePrompt("select-choice"));

            AddDialog(new ChoicePrompt("select-option"));

            AddDialog(breathingDialog);


            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InitialStepAsync,
                HandleMentalValuationAsync,
                ProcessMentalEvaluationChoice,
                HandleCaregiverChoiceAsync,
                ProcessMentalEvaluationChoice,
                EvaluateDialogTurnAsync,
                ProfessionalStatusAsync,
                CheckProfessionalSwitchAsync,
                FinalStepAsync


            }));

            _choices = new List<Choice>()
                {
                     new Choice() { Value ="Yes",Synonyms=new List<string>{"y","Y","YES","YE","ye","yE","1"}},
                     new Choice() { Value="No",Synonyms=new List<string>{"n","N","no"} }
                };

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);


        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // You can create the 'User' object here or retrieve it from somewhere else.
            User user = new User();

            // Set the 'User' object in the stepContext.Values dictionary.
            stepContext.Values[UserInfo] = user;


            // Move to the next step in the waterfall.
            return await stepContext.NextAsync(null, cancellationToken);

        }


        private async Task<DialogTurnResult> HandleMentalValuationAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // await stepContext.BeginDialogAsync("mental-valuation", stepContext, cancellationToken);

            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text("Are you aware of what could have resulted to that feeling?"),

                Choices = _choices

            };

            //return await stepContext.PromptAsync("select-choice", options, cancellationToken);
            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);


        }
        private async Task<DialogTurnResult> ProcessMentalEvaluationChoice(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            User user;

            if (!stepContext.Values.TryGetValue(UserInfo, out var userValue) || !(userValue is User))
            {
                user = new User();
                stepContext.Values[UserInfo] = user;
            }
            else
            {
                user = (User)userValue;
            }

            if (stepContext.Result != null && stepContext.Result is FoundChoice choiceResult)
            {
                switch (choiceResult.Value)
                {
                    case "Yes":
                        user.isAwareOfFeeling = true;
                        return await stepContext.PromptAsync(nameof(ChoicePrompt),
                            new PromptOptions()
                            {
                                Prompt = MessageFactory.Text("Have you talked to someone about it?"),
                                Choices = _choices
                            });
                    default:
                        user.isAwareOfFeeling = false;
                        return await stepContext.PromptAsync(nameof(ChoicePrompt),
                           new PromptOptions()
                           {
                               Prompt = MessageFactory.Text("It is important to take care of your mental well-being. Would you like to have a trusted person to talk to?"),
                               Choices = _choices
                           });
                }
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


            switch (((FoundChoice)stepContext.Result).Value)
            {
                case "Yes":
                    user.hasTalkedToSomeone =true;
                    return await stepContext.PromptAsync(nameof(ChoicePrompt),
                                new PromptOptions()
                                {
                                    Prompt = MessageFactory.Text("It is good to have a trusted person, family, friend or an expert whom you can talk to. Would you like to have a trusted expert from Kenya Red Cross to talk to?"),
                                    Choices = _choices
                                });
                default:
                    //check if false
                    user.hasTalkedToSomeone =false;

                    return await stepContext.NextAsync();

            }
        }


        private async Task<DialogTurnResult> EvaluateDialogTurnAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            if (stepContext.Result == null)
            {
                await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
                {
                    Prompt = MessageFactory.Text("Would you like me to take you through some breathing exercises or tips on managing mental health?"),
                    Choices = _choices

                }, cancellationToken);

                return await stepContext.EndDialogAsync(null, cancellationToken);

            }

            switch (((FoundChoice)stepContext.Result).Value)
            {
                case "Yes":
                    return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
                    {
                        Prompt=MessageFactory.Text("What is your relationship status ?"),
                        Choices =new List<Choice>()
                        {
                            new Choice  { Value ="Single",Synonyms=new List<string>{"Single","S"}},
                            new Choice  { Value ="Married",Synonyms=new List<string>{"married"}},
                            new Choice  { Value ="Divorced",Synonyms=new List<string>{"divorced"}},
                            new Choice  { Value ="In A relationship",Synonyms=new List<string>{"dating","relations","casual"}},
                            new Choice  { Value ="Widow /Widower",Synonyms=new List<string>{"widow","widower"}},
                            new Choice  { Value ="Complicated",Synonyms=new List<string>{"complicated","comp","it's complicated"}},

                        }
                    }, cancellationToken);


                default:

                    await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
                    {
                        Prompt=MessageFactory.Text("Would you like me to take you through some breathing exercises or tips on managing mental health?"),
                        Choices= _choices

                    }, cancellationToken);

                    return await stepContext.EndDialogAsync(null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> ProfessionalStatusAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text("What is your professional status ?"),
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

        private async Task<DialogTurnResult> CheckProfessionalSwitchAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text("Would you wish to talk to a Professional Counselor?"),
                Choices = new List<Choice>()
        {
            new Choice() { Value = "agent", Synonyms = new List<string> { "Yes", "Y" } },
            new Choice() { Value = "no", Synonyms = new List<string> { "no", "n" } },
        }
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
                    case "agent":
                        user.handOverToUser = true;

                        // Send the message to the user about the next available agent or calling 1199.
                        var agentMessage = "Next available agent will be with you shortly or you can also call 1199 to connect with our counselor.";
                        await stepContext.Context.SendActivityAsync(MessageFactory.Text(agentMessage), cancellationToken);
                        break;
                    case "no":
                        user.handOverToUser = false;
                        // Start the BreathingDialog
                        return await stepContext.BeginDialogAsync(nameof(BreathingDialog), null, cancellationToken);
                    default:
                        user.handOverToUser = false;
                        break;
                }
            }

            // Continue the waterfall to the next step (if needed)
            return await stepContext.NextAsync(null, cancellationToken);
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

