using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;
using Microsoft.AspNetCore.Authorization;

namespace Cailean_Razvan_Zboruri.Pages.Airport
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly Cailean_Razvan_Zboruri.Data.AviationContext _context;

        public IndexModel(Cailean_Razvan_Zboruri.Data.AviationContext context)
        {
            _context = context;
        }

        public IList<Cailean_Razvan_Zboruri.Models.Airport> Airport { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Airport = await _context.Airport.ToListAsync();
        }
        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            var airport = await _context.Airport.FindAsync(id);
            if (airport != null)
            {
                // 1. Facem aeroportul inactiv
                airport.IsActive = false;

                // 2. Anulăm toate zborurile legate de el
                var flights = await _context.Flight
                    .Where(f => f.DepartureAirportID == id || f.ArrivalAirportID == id)
                    .ToListAsync();

                var flightIds = flights.Select(f => f.ID).ToList();
                foreach (var f in flights) f.IsCancelled = true;

                // 3. Anulăm rezervările acelor zboruri
                var bookings = await _context.Booking
                    .Where(b => b.FlightID.HasValue && flightIds.Contains(b.FlightID.Value))
                    .ToListAsync();

                foreach (var b in bookings) b.PaymentStatus = "Anulat";

                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./Index");
        }
    }
}
