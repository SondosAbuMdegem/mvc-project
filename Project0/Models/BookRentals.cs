using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project0.Models
{
    public class BookRentals
    {
        [Key]
        public int RentalId { get; set; }  // معرّف فريد للاستعارة

        [ForeignKey("Book")]
        public int BookId { get; set; }  // معرّف الكتاب المستعار

        [ForeignKey("User")]
        public string Username { get; set; }  // اسم المستخدم الذي استعار الكتاب

        public DateTime RentalStartDate { get; set; }  // تاريخ بدء الاستعارة

        public DateTime RentalEndDate { get; set; }    // تاريخ انتهاء الاستعارة (بعد 30 يومًا)

        public DateTime? ReturnDate { get; set; }  // تاريخ إرجاع الكتاب (اختياري)

        public bool IsReturned { get; set; }  // حالة الكتاب (هل تم إرجاعه أم لا)

        public virtual Book Book { get; set; }  // ربط الكتاب في الكائن Book

        public virtual User User { get; set; }  // ربط المستخدم في الكائن User
    }
}