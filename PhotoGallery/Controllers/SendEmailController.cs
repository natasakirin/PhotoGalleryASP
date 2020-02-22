using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoGallery.Models;

namespace PhotoGallery.Controllers
{
    [Authorize]
    public class SendEmailController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Index(Email em)
        {
            string to = em.To;
            string subject = em.Subject;
            string body = em.Body;
            MailMessage mm = new MailMessage();
            mm.To.Add(to);
            mm.Subject = subject;
            mm.Body = body;
            mm.From = new MailAddress("lu.asp@yandex.com");
            mm.IsBodyHtml = false;
            SmtpClient smtp = new SmtpClient("smtp.yandex.com");
            smtp.Port = 587;
            smtp.UseDefaultCredentials = true;
            smtp.EnableSsl = true;
            smtp.Credentials = new System.Net.NetworkCredential("lu.asp@yandex.com", "Pass.123");
            smtp.Send(mm);
            ViewBag.message = "The Mail Has Been Sent To " + em.To + " successfully.";
            return View(em);

        }
    }
}


