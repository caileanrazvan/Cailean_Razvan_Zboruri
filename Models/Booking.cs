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
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        // Statusul plății (Pending, Paid, Failed)
        public string PaymentStatus { get; set; } = "Pending";

        // ID-ul tranzacției generat după plată
        public string? TransactionId { get; set; }

        // O rezervare are mai mulți pasageri
        public ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();

        public ICollection<BookingAmenity>? BookingAmenities { get; set; }
    }
}