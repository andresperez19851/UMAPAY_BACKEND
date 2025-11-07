using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UmaPay.Repository.Entities
{
    public class Gateway
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GatewayId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string Code { get; set; }

        public ICollection<GatewayCountry> GatewayCountries { get; set; }
        public ICollection<GatewayApplication> GatewayApplications { get; set; }
    }
}