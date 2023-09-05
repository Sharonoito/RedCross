using RedCrossChat.Entities;

namespace RedCrossChat.Contracts
{
    public interface IPersona :IRepositoryBase<Persona>
    {
        Task<IEnumerable<Persona>> GetAll();
        
    }
}
