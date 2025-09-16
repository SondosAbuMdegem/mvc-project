using System;


namespace Project0.Models
{
    public class Waitlist
    {
        public int WaitlistId { get; set; }
        public int BookId { get; set; }
        public string Username { get; set; }  // تأكد من أن Username هو مفتاح للمستخدمين
        public DateTime RequestDate { get; set; }

        // الربط مع جدول الكتب
        public virtual Book Book { get; set; }

        // الربط مع جدول المستخدمين عبر Username
        public virtual User User { get; set; }
    }
}