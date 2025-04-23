namespace CourseManager.Models
{
    public class UserEditModel
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
    }
}
