using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;

namespace RedCrossChat.Repository
{
    public class PersonaRepo : RepositoryBase<Persona>, IPersona
    {


        public PersonaRepo(AppDBContext repoContext) : base(repoContext)
        {

        }

        async Task<IEnumerable<Persona>> IPersona.GetAll()
        =>await FindAll().ToListAsync();
    }
}
