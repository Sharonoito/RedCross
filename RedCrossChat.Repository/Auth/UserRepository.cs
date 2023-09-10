using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;

namespace RedCrossChat.Repository
{
    public class UserRepository : RepositoryBase<AppUser>, IUserRepository
    {
        public UserRepository(AppDBContext repoContext) : base(repoContext) {}

        public async Task<IEnumerable<AppUser>> GetAllAsync()
        {
            return await FindAll().ToListAsync();
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersAsync() =>
            await FindAll().ToListAsync();
    }
}
