using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;


namespace RedCrossChat.Repository
{
    public class ProfessionRepo : RepositoryBase<Profession> , IProfession
    {
        public ProfessionRepo(AppDBContext appDBContext) : base(appDBContext) { }

        public async Task<IEnumerable<Profession>> GetAll()
        => await FindAll().ToListAsync();

    }
}
