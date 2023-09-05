using RedCrossChat.Contracts.Dependencies;

namespace RedCrossChat.Contracts
{
    public interface IRepositoryWrapper
    {
        #region Dependencies              
        IFeelingRepo Feeling { get; }
        ICountyRepo County { get; }

        #endregion

        #region Conversational
        IAiConversationRepo AiConversation { get; }
        IConversationRepo Conversation { get; }

        #endregion
    }
}
