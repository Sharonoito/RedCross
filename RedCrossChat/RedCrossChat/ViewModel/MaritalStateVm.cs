using System.ComponentModel.DataAnnotations;
using System;

namespace RedCrossChat.ViewModel
{
    public class MaritalStateVm
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Kiswahili")]
        public string Kiswahili { get; set; }

        [Display(Name = "Synonyms")]
        public string Synonyms { get; set; }
    }
}
