using Cailean_Razvan_Zboruri.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cailean_Razvan_Zboruri.Models
{
    public class BookingAmenitiesPageModel : PageModel
    {
        public List<AssignedAmenityData> AssignedAmenityDataList;

        public void PopulateAssignedAmenityData(AviationContext context, Booking booking)
        {
            var allAmenities = context.Amenity;
            var bookingAmenities = new HashSet<int>(
                booking.BookingAmenities.Select(c => c.AmenityID));

            AssignedAmenityDataList = new List<AssignedAmenityData>();

            foreach (var amenity in allAmenities)
            {
                AssignedAmenityDataList.Add(new AssignedAmenityData
                {
                    AmenityID = amenity.ID,
                    Name = amenity.Name,
                    Assigned = bookingAmenities.Contains(amenity.ID)
                });
            }
        }
    }
}