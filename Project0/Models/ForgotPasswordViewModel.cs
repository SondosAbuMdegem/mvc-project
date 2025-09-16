using System.ComponentModel.DataAnnotations;

namespace Project0.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}