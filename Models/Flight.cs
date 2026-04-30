using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cailean_Razvan_Zboruri.Models
{
    public class Flight
    {
        public int ID { get; set; }

        [Display(Name = "Numar Zbor")]
        public string FlightNumber { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DepartureTime { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ArrivalTime { get; set; }

        [Required]
        [Range(1, 10000, ErrorMessage = "Prețul trebuie să fie între 1 și 10000.")]
        [Column(TypeName = "decimal(6, 2)")]
        public decimal BasePrice { get; set; }
        public bool IsCancelled { get; set; } = false;

        [Display(Name = "Aeroport Plecare")]
        public int DepartureAirportID { get; set; }
        public Airport? DepartureAirport { get; set; }

        [Display(Name = "Aeroport Sosire")]
        public int ArrivalAirportID { get; set; }
        public Airport? ArrivalAirport { get; set; }

        public ICollection<Booking>? Bookings { get; set; }
    }
}