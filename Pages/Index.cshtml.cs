using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;

namespace Cailean_Razvan_Zboruri.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AviationContext _context;

        public IndexModel(AviationContext context)
        {
            _context = context;
        }

        public SelectList AirportsList { get; set; }
        public IList<Models.Flight> CheapFlights { get; set; }
        public IList<Models.Amenity> Amenities { get; set; }

        public async Task OnGetAsync()
        {
            // 1. Aeroporturi pentru formular (evaluare pe client pentru a evita eroarea de string format)
            var airportsFromDb = await _context.Airport.ToListAsync();
            var airports = airportsFromDb
                .Select(a => new { ID = a.ID, DisplayText = $"{a.IataCode} - {a.City} ({a.Country})" })
                .OrderBy(a => a.DisplayText).ToList();
            AirportsList = new SelectList(airports, "ID", "DisplayText");

            // 2. Oferte: Cele mai ieftine 3 zboruri (Includem aeroporturile pentru a afisa ruta)
            CheapFlights = await _context.Flight
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .OrderBy(f => f.BasePrice)
                .Take(3)
                .ToListAsync();

            // 3. Servicii suplimentare (Amenities)
            Amenities = await _context.Amenity.ToListAsync();
        }
    }
}