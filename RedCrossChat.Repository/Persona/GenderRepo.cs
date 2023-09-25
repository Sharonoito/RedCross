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
    public class GenderRepo : RepositoryBase<Gender>, IGender
    {
        public GenderRepo(AppDBContext repoContext) : base(repoContext)
        {

        }

        public async Task<IEnumerable<Gender>> GetAll()
        =>await FindAll().ToListAsync();
    }
}
