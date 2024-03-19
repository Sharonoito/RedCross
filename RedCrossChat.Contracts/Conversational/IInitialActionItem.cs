using RedCrossChat.Entities;


namespace RedCrossChat.Contracts
{
    public interface IInitialActionItem : IRepositoryBase<InitialActionItem>
    {
        Task<IEnumerable<InitialActionItem>> GetAll();

        Task<IEnumerable<InitialActionItem>> GetAllAsync();

        Task <IEnumerable<InitialActionItem>> GetByIdAsync(Guid id);

    }
}
