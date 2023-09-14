using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts.Dependencies;
using RedCrossChat.Domain;
using RedCrossChat.Entities;
namespace RedCrossChat.Repository
{
    public class FeelingRepo : RepositoryBase<DBFeeling>, IFeelingRepo
    {
        public FeelingRepo(AppDBContext repoContext) : base(repoContext)
        {

        }

        public async Task<IEnumerable<DBFeeling>> GetAll()
        => await FindAll().ToListAsync();
    }
}
