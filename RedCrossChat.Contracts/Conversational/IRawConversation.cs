using RedCrossChat.Entities;


namespace RedCrossChat.Contracts
{
    public interface IRawConversation : IRepositoryBase<RawConversation>
    {
        Task<IEnumerable<RawConversation>> GetAll();
    }
}
