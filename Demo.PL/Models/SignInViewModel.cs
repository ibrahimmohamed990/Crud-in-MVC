using System.ComponentModel.DataAnnotations;

namespace Demo.PL.Models
{
    public class SignInViewModel
    {
        [EmailAddress(ErrorMessage = "Invalid Email Format!!")]
        [Required]
        public string Email { get; set; }
        [Required]
        [MaxLength(6)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }


    }
}
