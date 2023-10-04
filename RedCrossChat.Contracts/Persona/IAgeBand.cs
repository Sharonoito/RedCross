using RedCrossChat.Entities;


namespace RedCrossChat.Contracts
{
    public interface IAgeBand : IRepositoryBase<AgeBand>
    {
        Task<IEnumerable<AgeBand>> GetAll();

        Task<IEnumerable<AgeBand>> GetAllAsync();

    }

}






