using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;
using RedCrossChat.Entities.Auth;

namespace RedCrossChat.Repository
{
    public class RoleRepository : RepositoryBase<AppRole>, IRoleRepository
    {
        public RoleRepository(AppDBContext repoContext) : base(repoContext)
        {
        }

        public async Task<IEnumerable<AppRole>> GetAll()
            => await FindAll().ToListAsync();

        public async Task<IEnumerable<AppRole>> GetAllAsync()
        {
            return await FindAll().ToListAsync();
        }

        public async Task<IEnumerable<AppRole>> GetAllUsersAsync()
            => await FindAll().ToListAsync();
    }
}
