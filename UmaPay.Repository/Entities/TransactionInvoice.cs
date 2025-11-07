using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UmaPay.Repository.Entities
{
    public class TransactionInvoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionInvoiceId { get; set; }

        [Required]
        [StringLength(50)]
        public string Number { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Total { get; set; }

        public bool IsPaid { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? TotalToPay { get; set; }

        [Required]
        public int TransactionId { get; set; }

        [ForeignKey("TransactionId")]
        public Transaction Transaction { get; set; }

        [Required]
        public int StatusId { get; set; }

        [ForeignKey("StatusId")]
        public TransactionStatus TransactionStatuses { get; set; }
    }
}
