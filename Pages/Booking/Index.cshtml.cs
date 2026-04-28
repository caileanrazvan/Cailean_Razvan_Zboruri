using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;

namespace Cailean_Razvan_Zboruri.Pages.Bookings
{
    [Authorize] // Obligă utilizatorul să fie logat pentru a accesa pagina
    public class IndexModel : PageModel
    {
        private readonly AviationContext _context;

        public IndexModel(AviationContext context)
        {
            _context = context;
        }

        // Inițializăm lista goală pentru a preveni erorile de tip 'source is null'
        public IList<Models.Booking> Booking { get; set; } = new List<Models.Booking>();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            // Construim interogarea aducând TOATE datele necesare
            var query = _context.Booking
                .Include(b => b.Flight).ThenInclude(f => f.DepartureAirport)
                .Include(b => b.Flight).ThenInclude(f => f.ArrivalAirport)
                .Include(b => b.Passengers)
                .AsQueryable();

            // Filtrăm dacă nu este admin
            if (!isAdmin)
            {
                query = query.Where(b => b.UserId == userId);
            }

            // Aducem rezultatele ordonate descrescător după dată
            Booking = await query.OrderByDescending(b => b.BookingDate).ToListAsync();
        }
    }
}