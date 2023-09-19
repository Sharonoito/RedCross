using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;


namespace RedCrossChat.Repository
{
    public class QuestionOptionRepo : RepositoryBase<QuestionOption>, IQuestionOption
    {

        public QuestionOptionRepo(AppDBContext repoContext) : base(repoContext)
        {

        }

        public async Task<IEnumerable<QuestionOption>> GetAll()
        => await FindAll().ToListAsync();
    }
}
