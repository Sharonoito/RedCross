
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
