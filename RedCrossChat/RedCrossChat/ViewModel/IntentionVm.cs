using System.ComponentModel.DataAnnotations;
using System;

namespace RedCrossChat.ViewModel
{
    public class IntentionVm
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Kiswahili")]
        [Required]
        public string Kiswahili { get; set; }

        [Display(Name = "SubIntention")]
        public string SubIntention { get; set; }

    }
}









