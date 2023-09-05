using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;

namespace RedCrossChat.Repository
{
    public class AiConversationRepo : RepositoryBase<AiConversation>, IAiConversationRepo
    {

        public AiConversationRepo(AppDBContext repoContext) : base(repoContext)
        {

        }

        public async Task<IEnumerable<AiConversation>> GetAll()
        =>await FindAll().ToListAsync();
    }
}
