

namespace RedCrossChat.Contracts
{

    public interface IRepositoryWrapper
    {
        #region Auth
        IUserRepository User { get; }
        IRoleRepository Role { get; }
        IAppClaimRepository AppClaim { get; }
        IAppModuleRepository AppModule { get; }

        IAppUserTeam AppUserTeam { get; }

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

        ITeam Team { get; }

        IExercise Exercise { get; }

        IItention Itention { get; }

        ISubIntention SubIntention { get; }

        IQuestionOption QuestionOption { get; }

        IHandOverRequest HandOverRequest { get; }
        
        IChatMessage ChatMessage { get; }
        #endregion

        IPersonaRepo Persona { get; }

        IIntroductionChoice IntroductionChoice { get; }

        IInitialActionItem InitialActionItem { get; }


        Task<bool> SaveChangesAsync();
    }
}
