using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace RedCrossChat.Repository
{
    public class FeelingRepo : RepositoryBase<DBFeeling>, IFeelingRepo
    {

        public FeelingRepo(AppDBContext repoContext) : base(repoContext) { }

        public Task<IEnumerable<DBFeeling>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<DBFeeling>> GetAllAsync()
        {
            return await FindAll().ToListAsync();
        }



    }
}









