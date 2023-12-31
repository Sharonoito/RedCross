﻿using System.ComponentModel.DataAnnotations;

namespace RedCrossChat
{
    public class LoginVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

    }

    public interface SampleVm
    {
        public string Name { get; set; }


    }
}
