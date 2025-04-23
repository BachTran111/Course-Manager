// Models/RegisterViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace CourseManager.Models
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu phải trùng nhau.")]
        public string ConfirmPassword { get; set; }
    }
}
