using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManager.Models
{
    [Table("courses")]
    public class Course
    {
        [Key]
        [Column("course_id")]
        public int courseId { get; set; }

        [Required]
        [Column("course_code")]
        [StringLength(20)]
        public string courseCode { get; set; }

        [Required]
        [Column("course_name")]
        [StringLength(150)]
        public string courseName { get; set; }

        [Column("instructor")]
        [StringLength(100)]
        public string instructor { get; set; }

        [Column("start_date")]
        [DataType(DataType.Date)]
        public DateTime? startDate { get; set; }

        [Column("fee", TypeName = "decimal(18,2)")]
        [DataType(DataType.Currency)]
        public decimal? fee { get; set; }

        [Column("max_students")]
        public int? maxStudents { get; set; }
    }
}
