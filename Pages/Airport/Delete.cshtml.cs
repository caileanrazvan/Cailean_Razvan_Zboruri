using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Cailean_Razvan_Zboruri.Pages.Airport
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly Cailean_Razvan_Zboruri.Data.AviationContext _context;

        public DeleteModel(Cailean_Razvan_Zboruri.Data.AviationContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cailean_Razvan_Zboruri.Models.Airport Airport { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airport = await _context.Airport.FirstOrDefaultAsync(m => m.ID == id);

            if (airport == null)
            {
                return NotFound();
            }
            else
            {
                Airport = airport;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            var airport = await _context.Airport.FindAsync(id);
            if (airport != null)
            {
                // 1. Zborurile afectate
                var flights = await _context.Flight
                    .Where(f => f.DepartureAirportID == id || f.ArrivalAirportID == id)
                    .ToListAsync();

                var flightIds = flights.Select(f => f.ID).ToList();

                // 2. Rezervările zborurilor
                var bookings = await _context.Booking
                    .Include(b => b.Passengers)
                    .Where(b => b.FlightID.HasValue && flightIds.Contains(b.FlightID.Value))
                    .ToListAsync();

                // 3. Ștergem tot în cascadă
                _context.Booking.RemoveRange(bookings);
                _context.Flight.RemoveRange(flights);
                _context.Airport.Remove(airport);

                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./Index");
        }
    }
}
