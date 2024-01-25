using RedCrossChat.Entities;


namespace RedCrossChat.Contracts
{
    public interface IIntroductionChoice : IRepositoryBase<IntroductionChoice>
    {
        Task<IEnumerable<IntroductionChoice>> GetAll();

        Task<IEnumerable<IntroductionChoice>> GetAllAsync();

        Task <IEnumerable<IntroductionChoice>> GetByIdAsync(Guid id);

    }
}
