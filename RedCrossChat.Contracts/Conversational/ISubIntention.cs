using RedCrossChat.Entities;


namespace RedCrossChat.Contracts
{
    public interface ISubIntention: IRepositoryBase<SubIntention>
    {
        //Task Add(SubIntention subIntention);
        Task<IEnumerable<SubIntention>> GetAll();

        Task<IEnumerable<SubIntention>> GetAllAsync();

    }
}
