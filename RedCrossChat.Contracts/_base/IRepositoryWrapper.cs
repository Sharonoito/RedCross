﻿


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

        IGender Gender { get; }
        IAgeBand AgeBand { get; }

        IProfession Profession { get; } 

        IMaritalState MaritalState { get; }

        #endregion

        #region Conversational
        IAiConversationRepo AiConversation { get; }
        IConversationRepo Conversation { get; }
        IRawConversation RawConversation { get; }

        IQuestion Question { get; }

        IQuestionOption QuestionOption { get; }
        #endregion

        IPersonaRepo Persona { get; }
        Task<bool> SaveChangesAsync();
    }
}