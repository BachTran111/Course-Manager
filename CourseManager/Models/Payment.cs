using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManager.Models
{
    [Table("payments")]
    public class Payment
    {
        [Key]
        [Column("payment_id")]
        public int PaymentId { get; set; }

        [Column("registration_id")]
        public int RegistrationId { get; set; }

        [Column("amount", TypeName = "decimal(12,2)")]
        [DataType(DataType.Currency)]
        public decimal? Amount { get; set; }

        [Column("date")]
        [DataType(DataType.DateTime)]
        public DateTime? Date { get; set; }

        // Navigation property
        [ForeignKey("RegistrationId")]
        public virtual Registration Registration { get; set; }
    }
}
