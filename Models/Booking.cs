using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Cailean_Razvan_Zboruri.Models
{
    public class Booking
    {
        public int ID { get; set; }

        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        public string SeatNumber { get; set; }

        public int? FlightID { get; set; }
        public Flight? Flight { get; set; }
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }

        // O rezervare are mai mulți pasageri
        public ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();

        public ICollection<BookingAmenity>? BookingAmenities { get; set; }
    }
}