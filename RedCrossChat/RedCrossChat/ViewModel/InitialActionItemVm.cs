using System.ComponentModel.DataAnnotations;
using System;
using RedCrossChat.Entities;
using System.Collections.Generic;

namespace RedCrossChat.ViewModel
{
    public class InitialActionItemVm
    {
        public Guid Id { get; set; }

        [Display(Name = "Choices")]
        [Required]
        public string Choices { get; set; }

        [Display(Name = "ActionMessage")]
        [Required]
        public string ActionMessage { get; set; }

        [Display(Name = "Value")]
        [Required]
        public string Value { get; set; }

        [Display(Name = "SubTitle")]
        [Required]
        public string SubTitle { get; set; }

        [Display(Name = "Language")]
        [Required]
        public int Language { get; set; }
        public Guid IntroductionChoiceId { get; set; }
        public List<IntroductionChoice>IntroductionChoices { get; set; }


    }
}

