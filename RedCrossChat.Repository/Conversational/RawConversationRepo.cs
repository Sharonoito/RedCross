using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;

namespace RedCrossChat.Repository
{
    public class RawConversationRepo : RepositoryBase<RawConversation>, IRawConversation
    {

        public RawConversationRepo(AppDBContext context):base(context) { }

        public async Task<IEnumerable<RawConversation>> GetAll()
        => await FindAll().ToListAsync();


    }
}
