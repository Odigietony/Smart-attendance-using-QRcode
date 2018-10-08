using AttendanceWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using QRCoder;
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;

namespace AttendanceWebApp.Controllers
{
    public class StaffController : Controller
    {
        private ProjectEntities db = new ProjectEntities();
        // GET: Staff
        public ActionResult Index()
        {
            Course course = new Course();
            IEnumerable<LecturerReg> similar = db.LecturerRegs.ToList();
            ViewBag.getSimilar = similar;
            return View(course);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Lecturer lecturer)
        {
            using (var db = new ProjectEntities())
            {
                var staffVerification = db.Lecturers.Where(l => l.lecturer_email == lecturer.lecturer_email 
                && l.lecturer_password == lecturer.lecturer_password).FirstOrDefault(); 
                if (staffVerification != null)
                {
                    Session["Name"] = staffVerification.lecturer_name.ToString();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect Email or Password!!");
                }
                return View();
            }
        }
         

        public ActionResult GeneratePassword(PasswordToken passwordToken, int? id)
        {

            string password = Membership.GeneratePassword(8, 2);
            string currentTime = DateTime.Now.ToLongTimeString();
            string currentDate = DateTime.Now.ToLongDateString();
            
            using (var _context = new ProjectEntities())
            {
                Course course = new Course();
                var getCourseID = _context.Courses.Where(c => c.courseID == id).FirstOrDefault();
                passwordToken.password_token = password;
                passwordToken.time = currentTime;
                passwordToken.date = currentDate;
                passwordToken.courseID = getCourseID.courseID;
                getCourseID.passwordID = passwordToken.passwordID;
                _context.Entry(getCourseID).State = EntityState.Modified;
                _context.PasswordTokens.Add(passwordToken);
                _context.SaveChanges();
                ViewData["GeneratedPassword"] = getCourseID.PasswordToken.password_token.ToString();
                return RedirectToAction("Index");
            }
           // return View();
        }

        //GET: Staff/GenerateQrCode/id
        public ActionResult GenerateQrCode(int? id)
        {
            using (var db = new ProjectEntities())
            { 
                var index = db.Courses.Where(c => c.courseID == id).FirstOrDefault();
                if (index != null)
                {
                    string pageUrl = "http://localhost:59524/Logs/SignAttendance/" + id;
                    QRCodeEncoder codeEncoder = new QRCodeEncoder();
                    Bitmap bitmap = codeEncoder.Encode(pageUrl);
                    bitmap.Save($"C:\\Users\\odigietony\\source\\repos\\AttendanceWebApp\\AttendanceWebApp\\QrImage\\{index.courseID}.jpg" ,ImageFormat.Jpeg);
                    var QrImage = Url.Content($"~/QrImage/{index.courseID}.jpg");
                    ViewData["QrImage"] = QrImage;
                }
                return View("GenerateQrCode");
            } 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }


}