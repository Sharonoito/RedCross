using System;
using System.ComponentModel.DataAnnotations;

namespace RedCrossChat.ViewModel
{
    public class ExerciseVm
    {
        public Guid Id { get; set; }

        [Display(Name = "Feeling")]
        public string Feeling { get; set; } = "";

        [Display(Name = "Exercises")]
        public string Exercises { get; set; }

        [Display(Name = "Swahili")]
        public string Kiswahili { get; set; }

    }
}
