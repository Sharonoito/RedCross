using RedCrossChat.Entities;

namespace RedCrossChat.Contracts
{
    public interface ICountyRepo : IRepositoryBase<DBCounty>
    {
        Task<IEnumerable<DBCounty>> GetAll();   
    }
}
