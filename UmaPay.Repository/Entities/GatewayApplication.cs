using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace UmaPay.Repository.Entities
{
    public class GatewayApplication
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GatewayApplicationId { get; set; }

        public int GatewayId { get; set; }

        [ForeignKey("GatewayId")]
        public Gateway Gateways { get; set; }

        public int ApplicationId { get; set; }

        [ForeignKey("ApplicationId")]
        public Application Applications { get; set; }
    }
}
