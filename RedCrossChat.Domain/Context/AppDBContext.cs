using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RedCrossChat.Entities;
using RedCrossChat.Entities.Auth;

namespace RedCrossChat.Domain
{
    public class AppDBContext : IdentityDbContext<AppUser, AppRole, string>
    {
       
        public DbSet<AppClaim> AppClaims { get; set; }

        public DbSet<AppModule> AppModules { get; set; }

        public DbSet<Persona> Persona { get; set; }

        public DbSet<Gender> Gender { get; set; }

        public DbSet<AgeBand> AgeBand { get; set; }

        public DbSet<MaritalState> MaritalState { get; set; }

        public DbSet<Profession> Profession { get; set; }   

        public DbSet<Question> Question { get; set; }

        public DbSet<QuestionOption> QuestionOption { get; set; }

        public DbSet<AiConversation> AiConversation { get; set; }

        public DbSet<Conversation> Conversation { get; set; }

        public DbSet<HandOverRequest> HandOverRequest { get; set; }

        public DbSet<DBFeeling> Feeling { get; set; }

        public DbSet<ChatMessage> ChatMessage { get; set; }

        public DbSet<DBCounty> County { get; set; }

        public DbSet<RawConversation> RawConversation { get; set; }

        public DbSet<Team> Team { get; set; }

        public DbSet<Intention> Intention { get; set; }

        public DbSet<SubIntention> SubIntention { get; set; }

        public DbSet<AppUserTeam> AppUserTeam { get; set; }

        public DbSet<Exercise> Exercise { get; set; }

        public DbSet<IntroductionChoice> IntroductionChoice { get; set; }

        public DbSet<InitialActionItem> InitialActionItem { get; set; }

        public DbSet<AIRejectedQuestion> AIRejectedQuestion { get; set; }

        public DbSet<AuthLoginLog> AuthLoginLog { get; set; }

        public AppDBContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            new DbInitializer(modelBuilder).Seed();

            modelBuilder.Entity<Intention>()
                .HasMany(i => i.SubIntentions)
                .WithOne(si => si.Intention)
                .HasForeignKey(si => si.IntentionId);


        }
    }
}
