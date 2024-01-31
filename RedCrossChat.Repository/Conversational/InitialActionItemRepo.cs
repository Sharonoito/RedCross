using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;



namespace RedCrossChat.Repository
{
    public class InitialActionItemRepo : RepositoryBase<InitialActionItem>, IInitialActionItem
    {
        public InitialActionItemRepo(AppDBContext repoContext) : base(repoContext) { }

        public async Task<IEnumerable<InitialActionItem>> GetAll()
            => await FindAll().ToListAsync();

        public async Task<IEnumerable<InitialActionItem>> GetAllAsync()
        {
            return await FindAll().ToListAsync();   
        }

        public async Task <IEnumerable<InitialActionItem>> GetByIdAsync(Guid id)
        {
            return await FindAll().ToListAsync();
        }

    }
}

