

using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using RedCrossChat.Entities;

namespace RedCrossChat.Repository
{
    public class SubChatRepo : RepositoryBase<SubChat>, ISubChat
    {
        public SubChatRepo(AppDBContext repoContext) : base(repoContext)
        {

        }
    }
}
