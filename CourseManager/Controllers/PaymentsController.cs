using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseManager.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using CourseManager.Data;

namespace CourseManager.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly CourseManagerContext _context;

        public PaymentsController(CourseManagerContext context)
        {
            _context = context;
        }

        // GET: Payments/Create?courseId=5
        public IActionResult Create(int courseId)
        {
            // Lấy UserId từ Session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Kiểm tra xem user đã đăng ký khóa học chưa
            var registration = _context.Registration
                .FirstOrDefault(r => r.CourseId == courseId && r.UserId == userId);

            if (registration == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa đăng ký khóa học này";
                return RedirectToAction("Details", "Courses", new { id = courseId });
            }

            // Tạo model mới với thông tin cơ bản
            var payment = new Payment
            {
                RegistrationId = registration.RegistrationId,
                Amount = _context.Course.Find(courseId)?.fee // Lấy học phí từ khóa học
            };

            return View(payment);
        }

        // POST: Payments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RegistrationId,Amount")] Payment payment)
        {
            // Lấy UserId từ Session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Kiểm tra xem registration có thuộc về user này không
            var registration = await _context.Registration
                .FirstOrDefaultAsync(r => r.RegistrationId == payment.RegistrationId && r.UserId == userId);

            if (registration == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Thiết lập ngày thanh toán là hiện tại
                    payment.Date = DateTime.Now;

                    _context.Add(payment);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Thanh toán thành công!";
                    return RedirectToAction("Detail", new { id = payment.PaymentId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi thanh toán: " + ex.Message);
                }
            }

            // Nếu có lỗi, hiển thị lại form với thông báo
            return View(payment);
        }

        // GET: Payments/Detail/5
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payment
                .Include(p => p.Registration)
                    .ThenInclude(r => r.Course)
                .Include(p => p.Registration)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(m => m.PaymentId == id);

            if (payment == null)
            {
                return NotFound();
            }

            // Kiểm tra xem payment có thuộc về user hiện tại không
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || payment.Registration.UserId != userId)
            {
                return Forbid();
            }

            return View(payment);
        }

        private bool PaymentExists(int id)
        {
            return _context.Payment.Any(e => e.PaymentId == id);
        }
    }
}