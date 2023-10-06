using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossChat.Repository
{
    public class AppUserTeamRepo : RepositoryBase<AppUserTeam>,IAppUserTeam
    {
        public AppUserTeamRepo(AppDBContext repoContext):base(repoContext) { }

        public async Task<IEnumerable<AppUserTeam>> GetAll()
        => await FindAll().ToListAsync();

        public async Task<IEnumerable<AppUserTeam>> GetAllAsync()
        {  return await FindAll().ToListAsync(); }
    }
}




