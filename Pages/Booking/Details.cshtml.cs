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

            var booking = await _context.Booking.FirstOrDefaultAsync(m => m.ID == id);

            if (booking == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Dacă rezervarea nu îi aparține și nu este Admin, îi dăm "Access Denied" (Forbid)
            if (booking.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            Booking = booking;
            return Page();
        }
    }
}