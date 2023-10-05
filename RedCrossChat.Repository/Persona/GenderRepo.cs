using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;


namespace RedCrossChat.Repository
{
    public class GenderRepo : RepositoryBase<Gender>, IGender
    {
        public GenderRepo(AppDBContext repoContext) : base(repoContext){ }

        public async Task<IEnumerable<Gender>> GetAll()
        =>await FindAll().ToListAsync();

        public async Task<IEnumerable<Gender>> GetAllAsync()
        {
            return await FindAll().ToListAsync();
        }


    }
}

