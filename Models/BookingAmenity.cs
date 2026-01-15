namespace Cailean_Razvan_Zboruri.Models
{
    public class BookingAmenity
    {
        public int ID { get; set; }
        public int BookingID { get; set; }
        public Booking Booking { get; set; }

        public int AmenityID { get; set; }
        public Amenity Amenity { get; set; }
    }
}