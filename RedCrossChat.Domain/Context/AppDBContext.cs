using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RedCrossChat.Entities;
using RedCrossChat.Entities.Auth;

namespace RedCrossChat.Domain
{
    public class AppDBContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public AppDBContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<AppClaim> AppClaims { get; set; }

        public DbSet<AppModule> AppModules { get; set; }

        public DbSet<Persona> Persona { get; set; }

        public DbSet<AiConversation> AiConversation { get; set; }

        public DbSet<Conversation> Conversation { get; set; }

        public DbSet<DBFeeling> Feeling { get; set; }

        public DbSet<DBCounty> County { get; set; }
    }
}
