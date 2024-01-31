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
    public class TeamRepo : RepositoryBase<Team>, ITeam
    {
        public TeamRepo(AppDBContext repoContext):base(repoContext) { }

        public async Task<IEnumerable<Team>> GetAll()
        => await FindAll().ToListAsync();

        public async Task<IEnumerable<Team>>GetAllAsync()
        {  return await FindAll().ToListAsync(); 
        }
    }
}

















