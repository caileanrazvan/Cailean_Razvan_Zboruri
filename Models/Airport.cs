using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cailean_Razvan_Zboruri.Models
{
    public class Airport
    {
        public int ID { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Codul IATA trebuie sa aiba exact 3 litere (ex: OTP).")]
        [Display(Name = "Cod IATA")]
        public string IataCode { get; set; }

        [Required]
        [Display(Name = "Oras")]
        public string City { get; set; }

        public string Country { get; set; }
        public string Name { get; set; }

        [InverseProperty("DepartureAirport")]
        public ICollection<Flight>? Departures { get; set; }

        [InverseProperty("ArrivalAirport")]
        public ICollection<Flight>? Arrivals { get; set; }
    }
}