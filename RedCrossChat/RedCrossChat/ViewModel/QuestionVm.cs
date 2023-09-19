using System.ComponentModel.DataAnnotations;

namespace RedCrossChat
{
    public class QuestionVm
    {
        [Display(Name = "Question")]
        [Required]
        [DataType(DataType.Text)]
        public string Value;

        [Display(Name = "Type")]
        [Required]
        public string Type;
    }
}
