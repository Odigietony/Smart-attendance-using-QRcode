using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AttendanceWebApp.Models;

namespace AttendanceWebApp.Controllers
{
    public class AdminController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Admin admin)
        {
            if (ModelState.IsValid)
            {
                using (var db = new ProjectEntities())
                {
                    var adminDetails = db.Admins.Where(a => a.username == admin.username && a.password == admin.password).FirstOrDefault();
                    if (adminDetails != null)
                    {
                        return RedirectToAction("Index");
                    }
                }  
            }
            return View(admin);
        }

        [HttpGet]
        public ActionResult StaffList()
        {
            using (var db = new ProjectEntities())
            {
                return View(db.Lecturers.ToList());
            }  
        }

        [HttpGet]
        public ActionResult StaffRegistration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StaffRegistration(Lecturer lecturer)
        {
            using (var _context = new ProjectEntities() )
            {
                if (ModelState.IsValid)
                {
                    _context.Lecturers.Add(lecturer);
                    _context.SaveChanges();
                    return RedirectToAction("StaffList");
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult Students()
        {
            using (var db = new ProjectEntities())
            {
                return View(db.Students.ToList());
            }   
        }

        [HttpGet]
        public ActionResult StudentRegistration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StudentRegistration(Student student)
        {
            using (var _context = new ProjectEntities())
            {
                if (ModelState.IsValid)
                {
                    _context.Students.Add(student);
                    _context.SaveChanges();
                    return RedirectToAction("Students");
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult Courses()
        {
            using (var db = new ProjectEntities())
            {
                return View(db.Courses.ToList());
            }
        }

        [HttpGet]
        public ActionResult CourseCreation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CourseCreation(Course course)
        {
            using (var _context = new ProjectEntities())
            {
                if (ModelState.IsValid)
                {
                    _context.Courses.Add(course);
                    _context.SaveChanges();
                    return RedirectToAction("Courses");
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult CourseRegList()
        {
            using (var db = new ProjectEntities())
            {
                var courseRegs = db.CourseRegs.Include(c => c.Course).Include(c => c.Student);
                return View(courseRegs.ToList());
            }
        }

        [HttpGet]
        public ActionResult CourseReg()
        {
            using (var db = new ProjectEntities())
            {
                ViewBag.courseID = new SelectList(db.Courses.ToList(), "courseID", "course_title");
                ViewBag.studentID = new SelectList(db.Students.ToList(), "studentID", "student_name");
                return View();
            }
        }

        [HttpPost]
        public ActionResult CourseReg(CourseReg courseReg)
        {
            using (var _context = new ProjectEntities())
            {
                if (ModelState.IsValid)
                {
                    _context.CourseRegs.Add(courseReg);
                    _context.SaveChanges();
                    return RedirectToAction("CourseRegList");
                }
                ViewBag.courseID = new SelectList(_context.Courses.ToList(), "courseID", "course_title", courseReg.courseID);
                ViewBag.studentID = new SelectList(_context.Students.ToList(), "studentID", "student_name", courseReg.studentID);
                return View(courseReg);
            }
           // return View(courseReg);
        }

        [HttpGet]
        public ActionResult LecturerRegList()
        {
            using (var db = new ProjectEntities())
            {
                return View(db.LecturerRegs.Include(c => c.Course).Include(s => s.Lecturer).ToList());
            }
        }

        [HttpGet]
        public ActionResult LecturerReg()
        {
            using (var db = new ProjectEntities())
            {
                ViewBag.courseID = new SelectList(db.Courses.ToList(), "courseID", "course_title");
                ViewBag.lecturerID = new SelectList(db.Lecturers.ToList(), "lecturerID", "lecturer_name");
                return View();
            }
        }

        [HttpPost]
        public ActionResult LecturerReg(LecturerReg lecturerReg)
        {
            using (var _context = new ProjectEntities())
            {
                if (ModelState.IsValid)
                {
                    _context.LecturerRegs.Add(lecturerReg);
                    _context.SaveChanges();
                    return RedirectToAction("LecturerRegList");
                }
                ViewBag.courseID = new SelectList(_context.Courses.ToList(), "courseID", "course_title");
                ViewBag.lecturerID = new SelectList(_context.Lecturers.ToList(), "lecturerID", "lecturer_name");
            }
            return View();
        }
 
    }
}