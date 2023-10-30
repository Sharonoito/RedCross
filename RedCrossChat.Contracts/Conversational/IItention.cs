using RedCrossChat.Entities;


namespace RedCrossChat.Contracts
{
    public interface IItention : IRepositoryBase<Intention>
    {
        Task<IEnumerable<Intention>> GetAll();

        Task<IEnumerable<Intention>> GetAllAsync();

        Task <IEnumerable<Intention>> GetByIdAsync(Guid id);

    }
}
