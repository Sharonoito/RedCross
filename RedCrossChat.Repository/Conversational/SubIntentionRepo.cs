using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;


namespace RedCrossChat.Repository
{
    public class SubIntentionRepo : RepositoryBase<SubIntention>, ISubIntention
    {
        public SubIntentionRepo(AppDBContext repoContext) : base(repoContext)
        {

        }
    }
}
