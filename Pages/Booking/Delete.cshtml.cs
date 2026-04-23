using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;
using System.Security.Claims;

namespace Cailean_Razvan_Zboruri.Pages.Booking
{
    public class DeleteModel : PageModel
    {
        private readonly AviationContext _context;
        public DeleteModel(AviationContext context) { _context = context; }

        [BindProperty]
        public Models.Booking Booking { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Booking = await _context.Booking
                .Include(b => b.Passengers)
                .Include(b => b.Flight)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (Booking == null) return NotFound();

            // Verificare Securitate
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Booking.UserId != userId && !User.IsInRole("Admin")) return Forbid();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            var bookingToDelete = await _context.Booking.FindAsync(id);

            if (bookingToDelete != null)
            {
                // Verificare Securitate la nivel de POST
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (bookingToDelete.UserId != userId && !User.IsInRole("Admin")) return Forbid();

                _context.Booking.Remove(bookingToDelete);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}