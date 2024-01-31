

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
                new AppRole(Constants.SuperAdministratorId, Constants.SuperAdministrator, true, DateTime.Now),
                new AppRole(Constants.AdministratorId, Constants.Administrator, true, DateTime.Now),
                new AppRole(Guid.NewGuid().ToString(),Constants.PssAgent,true, DateTime.Now),
                new AppRole(Guid.NewGuid().ToString(),Constants.PssManager,true, DateTime.Now),
                new AppRole(Guid.NewGuid().ToString(),Constants.AuditRole,true, DateTime.Now)  
           );
        }
    }
}
