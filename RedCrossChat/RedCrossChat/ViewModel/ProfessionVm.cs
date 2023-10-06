using System.ComponentModel.DataAnnotations;
using System;

namespace RedCrossChat.ViewModel
{
    public class ProfessionVm
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Kiswahili")]
        [Required]
        public string Kiswahili { get; set; }


        [Display(Name = "Synonyms")]

        public string Synonyms { get; set; }


      
    }
}







