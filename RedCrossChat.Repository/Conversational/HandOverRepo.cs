using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;


namespace RedCrossChat.Repository
{
    public class HandOverRepo : RepositoryBase<HandOverRequest>, IHandOverRequest
    {

        public HandOverRepo(AppDBContext dBContext) : base(dBContext) { }

        public async Task<IEnumerable<HandOverRequest>> GetAll()
        {
            return await FindAll().ToListAsync();
        }
    }
}
