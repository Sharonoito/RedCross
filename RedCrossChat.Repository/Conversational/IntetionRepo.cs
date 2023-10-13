using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;


namespace RedCrossChat.Repository
{
    public class IntetionRepo : RepositoryBase<Intention>, IItention
    {
        public IntetionRepo(AppDBContext repoContext) : base(repoContext) { }

        public async Task<IEnumerable<Intention>> GetAll()
            => await FindAll().ToListAsync();

        public async Task<IEnumerable<Intention>> GetAllAsync()
        { return await FindAll().ToListAsync(); }


    }
}