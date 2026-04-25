using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cailean_Razvan_Zboruri.Pages.Booking
{
    public class EditModel : BookingAmenitiesPageModel
    {
        private readonly AviationContext _context;
        public EditModel(AviationContext context) { _context = context; }

        [BindProperty]
        public Models.Booking Booking { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Booking = await _context.Booking
                .Include(b => b.Passengers)
                .Include(b => b.BookingAmenities).ThenInclude(b => b.Amenity)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (Booking == null) return NotFound();

            // Verificare Securitate (OnGet)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Booking.UserId != userId && !User.IsInRole("Admin")) return Forbid();

            ViewData["FlightID"] = new SelectList(_context.Flight, "ID", "FlightNumber", Booking.FlightID);
            PopulateAssignedAmenityData(_context, Booking);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id, string[] selectedAmenities)
        {
            if (id == null) return NotFound();

            var bookingToUpdate = await _context.Booking
                .Include(i => i.Passengers)
                .Include(i => i.BookingAmenities).ThenInclude(i => i.Amenity)
                .FirstOrDefaultAsync(s => s.ID == id);

            if (bookingToUpdate == null) return NotFound();

            // Verificare Securitate (OnPost) - Previne modificarea prin unelte de tip Postman
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (bookingToUpdate.UserId != userId && !User.IsInRole("Admin")) return Forbid();

            // UPDATE LOGIC
            bookingToUpdate.FlightID = Booking.FlightID;
            bookingToUpdate.BookingDate = Booking.BookingDate;

            // Actualizăm datele de contact adăugate anterior
            bookingToUpdate.ContactEmail = Booking.ContactEmail;
            bookingToUpdate.ContactPhone = Booking.ContactPhone;

            UpdateBookingAmenities(_context, selectedAmenities, bookingToUpdate);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}