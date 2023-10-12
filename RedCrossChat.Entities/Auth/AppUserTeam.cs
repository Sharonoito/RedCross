using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossChat.Entities
{
    public class AppUserTeam:BaseEntity
    {
        public string AppUser { get; set; }

        public Guid AppUserId { get; set;}

        public string Team { get; set; }

        public Guid TeamId { get; set; }


    }
}




