namespace Project0.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; } // المعرف الفريد
        public int BookId { get; set; } // معرف الكتاب
        public int Quantity { get; set; } // عدد النسخ
        public bool IsRent { get; set; } // هل هو استعارة أو شراء
        public string Username { get; set; } // اسم المستخدم (مفتاح خارجي)
                                             // إضافة المفتاح الأجنبي لربط CartItem بـ Purchase


        public virtual Book Book { get; set; }
        public virtual User User { get; set; }

        public decimal TotalPrice
        {
            get
            {
                if (Book == null)
                {
                    return 0; // أو قيمة افتراضية
                }

                decimal price = IsRent && Book.RentalPrice.HasValue ? Book.RentalPrice.Value : Book.Price;
                return Quantity * price;
            }
        }
    }
}