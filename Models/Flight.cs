using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cailean_Razvan_Zboruri.Models
{
    // Adăugăm IValidatableObject pentru validări logice complexe
    public class Flight : IValidatableObject
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Numărul zborului este obligatoriu.")]
        [StringLength(10)] // Protejăm baza de date
        [Display(Name = "Număr Zbor")]
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

        // VALIDARE CUSTOM: Nu poți ateriza înainte să decolezi!
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ArrivalTime <= DepartureTime)
            {
                yield return new ValidationResult(
                    "Data și ora sosirii trebuie să fie ulterioare datei de plecare.",
                    new[] { nameof(ArrivalTime) });
            }
        }
    }
}