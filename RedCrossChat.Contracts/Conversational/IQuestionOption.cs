using RedCrossChat.Entities;

namespace RedCrossChat.Contracts
{
    public interface IQuestionOption : IRepositoryBase<QuestionOption>
    {
        Task<IEnumerable<QuestionOption>> GetAll();
    }
}
