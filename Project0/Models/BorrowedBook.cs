using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project0.Models
{
    public class BorrowedBook
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public string UserId { get; set; }

        public DateTime BorrowedDate { get; set; }
        public bool IsBorrowed { get; set; } // جديد: لتحديد ما إذا كان الكتاب مستعارًا أم مشتريًا
        public DateTime? ReturnDate { get; set; }

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}