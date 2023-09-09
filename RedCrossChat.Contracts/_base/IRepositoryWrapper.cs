using RedCrossChat.Contracts.Dependencies;

namespace RedCrossChat.Contracts
{

    public interface IRepositoryWrapper
    {
        #region Auth
        IUserRepository User { get; }
        IRoleRepository Role { get; }
        IAppClaimRepository AppClaim { get; }
        IAppModuleRepository AppModule { get; }
        #endregion

        #region Dependencies              
        IFeelingRepo Feeling { get; }
        ICountyRepo County { get; }

        #endregion

        #region Conversational
        IAiConversationRepo AiConversation { get; }
        IConversationRepo Conversation { get; }

        #endregion

        IPersonaRepo Persona { get; }
        Task<bool> SaveChangesAsync();
    }
}
