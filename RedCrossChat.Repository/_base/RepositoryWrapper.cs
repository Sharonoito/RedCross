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
