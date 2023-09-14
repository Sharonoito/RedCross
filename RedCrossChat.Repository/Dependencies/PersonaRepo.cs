using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;

namespace RedCrossChat.Repository
{
    public class PersonaRepo : RepositoryBase<Persona>, IPersonaRepo
    {


        public PersonaRepo(AppDBContext repoContext) : base(repoContext)
        {

        }

        async Task<IEnumerable<Persona>> IPersonaRepo.GetAll()
        =>await FindAll().ToListAsync();
    }
}
