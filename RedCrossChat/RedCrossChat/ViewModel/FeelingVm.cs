using System;
using System.ComponentModel.DataAnnotations;

namespace RedCrossChat.ViewModel
{
    public class FeelingVm
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Synonyms")]
        public string Synonyms { get; set; }

    }
}
