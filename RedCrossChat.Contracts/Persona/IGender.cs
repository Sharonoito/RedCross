using RedCrossChat.Entities;


namespace RedCrossChat.Contracts
{
    public interface IGender : IRepositoryBase<Gender>
    {
        Task<IEnumerable<Gender>> GetAll();

        Task<IEnumerable<Gender>> GetAllAsync();
    }
}



