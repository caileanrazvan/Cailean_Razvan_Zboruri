using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;

namespace Cailean_Razvan_Zboruri.Pages.Bookings
{
    [Authorize] // Doar utilizatorii logați pot anula
    public class DeleteModel : PageModel
    {
        private readonly AviationContext _context;

        public DeleteModel(AviationContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.Booking Booking { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            // Încărcăm rezervarea cu toate datele necesare pentru a le afișa pe ecran
            Booking = await _context.Booking
                .Include(b => b.Flight).ThenInclude(f => f.DepartureAirport)
                .Include(b => b.Flight).ThenInclude(f => f.ArrivalAirport)
                .Include(b => b.Passengers)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (Booking == null) return NotFound();

            // Verificare de securitate: Nu lăsăm pe cineva să șteargă rezervarea altcuiva
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Booking.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            var bookingToDelete = await _context.Booking.FindAsync(id);

            if (bookingToDelete != null)
            {
                // Securitate și la POST
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (bookingToDelete.UserId != userId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                _context.Booking.Remove(bookingToDelete);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}