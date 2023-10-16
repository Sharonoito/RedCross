using RedCrossChat.Entities;


namespace RedCrossChat.Contracts
{
    public interface ISubIntention: IRepositoryBase<SubIntention>
    {
        Task<IEnumerable<SubIntention>> GetAll();

        Task<IEnumerable<SubIntention>> GetAllAsync();

    }
}
