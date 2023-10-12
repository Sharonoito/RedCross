using System.ComponentModel.DataAnnotations;
using System;
using RedCrossChat.Entities;

namespace RedCrossChat.ViewModel
{
    public class AppUserTeamVm
    {
        public Guid Id { get; set; }

        [Display(Name = "AppUser")]
        public string Employee { get; set; }

        [Display(Name = "AppUserId")]
        public int UserID { get; set; }

        [Display(Name = "TeamId")]
        public int TeamID { get; set; }

        [Display(Name = "Team")]
        public string Team { get; set; }
    }
}
