using Microsoft.AspNetCore.Mvc;
using CourseManager.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CourseManager.Controllers
{
    public class AdminsController : Controller
    {
        private readonly CourseManagerContext _context;

        public AdminsController(CourseManagerContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string period = "day", string sortCourse = "desc", string sortPeriod = "asc")
        {
            var currentRoleId = HttpContext.Session.GetInt32("RoleId");
            if (currentRoleId != 1)
            {
                return NotFound();
            }
            ViewData["TotalCourses"] = await _context.Course.CountAsync();
            ViewData["TotalUsers"] = await _context.User.CountAsync();
            ViewData["TotalRegistrations"] = await _context.Registration.CountAsync();

            var payments = await _context.Payment
                .Include(p => p.Registration).ThenInclude(r => r.Course)
                .Where(p => p.Amount != null && p.Date != null)
                .ToListAsync();

            // 1) Doanh thu theo khóa học
            var revenueByCourse = payments
                .GroupBy(p => p.Registration.Course.courseName)
                .Select(g => new {
                    CourseName = g.Key,
                    TotalRevenue = g.Sum(x => x.Amount ?? 0)
                });

            revenueByCourse = sortCourse == "asc"
                ? revenueByCourse.OrderBy(x => x.TotalRevenue)
                : revenueByCourse.OrderByDescending(x => x.TotalRevenue);

            ViewBag.SortCourse = sortCourse;
            ViewBag.RevenueByCourse = revenueByCourse.ToList();

            // 2) Doanh thu theo khoảng (day/month/year)
            IEnumerable<dynamic> revenueByPeriod;
            switch (period.ToLower())
            {
                case "month":
                    revenueByPeriod = payments
                        .GroupBy(p => new { p.Date.Value.Year, p.Date.Value.Month })
                        .Select(g => new {
                            Period = new DateTime(g.Key.Year, g.Key.Month, 1),
                            TotalRevenue = g.Sum(x => x.Amount ?? 0)
                        });
                    break;
                case "year":
                    revenueByPeriod = payments
                        .GroupBy(p => p.Date.Value.Year)
                        .Select(g => new {
                            Period = new DateTime(g.Key, 1, 1),
                            TotalRevenue = g.Sum(x => x.Amount ?? 0)
                        });
                    break;
                default: // "day"
                    revenueByPeriod = payments
                        .GroupBy(p => p.Date.Value.Date)
                        .Select(g => new {
                            Period = g.Key,
                            TotalRevenue = g.Sum(x => x.Amount ?? 0)
                        });
                    break;
            }

            revenueByPeriod = sortPeriod == "asc"
                ? revenueByPeriod.OrderBy(x => x.Period)
                : revenueByPeriod.OrderByDescending(x => x.Period);

            ViewBag.Period = period.ToLower();
            ViewBag.SortPeriod = sortPeriod;
            ViewBag.RevenuePeriod = revenueByPeriod.ToList();

            return View();
        }

    }
}
