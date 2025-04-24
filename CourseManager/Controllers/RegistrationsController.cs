using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseManager.Data;
using CourseManager.Models;

namespace CourseManager.Controllers
{
    public class RegistrationsController : Controller
    {
        private readonly CourseManagerContext _context;

        public RegistrationsController(CourseManagerContext context)
        {
            _context = context;
        }

        // GET: Registrations
        public async Task<IActionResult> Index()
        {
            // Lấy UserId từ Session
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return NotFound();
            }

            var registrations = await _context.Registration
                .Where(r => r.UserId == userId)
                .Include(r => r.Course)
                .ToListAsync();

            ViewData["UserId"] = userId;
            return View(registrations);
        }

        // GET: Registrations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registration = await _context.Registration
                .Include(r => r.Course)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.RegistrationId == id);
            if (registration == null)
            {
                return NotFound();
            }

            return View(registration);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(int courseId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Users","Login"); 
            }

            var isAlreadyRegistered = _context.Registration
                .Any(r => r.CourseId == courseId && r.UserId == userId);

            if (isAlreadyRegistered)
            {
                TempData["Message"] = "Bạn đã đăng ký khóa học này!";
                return RedirectToAction("Index", "Courses");
            }

            var registeredCount = _context.Registration.Count(r => r.CourseId == courseId);

            var course = _context.Course.Find(courseId);
            if (course == null)
            {
                TempData["Message"] = "Khóa học không tồn tại.";
                return RedirectToAction("Index", "Courses");
            }

            if (course.maxStudents.HasValue && registeredCount >= course.maxStudents.Value)
            {
                TempData["Message"] = "Khóa học đã đủ số lượng sinh viên!";
                return RedirectToAction("Index", "Courses");
            }

            var registration = new Registration
            {
                CourseId = courseId,
                UserId = userId.Value,
                RegistrationDate = DateTime.Now
            };

            _context.Registration.Add(registration);
            _context.SaveChanges();

            TempData["Message"] = "Đăng ký khóa học thành công!";
            return RedirectToAction("Index", "Courses");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(int registrationId)
        {
            var registration = _context.Registration
                .Include(r => r.Course)
                .FirstOrDefault(r => r.RegistrationId == registrationId);

            if (registration == null)
            {
                TempData["Error"] = "Không tìm thấy đăng ký.";
                return RedirectToAction("Index");
            }

            if (registration.Course.startDate <= DateTime.Now)
            {
                TempData["Error"] = "Không thể hủy khóa học đã khai giảng.";
                return RedirectToAction("Index");
            }

            _context.Registration.Remove(registration);
            _context.SaveChanges();

            TempData["Message"] = "Hủy đăng ký thành công.";
            return RedirectToAction("Index");
        }

    }

}
