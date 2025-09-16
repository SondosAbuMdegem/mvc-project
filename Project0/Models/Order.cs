using System;
using System.Collections.Generic;

namespace Project0.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string Username { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public DateTime? ReturnDate { get; set; }
        public virtual User User { get; set; } // العلاقة مع User
        public virtual ICollection<OrderItem> OrderItems { get; set; }


    }
}