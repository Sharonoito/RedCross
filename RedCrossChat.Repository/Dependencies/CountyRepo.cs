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
    public class CountyRepo : RepositoryBase<DBCounty>, ICountyRepo
    {

        public CountyRepo(AppDBContext repoContext) : base(repoContext)
        {

        }

        async Task<IEnumerable<DBCounty>> ICountyRepo.GetAll()
        => await FindAll().ToListAsync();
    }
}
