

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RedCrossChat.Entities;
using RedCrossChat.Entities.Auth;

namespace RedCrossChat.Domain
{
    public class DbInitializer
    {

        private readonly ModelBuilder modelBuilder;

        public DbInitializer(ModelBuilder modelBuilder)
        {
            this.modelBuilder = modelBuilder;
        }

        public  void Seed()
        {

            modelBuilder.Entity<DBFeeling>().HasData(
                new DBFeeling { Name="Happy",Description= "Happy 😀", Synonymns= "Happy,happy",Id=Guid.NewGuid(),DateCreated=DateTime.Now },
                new DBFeeling { Name= "Angry", Description= "Angry 😡", Synonymns="", Id = Guid.NewGuid(), DateCreated = DateTime.Now },
                new DBFeeling { Name= "Anxious", Description= "Anxious \U0001f974", Synonymns="", Id = Guid.NewGuid(), DateCreated = DateTime.Now },
                new DBFeeling { Name= "Sad", Description= "Sad 😪", Synonymns="", Id = Guid.NewGuid(), DateCreated = DateTime.Now },
                new DBFeeling { Name= "Flat Effect", Description= "Flat Effect \U0001fae5", Synonymns="", Id = Guid.NewGuid(), DateCreated = DateTime.Now },
                new DBFeeling { Name= "Expressionless ", Description= "Expressionless \U0001fae4", Synonymns="", Id = Guid.NewGuid(), DateCreated = DateTime.Now }
            );

            modelBuilder.Entity<AppRole>().HasData(
                new AppRole(Constants.SuperAdministratorId, "Super Administrator", true, DateTime.Now),
                new AppRole(Constants.AdministratorId, "Administrator", true, DateTime.Now)
           );

            
         
        }
    }
}
