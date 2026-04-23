using Microsoft.AspNetCore.Mvc.RazorPages;
using Cailean_Razvan_Zboruri.Data;
using System.Collections.Generic;
using System.Linq;

namespace Cailean_Razvan_Zboruri.Models
{
    public class BookingAmenitiesPageModel : PageModel
    {
        public List<AssignedAmenityData> AssignedAmenityDataList;

        public void PopulateAssignedAmenityData(AviationContext context, Booking booking)
        {
            var allAmenities = context.Amenity;
            var bookingAmenities = new HashSet<int>(booking.BookingAmenities?.Select(c => c.AmenityID) ?? new List<int>());

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

        public void UpdateBookingAmenities(AviationContext context, string[] selectedAmenities, Booking bookingToUpdate)
        {
            // FIX 1: Daca lista e null din baza de date, o initializam goala ca sa nu crape .Select()
            if (bookingToUpdate.BookingAmenities == null)
            {
                bookingToUpdate.BookingAmenities = new List<BookingAmenity>();
            }

            // FIX 2: Daca utilizatorul a debifat TOT (selectedAmenities e null sau gol)
            if (selectedAmenities == null || selectedAmenities.Length == 0)
            {
                // Trebuie sa stergem toate legaturile existente din baza de date
                foreach (var item in bookingToUpdate.BookingAmenities.ToList())
                {
                    context.Remove(item);
                }
                bookingToUpdate.BookingAmenities.Clear();
                return;
            }

            var selectedAmenitiesHS = new HashSet<string>(selectedAmenities);

            // FIX 3: Folosim AmenityID in loc de Amenity.ID, e mult mai sigur
            var bookingAmenities = new HashSet<int>(bookingToUpdate.BookingAmenities.Select(c => c.AmenityID));

            foreach (var amenity in context.Amenity)
            {
                if (selectedAmenitiesHS.Contains(amenity.ID.ToString()))
                {
                    // Daca e bifat, dar nu exista in baza de date -> ADAUGAM
                    if (!bookingAmenities.Contains(amenity.ID))
                    {
                        bookingToUpdate.BookingAmenities.Add(new BookingAmenity
                        {
                            BookingID = bookingToUpdate.ID,
                            AmenityID = amenity.ID
                        });
                    }
                }
                else
                {
                    // Daca NU e bifat, dar exista in baza de date -> STERGEM
                    if (bookingAmenities.Contains(amenity.ID))
                    {
                        BookingAmenity amenityToRemove = bookingToUpdate.BookingAmenities.FirstOrDefault(i => i.AmenityID == amenity.ID);
                        if (amenityToRemove != null)
                        {
                            context.Remove(amenityToRemove);
                        }
                    }
                }
            }
        }
    }
}