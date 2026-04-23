using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;
using System.Security.Claims;

namespace Cailean_Razvan_Zboruri.Pages.Booking
{
    public class DetailsModel : PageModel
    {
        private readonly AviationContext _context;
        public DetailsModel(AviationContext context) { _context = context; }

        public Models.Booking Booking { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Booking
                .Include(b => b.Flight)
                .Include(b => b.Passengers)
                .Include(b => b.BookingAmenities).ThenInclude(b => b.Amenity)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (booking == null) return NotFound();
            Booking = booking;

            // --- VERIFICARE SECURITATE ---
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isAdmin = User.IsInRole("Admin");

            if (Booking.UserId != userId && !isAdmin)
            {
                return Forbid(); // Interzice accesul dacă nu e proprietar sau admin
            }
            // -----------------------------

            return Page();
        }
    }
}