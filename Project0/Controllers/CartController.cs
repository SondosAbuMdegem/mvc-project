using Project0.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;


namespace Project0.Controllers
{
    public class CartController : Controller
    {
        private LibraryDbContext context = new LibraryDbContext();


        public ActionResult AddToCart(int bookId, bool? isRent)
        {
            var username = User.Identity.Name; // الحصول على اسم المستخدم الحالي
            var user = context.Users.SingleOrDefault(u => u.Username == username);

            if (user == null)
            {
                // التعامل مع حالة المستخدم غير الموجود في قاعدة البيانات
                return RedirectToAction("Login", "Account");
            }

            // البحث عن الكتاب
            var book = context.Book.SingleOrDefault(b => b.BookId == bookId);

            if (book == null)
            {
                // إذا لم يتم العثور على الكتاب
                TempData["Error"] = "Book not found.";
                return RedirectToAction("Cart", "Cart");
            }

            // إذا كانت النسخ غير متاحة وكان الكتاب للاستعارة
            if (isRent.HasValue && isRent.Value)
            {
                if (book.AvailableCopies <= 0)
                {
                    // إذا كانت جميع النسخ المستعارة، إضافة المستخدم إلى قائمة الانتظار
                    var waitingListEntry = new Waitlist
                    {
                        BookId = bookId,
                        Username = user.Username,
                        RequestDate = DateTime.Now
                    };
                    context.Waitlist.Add(waitingListEntry);
                    context.SaveChanges();

                    TempData["Message"] = "All copies are currently rented. You have been added to the waiting list.";
                    return RedirectToAction("Cart", "Cart");
                }

                var totalBooksRented = context.CartItems.Count(ci => ci.Username == user.Username && ci.IsRent == true) +
                       context.BookRentals.Count(br => br.Username == user.Username);

                if (totalBooksRented >= 3)
                {
                    TempData["Error"] = "You cannot rent more than 3 books at the same time.";
                    return RedirectToAction("Cart", "Cart");
                }


                // إذا كانت هناك نسخ متاحة للاستعارة
                var rentalStartDate = DateTime.Now;
                var rentalEndDate = rentalStartDate.AddDays(30); // إضافة 30 يومًا للاسترجاع

                var bookRental = new BookRentals
                {
                    BookId = bookId,
                    Username = user.Username,
                    RentalStartDate = rentalStartDate,
                    RentalEndDate = rentalEndDate
                };

                context.BookRentals.Add(bookRental);
                book.AvailableCopies -= 1; // تقليل عدد النسخ المتاحة للاستعارة
            }
            else
            {
                // إذا كان الكتاب للشراء (وليس للاستعارة)
                if (book.AvailableCopies <= 0 && !book.RentalPrice.HasValue)
                {
                    // إذا كانت الكمية صفرًا أو أقل وكان الكتاب غير قابل للاستعارة
                    TempData["Error"] = "This book is out of stock.";
                    return RedirectToAction("Cart", "Cart");
                }

                var existingCartItem = context.CartItems
                    .FirstOrDefault(c => c.BookId == bookId && c.Username == user.Username && c.IsRent == false); // التحقق من وجود الكتاب في السلة للشراء

                if (existingCartItem != null)
                {
                    // إذا كان العنصر موجودًا، قم بزيادة الكمية
                    existingCartItem.Quantity += 1;
                }
                else
                {
                    // إذا لم يكن العنصر موجودًا، أضفه كعنصر جديد
                    var cartItem = new CartItem
                    {
                        BookId = bookId,
                        Username = user.Username,
                        IsRent = false, // تعيين قيمة isRent على false لأن الكتاب للشراء
                        Quantity = 1 // تعيين الكمية الابتدائية
                    };

                    context.CartItems.Add(cartItem);
                }
            }

            // إذا كان الكتاب للاستعارة (isRent = true)
            if (isRent.HasValue && isRent.Value)
            {
                var existingRentItem = context.CartItems
                    .FirstOrDefault(c => c.BookId == bookId && c.Username == user.Username && c.IsRent == true); // التحقق من وجود الكتاب في السلة للاستعارة

                if (existingRentItem != null)
                {
                    // إذا كان العنصر موجودًا، قم بزيادة الكمية
                    existingRentItem.Quantity += 1;
                }
                else
                {
                    // إذا لم يكن العنصر موجودًا، أضفه كعنصر جديد للاستعارة
                    var rentCartItem = new CartItem
                    {
                        BookId = bookId,
                        Username = user.Username,
                        IsRent = true, // تعيين قيمة isRent على true لأن الكتاب للاستعارة
                        Quantity = 1 // تعيين الكمية الابتدائية
                    };

                    context.CartItems.Add(rentCartItem);
                }
            }

            context.SaveChanges();

            return RedirectToAction("Cart", "Cart");
        }

        [HttpPost]
        public ActionResult RemoveFromCart(int cartItemId)
        {
            var cartItem = context.CartItems.FirstOrDefault(c => c.CartItemId == cartItemId);

            if (cartItem != null)
            {
                var book = context.Book.SingleOrDefault(b => b.BookId == cartItem.BookId);

                if (book != null && cartItem.IsRent)
                {
                    // حذف الكتاب من السلة
                    context.CartItems.Remove(cartItem);
                    context.SaveChanges();

                    // التحقق من عدد الكتب المستعارة في نفس الفترة الزمنية
                    var username = User.Identity.Name;
                    var rentedBooksCount = context.BookRentals.Count(br => br.Username == username && br.RentalEndDate > DateTime.Now);

                    // التحقق من عدد الكتب في سلة المشتريات
                    var booksInCartCount = context.CartItems.Count(ci => ci.Username == username && ci.IsRent == true);

                    // التحقق من المساحة المتاحة للاستعارة
                    if (rentedBooksCount + booksInCartCount < 3)
                    {
                        TempData["Message"] = "You can now add more books to rent.";
                    }
                    else
                    {
                        TempData["Error"] = "You cannot rent more than 3 books at the same time.";
                    }
                }
                else
                {
                    // حذف الكتاب للشراء
                    context.CartItems.Remove(cartItem);
                    context.SaveChanges();
                }
            }

            return RedirectToAction("Cart", "Cart");
        }
        // عرض سلة التسوق
        public ActionResult Cart()
        {
            var username = User.Identity.Name;

            var cartItems = context.CartItems
                .Include(c => c.Book)
                .Where(c => c.Username == User.Identity.Name)
                .ToList();

            return View(cartItems);
        }

        /* public ActionResult ReturnBook(int bookId)
         {
             var book = context.Book.FirstOrDefault(b => b.BookId == bookId);

             if (book == null)
             {
                 return HttpNotFound("Book not found.");
             }

             // إذا كان هناك مستخدم في قائمة الانتظار
             var waitlistItem = context.Waitlist
                 .Where(w => w.BookId == bookId)
                 .OrderBy(w => w.RequestDate) // ترتيب حسب تاريخ الطلب
                 .FirstOrDefault();

             if (waitlistItem != null)
             {
                 // إرسال إشعار للمستخدم
                 var user = context.Users.FirstOrDefault(u => u.Username == waitlistItem.Username);
                 if (user != null)
                 {
                     // هنا يمكنك إرسال بريد إلكتروني أو إشعار
                     Console.WriteLine($"Notification sent to {user.Username} about the availability of the book.");
                 }

                 // إزالة المستخدم من قائمة الانتظار
                 context.Waitlist.Remove(waitlistItem);
             }

             // زيادة النسخ المتاحة
             book.AvailableCopies = (book.AvailableCopies ?? 0) + 1;

             context.SaveChanges();

             return RedirectToAction("Cart");
         }*/

        /*
        public ActionResult AddToWaitlist(int bookId, string username)
        {
            var book = context.Book.FirstOrDefault(b => b.BookId == bookId);
            var user = context.Users.FirstOrDefault(u => u.Username == username);

            if (book == null || user == null)
            {
                return HttpNotFound("Book or user not found.");
            }

            // تحقق من أن الكتاب يمكن استعارته وليس "للشراء فقط"
            if (book.BookType == BookType.OnlyBuying)
            {
                return Content("This book is only available for purchase.");
            }

            // تحقق إذا كان المستخدم موجودًا بالفعل في قائمة الانتظار
            var existingWaitlistItem = context.Waitlist
                .FirstOrDefault(w => w.BookId == bookId && w.Username == username);

            if (existingWaitlistItem != null)
            {
                return Content("You are already in the waiting list for this book.");
            }

            // إضافة المستخدم إلى قائمة الانتظار
            var waitlistItem = new Waitlist
            {
                BookId = book.BookId,
                Username = username,
                RequestDate = DateTime.Now
            };

            context.Waitlist.Add(waitlistItem);
            context.SaveChanges();

            return RedirectToAction("Cart");
        }
        */
        [HttpPost]
        public ActionResult AddToWaitlist(int bookId)
        {
            // الحصول على اسم المستخدم الحالي (يمكنك تعديله بناءً على مصادقة المستخدم)
            var username = User.Identity.Name;

            // التحقق إذا كان الكتاب موجودًا
            var book = context.Book.FirstOrDefault(b => b.BookId == bookId);


            // التحقق إذا كان المستخدم موجودًا في قائمة الانتظار لنفس الكتاب
            var existingEntry = context.Waitlist.FirstOrDefault(w => w.BookId == bookId && w.Username == username);
            if (existingEntry != null)
            {
                TempData["Message"] = "You are already in the waiting list for this book.";
                return RedirectToAction("ListBook", "Book");
            }

            // إنشاء سجل جديد في قائمة الانتظار
            var waitlistEntry = new Waitlist
            {
                BookId = bookId,
                Username = username,
                RequestDate = DateTime.Now
            };

            context.Waitlist.Add(waitlistEntry);
            context.SaveChanges();

            TempData["Message"] = "You have been added to the waiting list for this book.";
            return RedirectToAction("ListBook", "Book");
        }

    }
}