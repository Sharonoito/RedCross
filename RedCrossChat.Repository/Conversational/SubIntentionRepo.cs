using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;


namespace RedCrossChat.Repository
{
    public class SubIntentionRepo : RepositoryBase<SubIntention>, ISubIntention
    {
        public SubIntentionRepo(AppDBContext repoContext) : base(repoContext)
        {

        }

        public async Task<IEnumerable<SubIntention>> GetAll()
    => await FindAll().ToListAsync();

        public async Task<IEnumerable<SubIntention>> GetAllAsync()
        { return await FindAll().ToListAsync(); }


    }
}
