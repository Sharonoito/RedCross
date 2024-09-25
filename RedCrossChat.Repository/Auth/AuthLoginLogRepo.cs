using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;

namespace RedCrossChat.Repository
{
    public class AuthLoginLogRepo : RepositoryBase<AuthLoginLog>, IAuthLoginLog
    {
        public AuthLoginLogRepo(AppDBContext repoContext) : base(repoContext)
        {

        }

        public async Task<IEnumerable<AuthLoginLog>> GetAllAsync()
        {
            return await FindAll().ToListAsync();
        }
    }
}
