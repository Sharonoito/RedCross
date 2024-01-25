using RedCrossChat.Entities;
using RedCrossChat.Entities.Auth;

namespace RedCrossChat.Contracts
{
    public interface IRoleRepository : IRepositoryBase<AppRole>
    {
        Task<IEnumerable<AppRole>> GetAllUsersAsync();
        Task<IEnumerable<AppRole>> GetAllAsync();

    }
}

