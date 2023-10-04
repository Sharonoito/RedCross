

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
            modelBuilder.Entity<AppRole>().HasData(
                new AppRole(Constants.SuperAdministratorId, "Super Administrator", true, DateTime.Now),
                new AppRole(Constants.AdministratorId, "Administrator", true, DateTime.Now)
           );
        }
    }
}
