
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace RedCrossChat.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        [NotMapped]
        public List<string> Roles { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
                    return FirstName + " " + LastName;
                return "";
            }
        }

        public bool IsDeactivated { get; set; }
    }
}
