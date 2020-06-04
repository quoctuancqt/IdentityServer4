using System.ComponentModel.DataAnnotations;

namespace IdentityServer.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Email invalid")]
        public string Email { get; set; }
    }
}
