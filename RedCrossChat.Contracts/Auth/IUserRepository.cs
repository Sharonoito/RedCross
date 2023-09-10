using RedCrossChat.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RedCrossChat.Contracts
{
    public interface IUserRepository : IRepositoryBase<AppUser>
    {
        Task<IEnumerable<AppUser>> GetAllUsersAsync();

        Task<IEnumerable<AppUser>> GetAllAsync();
    }
}
