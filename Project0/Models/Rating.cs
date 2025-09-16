using System;

namespace Project0.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int RatingValue { get; set; }
        public string ReviewText { get; set; }
        public DateTime ReviewDate { get; set; }

        // العلاقات مع الجداول الأخرى
        public virtual Book Book { get; set; } // علاقة مع كتاب

    }

}