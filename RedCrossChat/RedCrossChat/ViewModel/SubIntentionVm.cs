using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using RedCrossChat.Entities;

namespace RedCrossChat.ViewModel
{
    public class SubIntentionVm
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Kiswahili")]
        [Required]
        public string Kiswahili { get; set; }

        public Guid ItentionId { get; set; }

        public List<Intention> Intentions { get; set; }

    }
}














