using AttendanceWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AttendanceWebApp.Controllers
{
    public class GeneratorController : Controller
    {
        // GET: Generator
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GeneratePassword(PasswordToken passwordToken)
        {
            string password = Membership.GeneratePassword(8, 2);
            string currentTime = DateTime.Now.ToLongTimeString();
            string currentDate = DateTime.Now.ToLongDateString();

            using (var _context = new ProjectEntities())
            {
                passwordToken.password_token = password;
                passwordToken.time = currentTime;
                passwordToken.date = currentDate;
                _context.PasswordTokens.Add(passwordToken);
                _context.SaveChanges();
            }
            return View();
        }
    }
}