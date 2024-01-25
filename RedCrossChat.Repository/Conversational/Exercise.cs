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
    

    public class ExerciseRepo : RepositoryBase<Exercise>, IExercise
    {

        public ExerciseRepo(AppDBContext repoContext) : base(repoContext)
        {

        }

        public async Task<IEnumerable<Exercise>> GetAll()
        => await FindAll().ToListAsync();

        public async Task <IEnumerable<Exercise>> GetAllAsync()
        {
            return await FindAll().ToListAsync();
        }
    }
}

 