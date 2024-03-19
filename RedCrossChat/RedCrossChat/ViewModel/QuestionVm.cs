using System;
using System.ComponentModel.DataAnnotations;

namespace RedCrossChat
{
    public class QuestionVm
    {
        public Guid Id { get; set; }

        [Display(Name = "Question")]
        [Required]
        [DataType(DataType.Text)]
        public string Value;

        [Display(Name = "Type")]
        [Required]
        public string Type;

        [Display(Name = "Question")]
        [Required]
        public string Question { get; set; }

        [Display(Name = "Kiswahili")]
        [Required]
        public string Kiswahili { get; set; }

        [Display(Name = "Code")]
        [Required]
        public int Code { get; set; }

    }
}



