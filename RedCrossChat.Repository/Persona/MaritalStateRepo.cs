using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;


namespace RedCrossChat.Repository
{
    public class MaritalStateRepo : RepositoryBase<MaritalState>,IMaritalState
    {

        public MaritalStateRepo(AppDBContext appDBContext) : base(appDBContext) { }

        public async Task<IEnumerable<MaritalState>> GetAll()
         => await FindAll().ToListAsync();
    }
}
