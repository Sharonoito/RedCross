using RedCrossChat.Entities;


namespace RedCrossChat.Contracts
{
    public interface IMaritalState:IRepositoryBase<MaritalState>
    {
        Task<IEnumerable<MaritalState>> GetAll();
    }
}
