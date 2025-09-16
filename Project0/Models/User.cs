using System;
using System.ComponentModel.DataAnnotations;

namespace Project0.Models
{
    public class User
    {
        [Key]
        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }

        // New fields for credit card information
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "ID must be exactly 9 digits.")]
        public string ID { get; set; }

        [Required]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Credit Card Number must be exactly 16 digits.")]
        public string CreditCardNumber { get; set; }

        [Required]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Valid Date must be in MM/YY format.")]
        public string ValidDate { get; set; }

        [Required]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "CVC must be exactly 3 digits.")]
        public string CVC { get; set; }
        public string ResetPasswordToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }

    }
}
