
using Microsoft.AspNetCore.Identity;


namespace RedCrossChat.Entities.Auth
{
    public class AppRole : IdentityRole
    {
        public AppRole() : base() { }

        public AppRole(string roleName) : base(roleName) { }

        public AppRole(string id, string roleName, bool isDefault, DateTime dateCreated) : base(roleName)
        {
            Id = id;
            IsDefault = isDefault;
            
            DateCreated = dateCreated;
        }

        public bool IsDefault { get; set; }
      
        public DateTime DateCreated { get; set; }


    }
}
