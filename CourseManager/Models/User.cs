using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace CourseManager.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("full_name")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Column("DateOfBirth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Column("phoneNumber")]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Column("email")]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Column("username")]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [Column("password")]
        [StringLength(200)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Column("role_id")]
        public int RoleId { get; set; }

        // Navigation property
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
    }
}
