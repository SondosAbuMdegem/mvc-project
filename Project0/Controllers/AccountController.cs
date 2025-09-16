using Project0.Controllers;
using Project0.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.SqlClient;

public class AccountController : Controller
{
    [HttpGet]
    public ActionResult SignUp()
    {
        return View();
    }

    public ActionResult Login()
    {

        return View();
    }

    [HttpPost]
    public ActionResult SignUp(User model)
    {
        if (ModelState.IsValid)
        {
            using (var context = new LibraryDbContext())
            {
                if (context.Users.Any(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "This username is already taken.");
                    return View(model);
                }
                if (context.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "This email is already taken.");
                    return View(model);
                }

                model.Password = HashingHelper.ComputeSha256Hash(model.Password);

                if (string.IsNullOrEmpty(model.Role))
                    model.Role = "user";

                context.Users.Add(model);
                context.SaveChanges();

            }

            return RedirectToAction("Login");
        }

        return View(model);
    }


    [HttpPost]
    public ActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            using (var context = new LibraryDbContext())
            {
                string hashedPassword = HashingHelper.ComputeSha256Hash(model.Password);

               
                var user = context.Users
                    .SqlQuery("SELECT * FROM Users WHERE Username = @username AND Password = @password",
                              new SqlParameter("@username", model.Username),
                              new SqlParameter("@password", hashedPassword))
                    .FirstOrDefault();

                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(user.Username, false);

                    if (user.Role == "admin")
                        return RedirectToAction("AdminPanel", "Admin");
                    else
                        return RedirectToAction("HomeC", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }
        }
        return View(model);
    }


    // GET: ForgotPassword
    [HttpGet]
    public ActionResult ForgotPassword()
    {
        return View();
    }

    // POST: ForgotPassword
    [HttpPost]
    public ActionResult ForgotPassword(string email)
    {
        using (var context = new LibraryDbContext())
        {
            var user = context.Users.FirstOrDefault(u => u.Email == email);

            if (user != null)
            {
                
                string token = Guid.NewGuid().ToString();
                user.ResetPasswordToken = token;
                user.ResetTokenExpiry = DateTime.Now.AddHours(1);

                context.SaveChanges();

              
                string resetLink = Url.Action("ResetPassword", "Account",
                    new { email = user.Email, token = token }, protocol: Request.Url.Scheme);

                string subject = "Password Reset Request";
                string body = $"Click <a href='{resetLink}'>here</a> to reset your password.";

              
                EmailService.SendEmail(user.Email, subject, body);

                TempData["Message"] = "Password reset link has been sent to your email.";
                TempData["AlertType"] = "info";
            }
            else
            {
                ModelState.AddModelError("", "Email not found.");
            }
        }

        return View();
    }


    // GET: ResetPassword
    public ActionResult ResetPassword(string email, string token)
    {
        var model = new ResetPasswordViewModel
        {
            Email = email,
            Token = token
        };

        return View(model);
    }


    // POST: ResetPassword
    [HttpPost]
    public ActionResult ResetPassword(ResetPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            using (var context = new LibraryDbContext())
            {
                var user = context.Users.FirstOrDefault(u =>
                    u.Email == model.Email &&
                    u.ResetPasswordToken == model.Token &&
                    u.ResetTokenExpiry > DateTime.Now);

                if (user != null)
                {
                   
                    using (var sha = SHA256.Create())
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(model.NewPassword);
                        byte[] hash = sha.ComputeHash(bytes);
                        user.Password = BitConverter.ToString(hash).Replace("-", "").ToLower();
                    }

                  
                    user.ResetPasswordToken = null;
                    user.ResetTokenExpiry = null;

                    context.SaveChanges();

                    TempData["Message"] = "Password updated successfully.";
                    TempData["AlertType"] = "success";

                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid or expired reset token.");
                }
            }
        }

        return View(model);
    }


    [HttpPost]
    public ActionResult LogOut()
    {
        FormsAuthentication.SignOut();
        return RedirectToAction("Home", "Home");
    }
}