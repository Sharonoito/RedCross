using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using RedCrossChat.Entities;

namespace RedCrossChat.ViewModel
{
    public class TeamVm
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Notification Type")]
        public int NotificationType { get; set; }

        public List<AppUser> appUsers { get; set; }

    }
}
