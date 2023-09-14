using RedCrossChat.Entities;

namespace RedCrossChat.Contracts
{
    public interface IConversationRepo : IRepositoryBase<Conversation>
    {
       

        Task<IEnumerable<Conversation>> GetAll();
    }
}
