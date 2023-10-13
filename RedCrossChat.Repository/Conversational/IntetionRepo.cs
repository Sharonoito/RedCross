using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;


namespace RedCrossChat.Repository
{
    public class IntetionRepo : RepositoryBase<Intention>, IItention
    {
        public IntetionRepo(AppDBContext repoContext) : base(repoContext)
        {
        }
    }
}
