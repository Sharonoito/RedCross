using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;


using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace RedCrossChat.Repository
{
    public class AgeBandRepo : RepositoryBase<AgeBand>,IAgeBand
    {
        public AgeBandRepo(AppDBContext appDBContext):base(appDBContext) { }

        public async Task<IEnumerable<AgeBand>> GetAll()
        => await FindAll().ToListAsync();

        public async Task<IEnumerable<AgeBand>> GetAllAsync()
        {
            return await FindAll().ToListAsync();
        }


    }
}




