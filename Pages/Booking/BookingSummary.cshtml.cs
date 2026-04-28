using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;

namespace Cailean_Razvan_Zboruri.Pages.Booking
{
    [Authorize]
    public class BookingSummaryModel : PageModel
    {
        private readonly AviationContext _context;

        public BookingSummaryModel(AviationContext context)
        {
            _context = context;
        }

        public Models.Booking Booking { get; set; }
        public decimal TotalPrice { get; set; }

        public async Task<IActionResult> OnGetAsync(int bookingId)
        {
            Booking = await _context.Booking
                .Include(b => b.Flight).ThenInclude(f => f.DepartureAirport)
                .Include(b => b.Flight).ThenInclude(f => f.ArrivalAirport)
                .Include(b => b.Passengers).ThenInclude(p => p.Amenities)
                .FirstOrDefaultAsync(m => m.ID == bookingId);

            if (Booking == null) return NotFound();

            // Calculăm totalul real al tranzacției
            TotalPrice = (Booking.Flight.BasePrice * Booking.Passengers.Count) +
                         Booking.Passengers.SelectMany(p => p.Amenities).Sum(a => a.Price);

            return Page();
        }
    }
}