using RedCrossChat.Entities;

namespace RedCrossChat.Contracts
{
    public interface IAppClaimRepository : IRepositoryBase<AppClaim>
    {
        Task<IEnumerable<AppClaim>> GetAllAsync();
    }
}
