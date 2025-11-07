using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UmaPay.Repository.Entities
{
    public class GatewayCountry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GatewayCountryId { get; set; }

        public int GatewayId { get; set; }

        [ForeignKey("GatewayId")]
        public Gateway Gateways { get; set; }

        public int CountryId { get; set; }

        [ForeignKey("CountryId")]
        public Country Countries { get; set; }
    }
}
