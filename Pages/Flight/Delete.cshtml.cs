using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;

namespace Cailean_Razvan_Zboruri.Pages.Flight
{
    // Blocăm accesul pentru utilizatorii normali, doar Adminul are voie să șteargă
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly AviationContext _context;

        public DeleteModel(AviationContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.Flight Flight { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // REZOLVAREA ESTE AICI:
            // Folosim .Include() pentru a forța încărcarea datelor despre aeroporturi
            Flight = await _context.Flight
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (Flight == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            var flight = await _context.Flight.FindAsync(id);
            if (flight != null)
            {
                // 1. Găsim și ștergem definitiv toate rezervările acestui zbor
                var bookings = await _context.Booking
                    .Include(b => b.Passengers)
                    .Where(b => b.FlightID == id)
                    .ToListAsync();

                _context.Booking.RemoveRange(bookings);

                // 2. Ștergem zborul
                _context.Flight.Remove(flight);

                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./Index");
        }
    }
}