using AttendanceWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AttendanceWebApp.Controllers
{
    public class LogsController : Controller
    {
        // GET: Logs
        public ActionResult Index(int? id)
        {
            using (var db = new ProjectEntities())
            {
                Attendance attendance = new Attendance();
                var getDate = DateTime.Now.ToLongDateString();
                var getCurrent = db.Attendances.Include(a => a.Course).Include(a => a.Student).Where(a => a.date == getDate && a.courseID == id);
                return View(getCurrent.ToList());
            } 
        }

        [HttpGet]
        public ActionResult SignAttendance()
        { 
            return View();
        }

        [HttpPost]
        public ActionResult SignAttendance(int? id, Student student, Attendance token)
        {
            
            using (var _context = new ProjectEntities())
            {
                Attendance attendance = new Attendance(); 
                string getCurrentDate = DateTime.Now.ToLongDateString();
                var getCourse = _context.Courses.Where(c => c.courseID == id).FirstOrDefault();
                var getStudentMatric = _context.Students.Where(s => s.student_matric_number == student.student_matric_number).FirstOrDefault();
                var getPassword = _context.PasswordTokens.Where(p => p.password_token == token.Course.PasswordToken.password_token).FirstOrDefault();

                if (getStudentMatric == null || getPassword == null)
                {
                    ViewData["Wrong Details"] = "Invalid Matric Number or Password Provided, please try again!!!";
                }
                else
                {
                    var getstdid = _context.CourseRegs.Where(p => p.Student.studentID == getStudentMatric.studentID).FirstOrDefault();
                    var getStudentID = _context.Attendances.Where(a => a.studentID == getStudentMatric.studentID).FirstOrDefault();
                    var getpassWordTime = _context.PasswordTokens.Where(p => p.password_token == getPassword.Course.PasswordToken.password_token).Select(p => p.time).FirstOrDefault();
                    var getServerTime = DateTime.Now.ToShortTimeString();
                    var courseIdComparison = _context.Attendances.Where(a => a.date == getCurrentDate && a.studentID == getstdid.studentID).Select(a => a.courseID).FirstOrDefault();
                    var numberOftimesStudentSignedInPreviously = _context.Attendances.Where(s => s.courseID == id && s.studentID == getstdid.studentID).GroupBy(g => g.studentID).Select(g => new { Studentrep = g.Key, StudentCount = g.Count() }).OrderBy(g => g.Studentrep);
                    int CountnumberOftimesStudentSignedInPreviously = numberOftimesStudentSignedInPreviously.Where(l => l.Studentrep == getstdid.studentID).Select(l => l.StudentCount).FirstOrDefault();
                    TimeSpan passwordTime = Convert.ToDateTime(getServerTime) - Convert.ToDateTime(getpassWordTime);

                    //if student not registered
                    if (getstdid == null)
                    {
                        ViewData["Unregistered"] = "Sorry, you cannot sign the attendance form for this course as You are not Registered for this course";
                    }
                    //Student registered and registering for the first time
                    if (getstdid != null && getStudentID == null)
                    {
                        //number of times student has signed in before
                        CountnumberOftimesStudentSignedInPreviously = 1; // student is signing for first time

                        if (passwordTime.TotalMinutes >= 5) //if time limit exceeds 5 minutes
                        {
                            ViewData["Time Expired"] = "Sorry, Cannot Sign in as Time expired!";
                        }
                        else
                        {
                            if (CountnumberOftimesStudentSignedInPreviously == 0)
                            {
                                CountnumberOftimesStudentSignedInPreviously = 1;
                            }
                            int percentage = (int)Math.Round((double)(100 * CountnumberOftimesStudentSignedInPreviously) / getCourse.number_of_days);
                            attendance.courseID = getCourse.courseID;
                            attendance.studentID = getStudentMatric.studentID;
                            attendance.attendance_percentage = percentage.ToString() + "%";
                            attendance.date = DateTime.Now.ToLongDateString();
                            _context.Attendances.Add(attendance);
                            _context.SaveChanges();
                            return RedirectToAction("StudentOwnAttendanceLogs", new { id = getstdid.studentID });
                        }

                    }
                    else if (getstdid != null && getStudentID.date != getCurrentDate)// if student registered and signed courses attendance before
                    {
                        if (passwordTime.TotalMinutes >= 5)
                        {
                            ViewData["Time Expired"] = "Sorry, Cannot Sign in as Time expired!";
                        }
                        else
                        {
                            if (CountnumberOftimesStudentSignedInPreviously == 0)
                            {
                                CountnumberOftimesStudentSignedInPreviously = 1;
                            }

                            int percentage = (int)Math.Round((double)(100 * CountnumberOftimesStudentSignedInPreviously) / getCourse.number_of_days);
                            attendance.courseID = getCourse.courseID;
                            attendance.studentID = getStudentMatric.studentID;
                            attendance.attendance_percentage = percentage.ToString() + "%";
                            attendance.date = DateTime.Now.ToLongDateString();
                            _context.Attendances.Add(attendance);
                            _context.SaveChanges();
                            return RedirectToAction("StudentOwnAttendanceLogs", new { id = getstdid.studentID });
                        }

                    }
                    else if (getstdid != null && getStudentID.date == getCurrentDate && courseIdComparison != id)
                    {
                        if (passwordTime.TotalMinutes >= 5)
                        {
                            ViewData["Time Expired"] = "Sorry, Cannot Sign in as Time expired!";
                        }
                        else
                        {
                            if (CountnumberOftimesStudentSignedInPreviously == 0)
                            {
                                CountnumberOftimesStudentSignedInPreviously = 1;
                            }

                            int percentage = (int)Math.Round((double)(100 * CountnumberOftimesStudentSignedInPreviously) / getCourse.number_of_days);
                            attendance.courseID = getCourse.courseID;
                            attendance.studentID = getStudentMatric.studentID;
                            attendance.attendance_percentage = percentage.ToString() + "%";
                            attendance.date = DateTime.Now.ToLongDateString();
                            _context.Attendances.Add(attendance);
                            _context.SaveChanges();
                            return RedirectToAction("StudentOwnAttendanceLogs", new { id = getstdid.studentID });
                        }
                    }
                    else if (getstdid != null && getStudentID.date == getCurrentDate && courseIdComparison == id)// if student registered and signed course attendance for that day
                    {
                        ViewData["Sign Once"] = "Sorry, can only sign once";
                    }
                }
                  
            }
            return View();
        } 

        public ActionResult StudentOwnAttendanceLogs(int? id)
        {
            ProjectEntities db = new ProjectEntities();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attendance attendance = new Attendance();
            var getLastAttendanceValue = db.Attendances.Where(a => a.studentID == id).ToList().Last();
            var getStudentAttendanceLogs = db.Attendances.Where(a => a.attendance_percentage == getLastAttendanceValue.attendance_percentage && a.studentID == id).ToList();
            
            if (attendance == null)
            {
                return HttpNotFound();
            }
            return View(getStudentAttendanceLogs);
        }
    }
}