using System;
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
    public class UsersController : Controller
    {
        private readonly CourseManagerContext _context;

        public UsersController(CourseManagerContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var courseManagerContext = _context.User.Include(u => u.Role);
            return View(await courseManagerContext.ToListAsync());
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null || id != currentUserId)
            {
                return NotFound();
            }

            var userVM = await _context.User
                .Where(u => u.UserId == id)
                .Select(u => new User
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Username = u.Username,
                    DateOfBirth = u.DateOfBirth,
                    PhoneNumber = u.PhoneNumber,
                    Password = u.Password,
                    RoleId = u.RoleId
                })
                .SingleOrDefaultAsync();

            if (userVM == null)
            {
                return NotFound();
            }

            return View(userVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User model)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != currentUserId || id != model.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _context.User
                    .Where(u => u.UserId == id)
                    .Select(u => new User
                    {
                        UserId = u.UserId,
                        Username = u.Username,
                        Email = u.Email,
                        FullName = u.FullName,
                        DateOfBirth = u.DateOfBirth,
                        PhoneNumber = u.PhoneNumber,
                        Password = u.Password,
                        RoleId = u.RoleId
                    })
                    .SingleOrDefaultAsync();

                if (user == null)
                {
                    return NotFound();
                }

                user.FullName = model.FullName;
                user.DateOfBirth = model.DateOfBirth;
                user.PhoneNumber = model.PhoneNumber;
                user.Password = model.Password;

                _context.Update(user);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.UserId == id);
        }
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            bool emailExists = await _context.User.AnyAsync(u => u.Email == vm.Email);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng.");
                return View(vm);
            }

            bool usernameExists = await _context.User.AnyAsync(u => u.Username == vm.Username);
            if (usernameExists)
            {
                ModelState.AddModelError("Username", "Username đã tồn tại.");
                return View(vm);
            }

            var user = new User
            {
                Email = vm.Email,
                Username = vm.Username,
                Password = vm.Password, 
                RoleId = 2,
                FullName = "N/A",
                PhoneNumber = "N/A",
                DateOfBirth = new DateTime(2000, 1, 1)
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                var user = await _context.User
                    .Where(u =>
                        u.Username != null && u.Password != null &&
                        u.Username == vm.Username && u.Password == vm.Password).Select(u=>new {u.UserId, u.RoleId, u.Username }).SingleOrDefaultAsync();

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
                    return View(vm);
                }

                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetInt32("RoleId", user.RoleId);
                HttpContext.Session.SetString("Username", user.Username);

                Console.WriteLine($"Username: {user.Username}, RoleId: {user.RoleId}");

                if (user.RoleId == 1)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Đăng nhập lỗi: " + ex.Message);
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi trong quá trình đăng nhập. Vui lòng thử lại.");
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            Console.WriteLine("Người dùng đã đăng xuất.");

            return RedirectToAction("Login", "Users");
        }

    }
}
