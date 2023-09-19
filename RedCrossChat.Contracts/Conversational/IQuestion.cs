using RedCrossChat.Entities;


namespace RedCrossChat.Contracts
{
    public interface IQuestion : IRepositoryBase<Question>
    {
        Task<IEnumerable<Question>> GetAll();
    }
}
