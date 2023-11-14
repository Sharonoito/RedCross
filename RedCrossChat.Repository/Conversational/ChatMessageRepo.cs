using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;


namespace RedCrossChat.Repository
{
    public class ChatMessageRepo : RepositoryBase<ChatMessage>, IChatMessage
    {
        public ChatMessageRepo(AppDBContext repoContext) : base(repoContext)
        {

        }

        public async Task<IEnumerable<ChatMessage>> GetAll()
        => await FindAll().ToListAsync();
    }
}
