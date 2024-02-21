using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using RedCrossChat.Contracts;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace RedCrossChat.Dialogs
{
    public class CancelAndHelpDialog : ComponentDialog
    {
        private const string HelpMsgText = "Do you wish to talk to an agent?";
        private const string CancelMsgText = "Cancelling...";
        // //stepContext.Context.Activity.Conversation.Id  2bf7bf00-62c5-11ee-a514-cbe89faff2c0|livechat 2bf7bf00-62c5-11ee-a514-cbe89faff2c0|livechat
        public CancelAndHelpDialog(string id,BaseDialog baseDialog, IRepositoryWrapper wrapper) :base(id) {
        
            AddDialog(baseDialog);
        }
        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken = default)
        {
            var result = await InterruptAsync(innerDc, cancellationToken);
            if (result != null)
            {
                return result;
            }

            return await base.OnContinueDialogAsync(innerDc, cancellationToken);
        }





        private static async Task<DialogTurnResult> InterruptAsync(DialogContext innerDc, CancellationToken cancellationToken)
        {
            //if (innerDc.Context.Activity.Type == ActivityTypes.Message)

            var text=innerDc.Context.Activity.Text.ToLowerInvariant();


            if(!string.IsNullOrEmpty(text) &&  innerDc.Context.Activity.ChannelId=="telegram"  && text.ToLower() == "/start")
            {

                var cancelMessage = MessageFactory.Text("the conversation has been Cancelled type anything to restart", CancelMsgText, InputHints.IgnoringInput);

                await innerDc.Context.SendActivityAsync(cancelMessage, cancellationToken);

                await innerDc.CancelAllDialogsAsync(cancellationToken);

            }

            if (!string.IsNullOrEmpty(text) && (text.ToLower() == "help" || text.ToLower() == "cancel" || text.ToLower() == "exit" || text.ToLower()== "quit")     )
            {
               // var text = innerDc.Context.Activity.Text.ToLowerInvariant();

                switch (text)
                {
                    case "help":
                    case "?":
                        var helpMessage = MessageFactory.Text(HelpMsgText, HelpMsgText, InputHints.ExpectingInput);
                        await innerDc.Context.SendActivityAsync(helpMessage, cancellationToken);
                        return new DialogTurnResult(DialogTurnStatus.Waiting);

                    case "cancel":
                    case "exit":
                    case "close":
                    case "quit":
                    case "goodbye":
                    case "good bye":
                        
                        //var cancelMessage = MessageFactory.Text(CancelMsgText, CancelMsgText, InputHints.IgnoringInput);
                        
                       // await innerDc.Context.SendActivityAsync(cancelMessage, cancellationToken);
                         
                        await innerDc.CancelAllDialogsAsync(cancellationToken);

                        return await innerDc.BeginDialogAsync(nameof(BaseDialog));
                }
            }

            return null;
        }

    }
}
