using System.ComponentModel.DataAnnotations;
using System;

namespace RedCrossChat.ViewModel
{
    public class IntroductionChoiceVm
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Kiswahili")]
        [Required]
        public string Kiswahili { get; set; }

        [Display(Name = "Action Type")]
        [Required]
        public int ActionType { get; set; }


    }
}









