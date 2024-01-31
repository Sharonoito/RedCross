using RedCrossChat.Entities;


namespace RedCrossChat.Contracts
{
    public interface IExercise : IRepositoryBase<Exercise>
    {
        Task<IEnumerable<Exercise>> GetAll();

        Task<IEnumerable<Exercise>> GetAllAsync();
    }
}






