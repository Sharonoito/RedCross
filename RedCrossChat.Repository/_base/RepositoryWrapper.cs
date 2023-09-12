using RedCrossChat.Contracts;
using RedCrossChat.Contracts.Dependencies;
using RedCrossChat.Domain;
using RedCrossChat.Entities;
using RedCrossChat.Repository;

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

        public IPersonaRepo? _persona;

        public IPersonaRepo Persona
        {
            get
            {
                if (_persona == null) { _persona = new PersonaRepo(_repoContext); }

                return _persona;
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
