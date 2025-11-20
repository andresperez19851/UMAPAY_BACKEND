using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UmaPay.Repository.Entities
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        public DateTime? SapDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        [Required]
        public Guid Token { get; set; }
        
        [StringLength(400)]
        public string? PaymentUrl { get; set; }

        [StringLength(30)]
        public string? Reference { get; set; }

        public string? GatewayResponse { get; set; }

        public string? GatewayRequest { get; set; }

        public string? GatewayPayment { get; set; }

        public string? SapRequest { get; set; }

        public string? SapResponse { get; set; }

        public string? SapDocument { get; set; }


        [Required]
        public int StatusId { get; set; }

        [ForeignKey("StatusId")]
        public TransactionStatus TransactionStatuses { get; set; }

        public int? GatewayApplicationId { get; set; }

        [ForeignKey("GatewayApplicationId")]
        public GatewayApplication? GatewayApplications { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customers { get; set; }

        [Required]
        public int CountryId { get; set; }

        [ForeignKey("CountryId")]
        public Country Countries { get; set; }

    }

}