using System;

namespace RedCrossChat.ViewModel
{
    public class ResetViewModel
    {

        public string Email { get; set; } = "";

        public string Password { get; set; } = "";

        public string OTP { get; set; } = "";

        public string Token { get; set; } = "";
        
        public Guid UserID {  get; set; } = Guid.NewGuid();

    }
}
