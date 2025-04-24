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
            var courseManagerContext = _context.Registration.Include(r => r.Course).Include(r => r.User);
            return View(await courseManagerContext.ToListAsync());
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

        // POST: Registrations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RegistrationId,CourseId,UserId,RegistrationDate")] Registration registration)
        {
            if (ModelState.IsValid)
            {
                _context.Add(registration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "courseId", "courseCode", registration.CourseId);
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "FullName", registration.UserId);
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

        // GET: Registrations/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Registrations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var registration = await _context.Registration.FindAsync(id);
            if (registration != null)
            {
                _context.Registration.Remove(registration);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegistrationExists(int id)
        {
            return _context.Registration.Any(e => e.RegistrationId == id);
        }
    }
}
