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
            if (id == null)
            {
                return NotFound();
            }

            // Găsim zborul pe care vrem să îl ștergem
            var flightToDelete = await _context.Flight.FindAsync(id);

            if (flightToDelete != null)
            {
                _context.Flight.Remove(flightToDelete);
                await _context.SaveChangesAsync();
            }

            // După ștergere, ne întoarcem la lista de zboruri
            return RedirectToPage("./Index");
        }
    }
}