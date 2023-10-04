using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using NuGet.Protocol.Core.Types;
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

        public AwarenessDialog(
            ILogger<AwarenessDialog> logger, 
            IRepositoryWrapper wrapper, UserState userState,
            HumanHandOverDialog humanHandOverDialog
            ) : base(nameof(AwarenessDialog))
        {

            _repository = wrapper;

            _userState = userState;

            _userProfileAccessor = userState.CreateProperty<ResponseDto>(DialogConstants.ProfileAssesor);

            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            AddDialog(new ChoicePrompt("select-choice"));

            AddDialog(new ChoicePrompt("select-option"));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), CreateWaterFallSteps()));

            AddDialog(humanHandOverDialog);
   
            InitialDialogId = nameof(WaterfallDialog);
        }

        private WaterfallStep[] CreateWaterFallSteps()
        {
            return new WaterfallStep[]
            {
                InitialStepAsync,
                
                ProcessMentalEvaluationChoice,
                HandleCaregiverChoiceAsync,
                EvaluateDialogTurnAsync,
                CheckFeelingAware,
                CheckProfessionalSwitchAsync,
                FinalStepAsync
            };
        }


        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var me = (Client)stepContext.Options;

            stepContext.Values[UserInfo] = me;

            // Move to the next step in the waterfall.
          
            var question = me.language ? "Are you aware of what could have resulted to that feeling?" : " Je, unafahamu kinachopelekea ujihisi katika hali ya (furaha, huzuni)? Taja";

            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text(question),

                Choices = me.language ? RedCrossLists.choices : RedCrossLists.choicesKiswahili,
                Style = ListStyle.HeroCard


            };


            await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, question, stepContext, _userProfileAccessor, _userState);


            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);

        }

        private async Task<DialogTurnResult> ProcessMentalEvaluationChoice(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client me = (Client)stepContext.Values[UserInfo];

            var question = me.language ? "Have you shared with someone how you feel?" : " Je, umeweza kuzungumza na mtu yeyote?";


            if (stepContext.Result != null && stepContext.Result is FoundChoice choiceResult)
            {

                Persona persona = await _repository.Persona.FindByCondition(x => x.SenderId == stepContext.Context.Activity.From.Id).FirstAsync();

                persona.IsAwareOfFeelings = stepContext.Context.Activity.Text;

                _repository.Persona.Update(persona);

                await _repository.SaveChangesAsync();

                switch (choiceResult.Value)
                {
                    case Validations.YES:
                    case ValidationsSwahili.YES:
                        me.IsAwareOfFeeling = true;

                        break;
                    default:
                      
                         question =me.language? " It's normal for one to feel not comfortable to share with others, however remember that a problem shared is half solved. Would you like to have a trusted person to talk to?":
                            "Ni muhimu kutunza ustawi wako wa akili. Je, ungependa kuwa na mtu unayemwamini wa kuzungumza naye? Ni kawaida kwa mtu kujisikia kukosa raha kushiriki na wengine, hata hivyo kumbuka kuwa shida iliyoshirikiwa hutatuliwa nusu";
                        break;
                        
                }

                await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, question, stepContext, _userProfileAccessor, _userState);


                return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
                            {
                                Prompt = MessageFactory.Text(question),
                                Choices = me.language ? RedCrossLists.choices : RedCrossLists.choicesKiswahili,
                                Style = ListStyle.HeroCard
                },
                            cancellationToken);
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

            Client me = (Client)stepContext.Values[UserInfo];

            var question = me.language ?"It's always relieving talking to someone trusted about what we are feeling. Would you want to speak to a professional therapist from Kenya Red Cross Society?":
                "Daima ni kutuliza kuzungumza na mtu anayeaminika kuhusu kile tunachohisi. Je, ungependa kuzungumza na mtaalamu wa tiba kutoka Chama cha Msalaba Mwekundu cha Kenya";

            Persona persona = await _repository.Persona.FindByCondition(x => x.SenderId == stepContext.Context.Activity.From.Id).FirstAsync();

            persona.HasTalkedTOSomeone = true;

            _repository.Persona.Update(persona);

            await _repository.SaveChangesAsync();

            switch (((FoundChoice)stepContext.Result).Value)
            {
                case "Yes":
                    me.HasTalkedToSomeone =true;
                    break;
                default:

                    if (!me.HasTalkedToSomeone && !me.IsAwareOfFeeling)
                    {
                        return await stepContext.EndDialogAsync(me);
                    }
                    break;
                    

            }

            await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, question, stepContext, _userProfileAccessor, _userState);


            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                               new PromptOptions()
                               {
                                   Prompt = MessageFactory.Text(question),
                                   Choices = me.language? RedCrossLists.choices : RedCrossLists.choicesKiswahili,
                                   Style = ListStyle.HeroCard

                               });
        }


        private async Task<DialogTurnResult> EvaluateDialogTurnAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            Client me = (Client)stepContext.Values[UserInfo];

            Persona persona = await _repository.Persona.FindByCondition(x => x.SenderId == stepContext.Context.Activity.From.Id).FirstAsync();


            if (stepContext.Result == null)
            {
                persona.WantsToTalkToSomeone = true;

                _repository.Persona.Update(persona);

                await _repository.SaveChangesAsync();


                return await stepContext.EndDialogAsync(me, cancellationToken);
            }

            switch (((FoundChoice)stepContext.Result).Value)
            {
                case Validations.YES:case ValidationsSwahili.YES:
                    return await stepContext.NextAsync(me, cancellationToken);
                default:
                    me.WantsBreathingExercises = true;
                    return await stepContext.EndDialogAsync(me, cancellationToken);
            }
        }

        

        public async Task<DialogTurnResult> CheckFeelingAware(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client me = (Client)stepContext.Values[UserInfo];

            var question = me.language ? "What makes you seek our psychological support?" : "Ni nini kinakufanya utafute msaada wetu wa kisaikolojia?";

            Persona persona = await _repository.Persona.FindByCondition(x => x.SenderId == stepContext.Context.Activity.From.Id).FirstAsync();

            persona.WantsBreathingExcercises = true;

            _repository.Persona.Update(persona);

            await _repository.SaveChangesAsync();

            await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, question, stepContext, _userProfileAccessor, _userState);

            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text(question),
                Choices =me.language ? RedCrossLists.Reasons : RedCrossLists.ReasonsKiswahili,
                Style = ListStyle.HeroCard
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> CheckProfessionalSwitchAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client me = (Client)stepContext.Values[UserInfo];


            var question = me.language ? "Would you wish to talk to a Professional Counselor?" : "Je, ungependa kuongea na mshauri wa kitaalam?";

            var conversation = await _repository.Conversation.FindByCondition(x => x.ConversationId == stepContext.Context.Activity.Conversation.Id).FirstOrDefaultAsync();

            if(conversation !=null)
            {
                conversation.Reason = stepContext.Context.Activity.Text;

                _repository.Conversation.Update(conversation);

                await _repository.SaveChangesAsync();
            }
           
            await DialogExtensions.UpdateDialogAnswer(stepContext.Context.Activity.Text, question, stepContext, _userProfileAccessor, _userState);

            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text(question),
                Choices = me.language? RedCrossLists.choices : RedCrossLists.choicesKiswahili,
                Style = ListStyle.HeroCard

            }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Client me = (Client)stepContext.Values[UserInfo];

            var conversation = await _repository.Conversation.FindByCondition(x => x.ConversationId == stepContext.Context.Activity.Conversation.Id).FirstOrDefaultAsync();

            if (stepContext.Result != null)
            {
                switch (((FoundChoice)stepContext.Result).Value)
                {
                    case Validations.YES:
                    case ValidationsSwahili.YES:

                        me.WantstoTalkToAProfessional= true;

                        me.HandOverToUser = true;

                        /*Updating the database*/

                        if (conversation != null)
                        {
                            conversation.HandedOver = true;

                            _repository.Conversation.Update(conversation);

                            await _repository.SaveChangesAsync();
                        }

                        // Send the message to the user about the next available agent or calling 1199.
                        var agentMessage = me.language ? "The next available counsellor will call you shortly, you can also contact us directly by dialing 1199, request to speak to a psychologist.":
                            "Utaweza kuzungumza na mhudumu baada ya muda mfupi ama pia unaweza piga nambari 1199 ili kuongea na mshauri. Utaweza kupigiwa na mshauri baada ya muda mfupi, ama upige simu ili kuongea na mwanasaikolojia kupitia nambari 1199";
                        await stepContext.Context.SendActivityAsync(MessageFactory.Text(agentMessage), cancellationToken);


                        var hotline = PersonalDialogCard.GetHotlineCard();
                        var hotlineSwahili = PersonalDialogCard.GetHotlineCardKiswahili();


                        var attachment = new Attachment
                        {
                            ContentType = HeroCard.ContentType,
                 
                            Content = !me.language ? hotlineSwahili : hotline,
                        };


                        var message = MessageFactory.Attachment(attachment);
                        await stepContext.Context.SendActivityAsync(message, cancellationToken);

                        var choices = new List<Choice>
                        {
                            new Choice() { Value = "hotline", Action = new CardAction() { Title = "hotline", Type = ActionTypes.OpenUrl, Value = "https://referraldirectories.redcross.or.ke/" } }
                        };

                       // return await stepContext.BeginDialogAsync(nameof(HumanHandOverDialog), me, cancellationToken);

                        return await stepContext.EndDialogAsync(me, cancellationToken);


                    case Validations.NO:
                    case ValidationsSwahili.NO:
                       
                        return await stepContext.BeginDialogAsync(nameof(BreathingDialog), me, cancellationToken);
                    default:
                        
                        break;
                }
            }

            return await stepContext.EndDialogAsync(me);
        }


}
}




