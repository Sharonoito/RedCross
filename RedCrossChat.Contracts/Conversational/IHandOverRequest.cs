using RedCrossChat.Entities;


namespace RedCrossChat.Contracts
{
    public interface IHandOverRequest : IRepositoryBase<HandOverRequest>
    {
        Task<IEnumerable<HandOverRequest>> GetAll();
    }
   
}
