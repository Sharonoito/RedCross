using RedCrossChat.Entities;

namespace RedCrossChat.Contracts
{
    public interface IPersonaRepo :IRepositoryBase<Persona>
    {
        Task<IEnumerable<Persona>> GetAll();
        
    }
}
