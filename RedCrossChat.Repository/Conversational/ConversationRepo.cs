using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;

namespace RedCrossChat.Repository
{
    public class ConversationRepo : RepositoryBase<Conversation>, IConversationRepo
    {

        public ConversationRepo(AppDBContext repoContext) : base(repoContext)
        {

        }

        public async Task<IEnumerable<Conversation>> GetAll()
        => await FindAll().ToListAsync();
    }
}
