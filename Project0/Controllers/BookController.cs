using Microsoft.AspNet.Identity;
using Project0.Models;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project0.Controllers
{
    public class BookController : Controller
    {
        private LibraryDbContext context = new LibraryDbContext();

        public ActionResult AddNewBook()
        {
            return View();
        }


        [HttpPost]
        public ActionResult AddNewBook(Book model, HttpPostedFileBase coverImage, HttpPostedFileBase PdfFile, HttpPostedFileBase MobiFile, HttpPostedFileBase Fb2File, HttpPostedFileBase EpubFile)
        {
            if (ModelState.IsValid)
            {
                var existingBook = context.Book.FirstOrDefault(b => b.Title == model.Title);
                if (existingBook != null)
                {
                    ModelState.AddModelError("Title", "A book with this title already exists.");
                }
                // Validate cover image type
                if (coverImage != null && coverImage.ContentLength > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(coverImage.FileName);
                    string extension = Path.GetExtension(coverImage.FileName).ToLower();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".gif")
                    {
                        ModelState.AddModelError("coverImage", "Please upload an image file in jpg, jpeg, png, or gif format.");
                    }
                    else
                    {
                        fileName = $"{fileName}_{DateTime.Now.Ticks}{extension}";
                        string filePath = Path.Combine(Server.MapPath("~/Files/Images"), fileName);
                        coverImage.SaveAs(filePath);
                        model.CoverImage = $"/Files/Images/{fileName}";
                    }
                }

                // Validate PDF file type
                if (PdfFile != null && PdfFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(PdfFile.FileName);
                    string extension = Path.GetExtension(PdfFile.FileName).ToLower();
                    if (extension != ".pdf")
                    {
                        ModelState.AddModelError("PdfFile", "Please upload a file in PDF format.");
                    }
                    else
                    {
                        fileName = $"{fileName}_{DateTime.Now.Ticks}{extension}";
                        string filePath = Path.Combine(Server.MapPath("~/Files/PdfFiles"), fileName);
                        PdfFile.SaveAs(filePath);
                        model.PdfFile = $"/Files/PdfFiles/{fileName}";
                    }
                }

                // Validate MOBI file type
                if (MobiFile != null && MobiFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(MobiFile.FileName);
                    string extension = Path.GetExtension(MobiFile.FileName).ToLower();
                    if (extension != ".mobi")
                    {
                        ModelState.AddModelError("MobiFile", "Please upload a file in Mobi format.");
                    }
                    else
                    {
                        fileName = $"{fileName}_{DateTime.Now.Ticks}{extension}";
                        string filePath = Path.Combine(Server.MapPath("~/Files/MobiFiles"), fileName);
                        MobiFile.SaveAs(filePath);
                        model.MobiFile = $"/Files/MobiFiles/{fileName}";
                    }
                }

                // Validate FB2 file type
                if (Fb2File != null && Fb2File.ContentLength > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(Fb2File.FileName);
                    string extension = Path.GetExtension(Fb2File.FileName).ToLower();
                    if (extension != ".fb2")
                    {
                        ModelState.AddModelError("Fb2File", "Please upload a file in FB2 format.");
                    }
                    else
                    {
                        fileName = $"{fileName}_{DateTime.Now.Ticks}{extension}";
                        string filePath = Path.Combine(Server.MapPath("~/Files/Fb2Files"), fileName);
                        Fb2File.SaveAs(filePath);
                        model.Fb2File = $"/Files/Fb2Files/{fileName}";
                    }
                }

                // Validate EPUB file type
                if (EpubFile != null && EpubFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(EpubFile.FileName);
                    string extension = Path.GetExtension(EpubFile.FileName).ToLower();
                    if (extension != ".epub")
                    {
                        ModelState.AddModelError("EpubFile", "Please upload a file in EPUB format.");
                    }
                    else
                    {
                        fileName = $"{fileName}_{DateTime.Now.Ticks}{extension}";
                        string filePath = Path.Combine(Server.MapPath("~/Files/EpubFiles"), fileName);
                        EpubFile.SaveAs(filePath);
                        model.EpubFile = $"/Files/EpubFiles/{fileName}";
                    }
                }
                if (model.BookType == BookType.OnlyBuying && model.RentalPrice == 0)
                {
                    model.RentalPrice = null;  // هذا يمنع تخزين سعر الإيجار عندما يكون صفرًا
                }

                // إذا كان نوع الكتاب "شراء وإيجار" وكان سعر الإيجار أقل من أو يساوي صفرًا
                if (model.BookType == BookType.BuyAndRent && (model.RentalPrice <= 0))
                {
                    ModelState.AddModelError("RentalPrice", "Rental price must be greater than 0.");
                }
                if (model.Price <= model.RentalPrice)
                {
                    ModelState.AddModelError("Price", "The  price must be greater than the rental price.");
                }

                if (ModelState.IsValid)
                {
                    model.DateAdded = DateTime.Now;  // Date when the book is added
                    context.Book.Add(model);         // Add the book to the database
                    context.SaveChanges();          // Save changes in the database

                    return RedirectToAction("BookList");  // Redirect to the book list page
                }
            }

            // If there are any errors in the ModelState, return the same view with the model to display validation errors
            return View("AddNewBook", model);  // Re-render the form if there are validation errors
        }
        [HttpGet]
        public ActionResult BookList(string category)
        {
            var books = context.Book.AsQueryable();

            // التصفية حسب الفئة
            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                books = books.Where(b => b.Category == category);
            }

            // ترتيب الكتب حسب العنوان
            books = books.OrderBy(b => b.Title);  // الترتيب حسب العنوان

            // تحويل النتيجة إلى قائمة عند العودة إلى العرض
            return View(books.ToList());  // العودة إلى الـ View مع النتيجة النهائية
        }
        public ActionResult ListBook()
        {
            var books = context.Book.ToList(); // جلب جميع الكتب من قاعدة البيانات

            // تأكد من أن الخصائص TotalCopies و AvailableCopies لا تحتوي على قيم null
            foreach (var book in books)
            {
                // تعيين القيم الافتراضية إذا كانت null
                book.TotalCopies = book.TotalCopies ?? 3;  // تعيين 3 كقيمة افتراضية إذا كانت null
                book.AvailableCopies = book.AvailableCopies ?? 3;  // تعيين 3 كقيمة افتراضية إذا كانت null
            }

            return View(books); // عرض الكتب في الـ View
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBook(int id)
        {
            // حذف السجلات المرتبطة في جدول BookRentals
            var bookRentals = context.BookRentals.Where(br => br.BookId == id);
            context.BookRentals.RemoveRange(bookRentals);

            // حذف السجلات المرتبطة في جدول Waitlist
            var waitlist = context.Waitlist.Where(w => w.BookId == id);
            context.Waitlist.RemoveRange(waitlist);

            // حذف السجلات المرتبطة في جداول أخرى إذا كانت موجودة
            // مثل CartItems، BookReviews، أو أي جداول أخرى تعتمد على BookId

            // الآن حذف الكتاب نفسه من جدول Books
            var book = context.Book.Find(id);
            if (book != null)
            {
                context.Book.Remove(book);
                context.SaveChanges();
            }

            // إعادة التوجيه إلى صفحة قائمة الكتب
            return RedirectToAction("BookList");
        }


        public ActionResult EditDiscount(int id)
        {
            var book = context.Book.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }

            return View(book);
        }









        // صفحة عرض الكتب مع البحث والتصفية والفرز
        public ActionResult Index(string search, string category, string sortBy, string ageGroup)
        {
            var books = context.Book.AsQueryable(); // افتراضاً أن db هو السياق الخاص بك للبيانات.

            // تصفية حسب البحث
            if (!string.IsNullOrEmpty(search))
            {
                books = books.Where(b => b.Title.Contains(search) || b.Author.Contains(search));
            }

            // تصفية حسب التصنيف
            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                books = books.Where(b => b.Category == category);
            }

            // تصفية حسب الفئة العمرية
            if (!string.IsNullOrEmpty(ageGroup))
            {
                books = books.Where(b => b.AgeGroup == ageGroup);
            }

            // ترتيب حسب
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "Popularity":
                        books = books.OrderBy(b => b.Popularity);
                        break;
                    case "PriceHighLow":
                        books = books.OrderByDescending(b => b.Price); // ترتيب من الأعلى للأدنى
                        break;
                    case "PriceLowHigh":
                        books = books.OrderBy(b => b.Price); // ترتيب من الأدنى للأعلى
                        break;
                    case "DatePublished":
                        books = books.OrderBy(b => b.DateAdded);
                        break;
                    default:
                        break;
                }
            }

            return View(books.ToList());
        }











        public void BorrowBook(int bookId, string userId)
        {
            // تحقق إذا كان المستخدم مستعيرًا للكتاب مسبقًا
            var existingBorrow = context.BorrowedBooks.FirstOrDefault(b => b.BookId == bookId && b.UserId == userId);
            if (existingBorrow != null)
            {
                throw new Exception("You already borrowed this book");
            }

            // تحقق من الحد الأقصى للاستعارات
            int userBorrowCount = context.BorrowedBooks.Count(b => b.UserId == userId);
            if (userBorrowCount >= 3)
            {
                throw new Exception("You cannot borrow more than 3 books");
            }

            // إنشاء سجل استعارة جديد
            var borrow = new BorrowedBook
            {
                BookId = bookId,
                UserId = userId,
                BorrowedDate = DateTime.Now
            };

            context.BorrowedBooks.Add(borrow);

            // تحديث النسخ المتوفرة
            var book = context.Book.First(b => b.BookId == bookId);
            book.AvailableCopies--;

            context.SaveChanges();
        }
        public void AddToWaitlist(int bookId, string userId)
        {
            var waitlistEntry = new Waitlist
            {
                BookId = bookId,
                Username = userId,
                RequestDate = DateTime.Now
            };

            context.Waitlist.Add(waitlistEntry);
            context.SaveChanges();
        }

        public ActionResult ShowBooks()
        {
            // استرجاع اسم المستخدم الحالي
            var username = User.Identity.Name;

            // استرجاع الكتب المشتراة
            var purchasedBooksQuery = context.OrderItems
                .Where(oi => oi.Order.Username == username && oi.IsRent == false) // الكتب المشتراة (IsRent = false)
                .Select(oi => oi.Book); // اختيار الكائن Book مباشرة من الـ OrderItems

            // استرجاع الكتب المستأجرة
            var rentedBooksQuery = context.OrderItems
                .Where(oi => oi.Order.Username == username && oi.IsRent == true) // الكتب المستأجرة (IsRent = true)
                .Select(oi => oi.Book); // اختيار الكائن Book مباشرة من الـ OrderItems

            // تحويل الاستعلامات إلى قوائم من الكائنات Book
            var purchasedBooks = purchasedBooksQuery.ToList();
            var rentedBooks = rentedBooksQuery.ToList();

            // تمرير الكتب المشتراة والمستأجرة إلى العرض
            ViewBag.PurchasedBooks = purchasedBooks;
            ViewBag.RentedBooks = rentedBooks;

            return View();
        }
        [HttpPost]
        public ActionResult SubmitRating(int BookId, int Rating, string ReviewText)
        {
            var newRating = new Rating
            {
                BookId = BookId,
                RatingValue = Rating,
                ReviewText = ReviewText,
                ReviewDate = DateTime.Now  // تعيين التاريخ الحالي
            };

            context.Ratings.Add(newRating);
            context.SaveChanges();

            return RedirectToAction("ShowBooks", "Book");
        }


        [HttpPost]
        public void AutoReturnBooksFromOrders()
        {
            var overdueOrders = context.Orders
                .Where(o => o.ReturnDate == null && DbFunctions.DiffDays(o.OrderDate, DateTime.Now) >= 30)
                .ToList();

            foreach (var order in overdueOrders)
            {
                // تعيين تاريخ الإرجاع
                order.ReturnDate = DateTime.Now;

                // الحصول على الكتب المرتبطة بالطلب عبر OrderItems
                var orderItems = context.OrderItems.Where(oi => oi.OrderId == order.OrderId).ToList();

                foreach (var orderItem in orderItems)
                {
                    var book = context.Book.FirstOrDefault(b => b.BookId == orderItem.BookId);
                    if (book != null)
                    {
                        // زيادة النسخ المتاحة
                        book.AvailableCopies++;
                    }
                }
            }

            context.SaveChanges();
        }

        public ActionResult BuyOrBorrowBook(int bookId, bool isRent)
        {
            var book = context.Book.FirstOrDefault(b => b.BookId == bookId);
            if (book == null) return HttpNotFound("Book not found");

            if (isRent)
            {
                if (book.AvailableCopies > 0)
                {
                    // إذا كان الكتاب متوفرًا، يتم إضافته كمستعار
                    BorrowBook(bookId, User.Identity.GetUserId());
                }
                else
                {
                    // إذا كان غير متوفر، يتم إدخال المستخدم في قائمة الانتظار
                    AddToWaitlist(bookId, User.Identity.GetUserId());
                }
            }
            else
            {
                // الشراء
                BuyBook(bookId, User.Identity.GetUserId());
            }

            return RedirectToAction("Library");
        }

        private void BuyBook(int bookId, string userId)
        {
            // التحقق من وجود المستخدم
            var user = context.Users.FirstOrDefault(u => u.Username == userId);
            if (user == null)
                throw new Exception("User not found");

            // التحقق من وجود الكتاب
            var book = context.Book.FirstOrDefault(b => b.BookId == bookId);
            if (book == null)
                throw new Exception("Book not found");

            // إضافة الكتاب إلى مكتبة المستخدم
            var purchasedBook = new BorrowedBook
            {
                BookId = bookId,
                UserId = userId,
                BorrowedDate = DateTime.Now,
                IsBorrowed = false // تم الشراء وليس الاستعارة
            };

            context.BorrowedBooks.Add(purchasedBook);
            context.SaveChanges();
        }





        public ActionResult DownloadBook(int bookId)
        {
            var book = context.Book.FirstOrDefault(b => b.BookId == bookId);
            if (book == null) return HttpNotFound();

            var filePath = Path.Combine(Server.MapPath("~/eBooks/"), book.Title + ".pdf");
            if (!System.IO.File.Exists(filePath))
            {
                return HttpNotFound("Book file not found");
            }

            return File(filePath, "application/pdf", book.Title + ".pdf");
        }
        public ActionResult ChangeOption(int bookId, bool isRent)
        {
            var book = context.Book.FirstOrDefault(b => b.BookId == bookId);
            if (book == null) return HttpNotFound();

            if (isRent)
            {
                ViewBag.Option = "Borrow";
            }
            else
            {
                ViewBag.Option = "Buy";
            }

            return View("ConfirmPayment");
        }


        public ActionResult Search(string searchTerm = null, string author = null)
        {
            var books = context.Book.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                books = books.Where(b => b.Title.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(author))
            {
                books = books.Where(b => b.Author.Contains(author));
            }



            return View(books.ToList());
        }
        // صفحة عرض الكتب مع البحث والتصفية والفرز
        public ActionResult SearchAdmin(string search, string category, string sortBy, string ageGroup)
        {
            var books = context.Book.AsQueryable(); // افتراضاً أن db هو السياق الخاص بك للبيانات.

            // تصفية حسب البحث
            if (!string.IsNullOrEmpty(search))
            {
                books = books.Where(b => b.Title.Contains(search) || b.Author.Contains(search));
            }

            // تصفية حسب التصنيف
            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                books = books.Where(b => b.Category == category);
            }

            // تصفية حسب الفئة العمرية
            if (!string.IsNullOrEmpty(ageGroup))
            {
                books = books.Where(b => b.AgeGroup == ageGroup);
            }

            // ترتيب حسب
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "Popularity":
                        books = books.OrderBy(b => b.Popularity);
                        break;
                    case "PriceHighLow":
                        books = books.OrderByDescending(b => b.Price); // ترتيب من الأعلى للأدنى
                        break;
                    case "PriceLowHigh":
                        books = books.OrderBy(b => b.Price); // ترتيب من الأدنى للأعلى
                        break;
                    case "DatePublished":
                        books = books.OrderBy(b => b.DateAdded);
                        break;
                    default:
                        break;
                }
            }

            return View(books.ToList());
        }



        public ActionResult Rating()
        {
            // تحميل قائمة الكتب مع التقييمات الخاصة بكل كتاب
            var booksWithRatings = context.Book
                                           .Include(b => b.Rating)  // تضمين التقييمات المرتبطة بكل كتاب
                                           .ToList();

            return View(booksWithRatings);
        }

    }
}
