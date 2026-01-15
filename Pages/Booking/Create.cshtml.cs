using Cailean_Razvan_Zboruri.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Cailean_Razvan_Zboruri.Data;


namespace Cailean_Razvan_Zboruri.Pages.Booking
{
    public class CreateModel : BookingAmenitiesPageModel
    {
        private readonly Cailean_Razvan_Zboruri.Data.AviationContext _context;

        public CreateModel(Cailean_Razvan_Zboruri.Data.AviationContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["PassengerID"] = new SelectList(_context.Passenger, "ID", "FullName");
            ViewData["FlightID"] = new SelectList(_context.Flight, "ID", "FlightNumber");

            var booking = new Cailean_Razvan_Zboruri.Models.Booking();
            booking.BookingAmenities = new List<BookingAmenity>();

            PopulateAssignedAmenityData(_context, booking);

            return Page();
        }

        [BindProperty]
        public Cailean_Razvan_Zboruri.Models.Booking Booking { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync(string[] selectedAmenities)
        {
            if (selectedAmenities != null)
            {
                Booking.BookingAmenities = new List<BookingAmenity>();

                foreach (var amenity in selectedAmenities)
                {
                    var amenityToAdd = new BookingAmenity
                    {
                        AmenityID = int.Parse(amenity)
                    };
                    Booking.BookingAmenities.Add(amenityToAdd);
                }
            }

            _context.Booking.Add(Booking);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}