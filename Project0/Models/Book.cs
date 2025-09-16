using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Project0.Models
{


    public enum BookType
    {
        OnlyBuying = 0,  // شراء فقط
        BuyAndRent = 1   // شراء واستعارة
    }
    public class Book
    {
        public int BookId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(100)]
        public string Author { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a valid price.")]
        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; }

        public string CoverImage { get; set; }
        public DateTime DateAdded { get; set; }
        public string Category { get; set; }
        public string AgeGroup { get; set; }
        public int? Popularity { get; set; }

        public decimal? RentalPrice { get; set; }


        public string PdfFile { get; set; }
        public string MobiFile { get; set; }
        public string Fb2File { get; set; }
        public string EpubFile { get; set; }
        public BookType BookType { get; set; }  // إضافة النوع
        public decimal? DiscountPercentage { get; set; } // نسبة الخصم
        public DateTime? DiscountStartDate { get; set; } // تاريخ بداية الخصم
        public DateTime? DiscountEndDate { get; set; } // تاريخ نهاية الخصم
        public decimal? DiscountedPrice { get; set; } // السعر بعد الخصم

        public int? TotalCopies { get; set; } = 3;  // اجعل العدد 3 كعدد ثابت
        public int? AvailableCopies { get; set; } = 3; // نسخ متاحة للاستعارة

        public virtual ICollection<Rating> Rating { get; set; }

    }
}