using System.ComponentModel.DataAnnotations;

namespace Project0.Models
{
    public class ResetPasswordViewModel
    {
       
        public string Email { get; set; }
        public string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}