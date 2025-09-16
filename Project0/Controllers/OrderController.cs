using Project0.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace Project0.Controllers
{
    public class OrderController : Controller
    {

        private LibraryDbContext db = new LibraryDbContext();
        public bool IsValidCreditCard(PaymentViewModelcs payment)
        {
            // تحقق أن رقم البطاقة يتكون من 16 رقمًا
            if (string.IsNullOrEmpty(payment.CardNumber) || payment.CardNumber.Length != 16 || !payment.CardNumber.All(char.IsDigit))
            {
                return false;
            }

            // تحقق أن شهر الانتهاء بين 1 و 12
            if (payment.ExpirationMonth < 1 || payment.ExpirationMonth > 12)
            {
                return false;
            }

            // تحقق أن السنة أكبر من أو تساوي السنة الحالية
            int currentYear = DateTime.Now.Year;
            if (payment.ExpirationYear < currentYear)
            {
                return false;
            }

            // إذا كانت السنة هي السنة الحالية، تحقق أن الشهر أكبر من أو يساوي الشهر الحالي
            int currentMonth = DateTime.Now.Month;
            if (payment.ExpirationYear == currentYear && payment.ExpirationMonth < currentMonth)
            {
                return false;
            }

            // تحقق أن الـ CVC يتكون من 3 أرقام
            if (string.IsNullOrEmpty(payment.CVC) || payment.CVC.Length != 3 || !payment.CVC.All(char.IsDigit))
            {
                return false;
            }

            // إذا اجتازت كل الاختبارات
            return true;
        }
        [HttpPost]
        public ActionResult ProcessPayment(PaymentViewModelcs model)
        {
            if (ModelState.IsValid)
            {
                if (IsValidCreditCard(model))
                {
                    string userName = User.Identity.Name;
                    var cartItems = db.CartItems.Where(c => c.Username == userName).ToList();

                    if (cartItems.Any())
                    {
                        var totalAmount = cartItems.Sum(c => c.Quantity * (c.IsRent ? c.Book.RentalPrice : c.Book.Price));

                        var order = new Order
                        {
                            Username = userName,
                            OrderDate = DateTime.Now,
                            TotalAmount = totalAmount
                        };

                        db.Orders.Add(order);
                        db.SaveChanges();

                        foreach (var item in cartItems)
                        {
                            var orderItem = new OrderItem
                            {
                                OrderId = order.OrderId,
                                BookId = item.BookId,
                                Quantity = item.Quantity,
                                Price = item.TotalPrice,
                                IsRent = item.IsRent
                            };

                            db.OrderItems.Add(orderItem);
                            if (item.IsRent)
                            {
                                var book = db.Book.FirstOrDefault(b => b.BookId == item.BookId);
                                if (book != null)
                                {
                                    if (book.AvailableCopies >= item.Quantity)
                                    {
                                        book.AvailableCopies -= item.Quantity;
                                    }

                                }
                            }
                        }

                        db.CartItems.RemoveRange(cartItems);
                        db.SaveChanges();
                        // إرسال بريد إلكتروني بعد إضافة الطلب
                        string userEmail = db.Users.Where(u => u.Username == userName).FirstOrDefault()?.Email;
                        if (!string.IsNullOrEmpty(userEmail))
                        {
                            SendEmail(userEmail, "Order Confirmation", "Your order has been successfully placed.");
                        }

                        // إضافة رسالة النجاح
                        TempData["SuccessMessage"] = "تمت عملية الدفع بنجاح وتم إضافة طلبك!";
                        return RedirectToAction("HomeC", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "لا توجد عناصر في السلة لإتمام العملية.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "معلومات بطاقة الائتمان غير صحيحة. يرجى التحقق من التفاصيل.");
                }
            }

            return View(model);
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                // إعداد البريد
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("sondos.nice.girl1@gmail.com"); // بريد المرسل
                mail.To.Add(toEmail); // بريد المستلم
                mail.Subject = subject; // العنوان
                mail.Body = body; // محتوى الرسالة
                mail.IsBodyHtml = true; // إذا كانت الرسالة بصيغة HTML

                // إعداد خادم SMTP
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.Credentials = new NetworkCredential("sondos.nice.girl1@gmail.com", "cpduhhbmriwlnrvo");
                smtpClient.EnableSsl = true;

                // إرسال البريد
                smtpClient.Send(mail);
                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
            }
        }
        public void NotifyNextUser(int bookId)
        {
            var nextUserInWaitlist = db.Waitlist
                .Where(w => w.BookId == bookId)
                .OrderBy(w => w.RequestDate)
                .FirstOrDefault();

            if (nextUserInWaitlist != null)
            {
                SendEmail(nextUserInWaitlist.User.Email, "Book Available", "The book is now available for borrowing.");
            }
        }



        public ActionResult Payment()
        {
            return View();
        }




    }
}