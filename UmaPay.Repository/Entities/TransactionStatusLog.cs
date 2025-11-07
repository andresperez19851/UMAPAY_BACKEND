using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UmaPay.Repository.Entities
{
    public class TransactionStatusLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionStatusLogId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public string? Comment { get; set; }

        [Required]
        public int TransactionId { get; set; }

        [ForeignKey("TransactionId")]
        public Transaction? Transactions { get; set; }

        [Required]
        public int StatusId { get; set; }

        [ForeignKey("StatusId")]
        public TransactionStatus TransactionStatuses { get; set; }

    }

}