using RedCrossChat.Entities;

namespace RedCrossChat.Contracts
{
    public interface IUserRepository : IRepositoryBase<AppUser>
    {
        Task<IEnumerable<AppUser>> GetAllUsersAsync();
    }
}
