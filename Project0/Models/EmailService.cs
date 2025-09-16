using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace Project0.Models
{
    public class EmailService
    {


        public static void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("sondos.nice.girl1@gmail.com");
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.Credentials = new NetworkCredential("sondos.nice.girl1@gmail.com", "cpduhhbmriwlnrvo");
                smtpClient.EnableSsl = true;

                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                // الأفضل تخزين الخطأ أو تسجيله في Log وليس فقط Console
                throw new Exception("فشل في إرسال البريد الإلكتروني: " + ex.Message);
            }
        }
    }
}
