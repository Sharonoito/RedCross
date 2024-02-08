using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;



namespace RedCrossChat.Repository
{
    public class IntroductionChoiceRepo : RepositoryBase<IntroductionChoice>, IIntroductionChoice
    {
        public IntroductionChoiceRepo(AppDBContext repoContext) : base(repoContext) { }

        public async Task<IEnumerable<IntroductionChoice>> GetAll()
            => await FindAll().ToListAsync();

        public async Task<IEnumerable<IntroductionChoice>> GetAllAsync()
        {
            return await FindAll().ToListAsync();   
        }

        public async Task <IEnumerable<IntroductionChoice>> GetByIdAsync(Guid id)
        {
            return await FindAll().ToListAsync();
        }

    }
}

