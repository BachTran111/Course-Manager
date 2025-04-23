using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManager.Models
{
    [Table("registrations")]
    public class Registration
    {
        [Key]
        [Column("registration_id")]
        public int RegistrationId { get; set; }

        [Column("course_id")]
        public int CourseId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("registration_date")]
        [DataType(DataType.DateTime)]
        public DateTime? RegistrationDate { get; set; }

        // Navigation properties
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
