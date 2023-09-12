

using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;

namespace RedCrossChat.Repository
{
    public class AppClaimRepository : RepositoryBase<AppClaim>, IAppClaimRepository
    {
        public AppClaimRepository(AppDBContext repoContext) : base(repoContext) { }
            
        public async Task<IEnumerable<AppClaim>> GetAllAsync() =>
            await FindAll().ToListAsync();

    }
}
