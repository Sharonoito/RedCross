using Microsoft.EntityFrameworkCore;
using RedCrossChat.Entities;

namespace RedCrossChat.Domain
{
    public class AppDBContext: DbContext
    {
        public AppDBContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Persona> Persona { get; set; }

        public DbSet<AiConversation> AiConversation { get; set; }

        public DbSet<Conversation> Conversation { get; set; }

        public DbSet<DBFeeling> Feeling { get; set; }

        public DbSet<DBCounty> County { get; set; }
    }
}
