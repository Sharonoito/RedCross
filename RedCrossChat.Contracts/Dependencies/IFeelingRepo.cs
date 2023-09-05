using RedCrossChat.Entities;

namespace RedCrossChat.Contracts.Dependencies
{
    public interface IFeelingRepo: IRepositoryBase<DBFeeling>
    {
        Task<IEnumerable<DBFeeling>> GetAll();


    }
}
