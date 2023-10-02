using RedCrossChat.Entities;

namespace RedCrossChat.Contracts
{
    public interface IFeelingRepo: IRepositoryBase<DBFeeling>
    {
     
        Task<IEnumerable<DBFeeling>> GetAll();

        Task<IEnumerable<DBFeeling>> GetAllAsync();
    }
}


