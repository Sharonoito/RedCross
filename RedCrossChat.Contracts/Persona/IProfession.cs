using RedCrossChat.Entities;


namespace RedCrossChat.Contracts
{
    public interface IProfession : IRepositoryBase<Profession>
    {
        Task<IEnumerable<Profession>> GetAll();
    }
}
