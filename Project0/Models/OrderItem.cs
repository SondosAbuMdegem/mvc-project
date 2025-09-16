namespace Project0.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; } // المفتاح الأساسي
        public int OrderId { get; set; }     // معرّف الطلب (Foreign Key)
        public int BookId { get; set; }      // معرّف الكتاب
        public int Quantity { get; set; }    // الكمية
        public decimal Price { get; set; }   // السعر
        public bool IsRent { get; set; }     // هل العنصر مستأجر؟

        // العلاقات
        public virtual Order Order { get; set; } // العلاقة مع Order
        public virtual Book Book { get; set; } // العلاقة مع Book   // العلاقة مع جدول Orders
    }
}