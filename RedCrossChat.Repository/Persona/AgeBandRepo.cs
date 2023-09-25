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
    public class AgeBandRepo : RepositoryBase<AgeBand>,IAgeBand
    {
        public AgeBandRepo(AppDBContext appDBContext):base(appDBContext) {
        
        }

        public async Task<IEnumerable<AgeBand>> GetAll()
        => await FindAll().ToListAsync();
    }
}
