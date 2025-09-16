using Project0.Models;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;

namespace Project0.Controllers
{
    public class AdminController : Controller
    {
        private LibraryDbContext context = new LibraryDbContext();

        public ActionResult Index()
        {
            var books = context.Book.ToList();
            return View(books);
        }

        [HttpGet]
        public ActionResult EditPrice(int id)
        {
            var book = context.Book.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPrice(int id, decimal? discountPercentage, DateTime? discountStartDate, DateTime? discountEndDate)
        {
            var book = context.Book.FirstOrDefault(b => b.BookId == id);
            if (book == null)
            {
                return HttpNotFound();
            }

            // تحقق من أن نسبة الخصم صحيحة
            if (discountPercentage.HasValue && discountPercentage.Value > 0 && discountPercentage.Value <= 100)
            {
                // تحقق من أن تاريخ البداية قبل تاريخ النهاية
                if (discountStartDate.HasValue && discountEndDate.HasValue)
                {
                    if (discountEndDate.Value > discountStartDate.Value.AddDays(7))
                    {
                        // إذا كانت مدة الخصم تتجاوز 7 أيام
                        ModelState.AddModelError("", "مدة الخصم لا يمكن أن تتجاوز 7 أيام.");
                        return View(book);
                    }

                    // حساب السعر المخفض بناءً على النسبة المدخلة
                    decimal discountAmount = book.Price * (discountPercentage.Value / 100);
                    book.DiscountedPrice = book.Price - discountAmount; // السعر بعد الخصم
                    book.DiscountStartDate = discountStartDate;
                    book.DiscountEndDate = discountEndDate;
                }
                else
                {
                    // إذا كانت تواريخ الخصم غير صالحة
                    ModelState.AddModelError("", "تاريخ نهاية الخصم يجب أن يكون بعد تاريخ البدء.");
                    return View(book);
                }
            }
            else
            {
                // إذا لم يتم إدخال نسبة خصم صالحة
                book.DiscountedPrice = null;
                book.DiscountStartDate = null;
                book.DiscountEndDate = null;
            }

            try
            {
                // حاول حفظ التغييرات
                context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                // إذا كانت هناك أخطاء في الكيانات المدخلة، قم بعرضها للمستخدم
                foreach (var validationErrors in e.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return View(book);
            }
            return RedirectToAction("BookList", "Book");  // إعادة التوجيه إلى الصفحة الرئيسية بعد النجاح
        }
        /* public ActionResult Waitlist()
         {
             var waitlistItems = context.Waitlist.Include(w => w.Book).Include(w => w.User).ToList();
             return View(waitlistItems);
         }
         public ActionResult TestWaitlist()
         {
             var waitlistItems = context.Waitlist
                                        .Include(w => w.Book)  // تأكد أن `Book` مرتبط بشكل صحيح
                                        .Include(w => w.User)  // تأكد أن `User` مرتبط بشكل صحيح
                                        .ToList();

             foreach (var item in waitlistItems)
             {
                 Console.WriteLine($"Waitlist Item: {item.WaitlistId}, Book: {item.Book?.Title}, User: {item.User?.Username}");
             }

             return Content("تم تحميل قائمة الانتظار بنجاح. تحقق من وحدة التحكم لمعرفة التفاصيل.");
         }*/
        public ActionResult WaitingList()
        {
            var waitlist = context.Waitlist
                .Select(w => new
                {
                    w.WaitlistId,
                    w.BookId,
                    BookTitle = w.Book.Title,
                    w.Username,
                    w.RequestDate
                })
                .ToList();

            return View(waitlist);
        }
        public ActionResult AdminPanel()
        {
            var users = context.Users.ToList();
            return View(users); // تمرير قائمة المستخدمين إلى الـ View
        }

    }
}