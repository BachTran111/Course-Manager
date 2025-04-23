using System.Diagnostics;
using CourseManager.Data;
using CourseManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace CourseManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CourseManagerContext _context;

        public HomeController(CourseManagerContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var courses = _context.Course.ToList();
            ViewBag.Courses = courses; 

            return View(courses);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
