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
    public class AIRejectedQuestionRepo
    : RepositoryBase<AIRejectedQuestion>, IAIRejectedQuestion
    {
        public AIRejectedQuestionRepo(AppDBContext repoContext) : base(repoContext)
        {

        }

        public async Task<IEnumerable<AIRejectedQuestion>> GetAll()
        => await FindAll().ToListAsync();
    }
}
