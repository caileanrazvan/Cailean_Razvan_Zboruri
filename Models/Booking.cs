using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Cailean_Razvan_Zboruri.Models
{
    public class Booking
    {
        public int ID { get; set; }

        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        public int? FlightID { get; set; }
        public Flight? Flight { get; set; }

        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Adresa de email nu este validă.")]
        public string ContactEmail { get; set; }

        [Required]
        [Phone(ErrorMessage = "Numărul de telefon nu este valid.")]
        public string ContactPhone { get; set; }

        // Statusul plății (Pending, Paid, Anulat)
        // (La nivel pro, se folosește un Enum, dar string-ul este ok pentru stadiul actual)
        public string PaymentStatus { get; set; } = "Pending";

        public string? TransactionId { get; set; }

        public ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();
        public ICollection<BookingAmenity>? BookingAmenities { get; set; }
    }
}