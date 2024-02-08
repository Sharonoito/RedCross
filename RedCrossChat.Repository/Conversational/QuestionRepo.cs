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
    

    public class QuestionRepo : RepositoryBase<Question>, IQuestion
    {

        public QuestionRepo(AppDBContext repoContext) : base(repoContext)
        {

        }

        public async Task<IEnumerable<Question>> GetAll()
        => await FindAll().ToListAsync();

        public async Task <IEnumerable<Question>> GetAllAsync()
        {
            return await FindAll().ToListAsync();
        }
    }
}

 