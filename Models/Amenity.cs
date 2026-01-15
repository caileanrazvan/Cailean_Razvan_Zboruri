using System.ComponentModel.DataAnnotations.Schema;

namespace Cailean_Razvan_Zboruri.Models
{
    public class Amenity
    {
        public int ID { get; set; }
        public string Name { get; set; }

        [Column(TypeName = "decimal(6, 2)")]
        public decimal Price { get; set; }

        public ICollection<BookingAmenity>? BookingAmenities { get; set; }
    }
}