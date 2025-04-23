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

        public async Task<IActionResult> Index()
        {
            var totalCourses = await _context.Course.CountAsync();
            var totalUsers = await _context.User.CountAsync();
            var totalRegistrations = await _context.Registration.CountAsync();

            ViewData["TotalCourses"] = totalCourses;
            ViewData["TotalUsers"] = totalUsers;
            ViewData["TotalRegistrations"] = totalRegistrations;

            return View();
        }
    }
}
