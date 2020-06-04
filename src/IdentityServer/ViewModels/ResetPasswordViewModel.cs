using System.ComponentModel.DataAnnotations;

namespace IdentityServer.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string UserId { get; set; }

        public string Token { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "The password is less than 6 character")]
        public string Password { get; set; }

        [Required(ErrorMessage = "The Confirm password field is required")]
        [Compare("Password", ErrorMessage = "The Confirm password and Password do not match")]
        public string ConfirmPassword { get; set; }
    }
}
