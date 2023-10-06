using RedCrossChat.Contracts;


using RedCrossChat.Domain;



namespace RedCrossChat.Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly AppDBContext _repoContext;

        public RepositoryWrapper(AppDBContext repoContext)
        {
            _repoContext = repoContext;
        }

        #region Auth
        private IUserRepository? _user;
        public IUserRepository User
        {
            get
            {
                if (_user == null) _user = new UserRepository(_repoContext);
                return _user;
            }
        }

        private IRoleRepository? _role;
        public IRoleRepository Role
        {
            get
            {
                if (_role == null) _role = new RoleRepository(_repoContext);
                return _role;
            }
        }

        private IAppClaimRepository? _appClaim;
        public IAppClaimRepository AppClaim
        {
            get
            {
                if (_appClaim == null) _appClaim = new AppClaimRepository(_repoContext);
                return _appClaim;
            }
        }

        private IAppModuleRepository? _appModule;
        public IAppModuleRepository AppModule
        {
            get
            {
                if (_appModule == null) _appModule = new AppModuleRepository(_repoContext);
                return _appModule;
            }
        }

        public IAppUserTeam _appUserTeam;

        public IAppUserTeam GetQuestion()
        {

            if (_appUserTeam==null) { _appUserTeam = new AppUserTeamRepo(_repoContext); }

            return _appUserTeam;
        }


        #endregion


        public IFeelingRepo? _feeling;
        public IFeelingRepo Feeling
        {

            get{
                if(_feeling == null) { _feeling = new FeelingRepo(_repoContext); }

                return _feeling;
            }
        }

        public ICountyRepo? _county;

        public ICountyRepo County
        {
            get
            {
                if (_county == null) { _county = new CountyRepo(_repoContext); }

                return _county;
            }
        }

        public IGender? _gender;
        public IGender Gender
        {
            get
            {
                if (_gender == null)  _gender = new GenderRepo(_repoContext);

                return _gender;
            }
        }

        public IAgeBand? _ageBand;
        public IAgeBand AgeBand
        {
            get
            {
                if (_ageBand == null) _ageBand = new AgeBandRepo(_repoContext);

                return _ageBand;
            }
        }

        public IMaritalState? _maritalState;
        public IMaritalState MaritalState
        {
            get
            {
                if(_maritalState==null) _maritalState= new MaritalStateRepo(_repoContext);

                return _maritalState;
            }
        }

        public IProfession? _profession;
        public IProfession Profession
        {
            get
            {
                if(_profession==null) _profession= new ProfessionRepo(_repoContext);

                return _profession;
            }
        }



        public IAiConversationRepo? _aiConversation;
        public IAiConversationRepo AiConversation
        {
            get
            {
                if (_aiConversation==null) { _aiConversation = new AiConversationRepo(_repoContext); }

                return _aiConversation;
            }
        }

        public IConversationRepo? _conversation;
        public IConversationRepo Conversation
        {
            get
            {
                if (_conversation == null) { _conversation = new ConversationRepo(_repoContext);  }

                return _conversation;
            }
        }

        public IQuestion _question;
        public IQuestion Question
        {
            get { 
                
                if(_question==null) { _question = new QuestionRepo(_repoContext);  }

                return _question;
            }
        }

        public IQuestionOption _questionOption;
        public IQuestionOption QuestionOption
        {
            get
            {

                if (_questionOption == null) { _questionOption = new QuestionOptionRepo(_repoContext); }

                return _questionOption;
            }
        }

        public IPersonaRepo? _persona;

        public IPersonaRepo Persona
        {
            get
            {
                if (_persona == null) { _persona = new PersonaRepo(_repoContext); }

                return _persona;
            }
        }

        public ITeam _team;
        public ITeam Team
        {
            get
            {

                if (_team==null) { _team = new TeamRepo(_repoContext); }

                return _team;
            }
        }


        public IRawConversation _rawConversation;

        public IRawConversation RawConversation
        {
            get
            {
                if (_rawConversation == null) { _rawConversation = new RawConversationRepo(_repoContext); }

                return _rawConversation;
            }
        }

        public IHandOverRequest _handOverRequest;

        public IHandOverRequest HandOverRequest
        {
            get
            {
                if (_handOverRequest == null)
                {
                    _handOverRequest = new HandOverRepo(_repoContext);

                }

                return _handOverRequest;    
            }
        }
      

        public async Task<bool> SaveChangesAsync()
        {
            if (await _repoContext.SaveChangesAsync() > 0)
                return true;

            return false;
        }

        public void DetachAllEntities()
        {
            throw new System.NotImplementedException();
        }
    }
}
