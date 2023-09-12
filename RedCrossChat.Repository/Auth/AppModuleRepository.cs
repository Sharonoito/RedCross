using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;

namespace RedCrossChat.Repository
{
    public class AppModuleRepository : RepositoryBase<AppModule>, IAppModuleRepository
    {
        public AppModuleRepository(AppDBContext repoContext) : base(repoContext)
        {
        }
    }
}
