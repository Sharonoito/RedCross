﻿using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RedCrossChat.Dialogs
{
    public class BreathingDialog : CancelAndHelpDialog
    {

        private List<Choice> _choice;

        public BreathingDialog() : base(nameof(BreathingDialog))
        {


            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));


            var waterFallSteps = new WaterfallStep[]
            {
                InitialDialogTest,
                GroundingTechniqueStepAsync,
                SelfCompassionStepAsync,
                PositiveAffirmationAsync,
                MindfulnessStepAsync,
                StressReductionStepAsync,
                GratitudeStepAsync,
                EncouragementSupportpAsync,
                FinalStepAsync
            };


            _choice = new List<Choice>()
            {
                 new Choice() { Value = "Yes", Synonyms = new List<string> { "y", "Y", "YES", "YE", "ye", "yE", "1" } },
                  new Choice() { Value = "No", Synonyms = new List<string> { "n", "N", "no" } }
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterFallSteps));

          
            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        public async Task<DialogTurnResult> InitialDialogTest(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var prompts = new PromptOptions
            {
                Prompt = MessageFactory.Text("Feeling overwhelmed? Let's do a quick deep breathing exercise together. Inhale deeply for a count of 4, hold for 4," +
                " and exhale slowly for a count of 6. Repeat this 3-5 times. Remember to focus on your breath and let go of any tension.\""),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), prompts, cancellationToken);
        }

        private async Task<DialogTurnResult> GroundingTechniqueStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var prompts = new PromptOptions
            {
                Prompt = MessageFactory.Text("\"If anxiety is taking over, try this grounding technique: Name 5 things you can see, 4 things you can touch, " +
                "3 things you can hear, 2 things you can smell, and 1 thing you can taste. This can help you stay present and reduce stress"),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), prompts, cancellationToken);
        }

        private async Task<DialogTurnResult> SelfCompassionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var prompts = new PromptOptions
            {
                Prompt = MessageFactory.Text("Remember, it's okay to have bad days. Be kind to yourself like you would to a friend." +
                " Treat yourself with self-compassion and remember that you are not alone in this journey"),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), prompts, cancellationToken);
        }

        private async Task<DialogTurnResult> PositiveAffirmationAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var prompts = new PromptOptions
            {
                Prompt = MessageFactory.Text("Affirmations can boost your mood. Repeat after me: 'I am strong, I am resilient, " +
                "and I can handle challenges. I am worthy of love and support"),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), prompts, cancellationToken);
        }

        private async Task<DialogTurnResult> MindfulnessStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var prompts = new PromptOptions
            {
                Prompt = MessageFactory.Text("Take a moment to practice mindfulness. Close your eyes and focus on your breath, letting go of any distractions. " +
                "Notice the sensations in your body and thoughts in your mind without judgment. Bring yourself back to the present moment"),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), prompts, cancellationToken);
        }

        private async Task<DialogTurnResult> StressReductionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var prompts = new PromptOptions
            {
                Prompt = MessageFactory.Text("Feeling stressed? Engage in activities you enjoy, such as listening to music, reading a book, or spending time in nature. Taking breaks can help you recharge and improve your well-being."),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), prompts, cancellationToken);
        }

        private async Task<DialogTurnResult> GratitudeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var prompts = new PromptOptions
            {
                Prompt = MessageFactory.Text("Let's practice gratitude. Name three things you are thankful for today. " +
                "Focusing on the positive aspects of your life can improve your mood and overall mental health"),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), prompts, cancellationToken);
        }


        private async Task<DialogTurnResult> EncouragementSupportpAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var prompts = new PromptOptions
            {
                Prompt = MessageFactory.Text("Remember, you are not alone in your struggles. Reach out to friends, family," +
                " or a mental health professional if you need someone to talk to. Asking for help is a sign of strength"),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), prompts, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(cancellationToken);
        }


    }
}