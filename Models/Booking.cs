using System.ComponentModel.DataAnnotations;

namespace Cailean_Razvan_Zboruri.Models
{
    public class Booking
    {
        public int ID { get; set; }

        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }

        public string SeatNumber { get; set; }

        public int? FlightID { get; set; }
        public Flight? Flight { get; set; }

        public int? PassengerID { get; set; }
        public Passenger? Passenger { get; set; }

        public ICollection<BookingAmenity>? BookingAmenities { get; set; }
    }
}