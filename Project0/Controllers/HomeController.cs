using Project0.Models;
using System.Linq;
using System.Web.Mvc;

namespace Project1.Controllers
{
    public class HomeController : Controller
    {
        private LibraryDbContext context = new LibraryDbContext();
        public ActionResult UserHome()
        {
            ViewBag.Username = TempData["Username"];
            return View();
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult AdminDashboard()
        {

            return View();
        }
        public ActionResult Home()
        {
            using (var _context = new LibraryDbContext())
            {
                var books = _context.Book.Take(10).ToList(); // عرض 5 كتب فقط
                return View(books);
            }
        }
        public ActionResult HomeC()
        {
            using (var _context = new LibraryDbContext())
            {
                var books = _context.Book.Take(10).ToList(); // عرض 5 كتب فقط
                return View(books);
            }
        }
    }
}