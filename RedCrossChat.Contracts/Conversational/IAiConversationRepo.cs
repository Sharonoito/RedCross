using RedCrossChat.Entities;


namespace RedCrossChat.Contracts
{
    public interface IAiConversationRepo : IRepositoryBase<AiConversation>
    {
        Task<IEnumerable<AiConversation>> GetAll();
    }
}
