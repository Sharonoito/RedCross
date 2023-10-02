using RedCrossChat.Contracts;

namespace RedCrossChat.Dialogs
{
    public class HumanHandOverDialog : CancelAndHelpDialog
    {
        private IRepositoryWrapper repository;

        public HumanHandOverDialog(IRepositoryWrapper repository):base(nameof(HumanHandOverDialog)) { 


        }

    }
}
