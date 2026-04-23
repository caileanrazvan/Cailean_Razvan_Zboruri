using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;

namespace Cailean_Razvan_Zboruri.Pages.Flight
{
    public class IndexModel : PageModel
    {
        private readonly AviationContext _context;

        public IndexModel(AviationContext context)
        {
            _context = context;
        }

        public IList<Models.Flight> Flight { get; set; } = default!;

        // Proprietăți ajutătoare pentru interfață
        public string DepartureCityName { get; set; } = "Oriunde";
        public string ArrivalCityName { get; set; } = "Oriunde";
        public DateTime? SearchDate { get; set; }
        public int Passengers { get; set; }

        public async Task OnGetAsync(int? departureId, int? arrivalId, DateTime? flightDate, int passengers = 1)
        {
            // Salvăm datele pentru a le afișa în header-ul paginii de rezultate
            SearchDate = flightDate;
            Passengers = passengers < 1 ? 1 : passengers;

            // 1. Definim interogarea de bază (Aducem toate zborurile)
            IQueryable<Models.Flight> query = _context.Flight
                .Include(f => f.ArrivalAirport)
                .Include(f => f.DepartureAirport);

            // 2. Aplicăm filtrul pentru PLECARE doar dacă s-a selectat un oraș
            if (departureId.HasValue)
            {
                query = query.Where(f => f.DepartureAirport.ID == departureId.Value);

                // Căutăm numele pentru design
                var depAirport = await _context.Airport.FindAsync(departureId.Value);
                if (depAirport != null) DepartureCityName = depAirport.City;
            }

            // 3. Aplicăm filtrul pentru SOSIRE doar dacă s-a selectat un oraș
            if (arrivalId.HasValue)
            {
                query = query.Where(f => f.ArrivalAirport.ID == arrivalId.Value);

                // Căutăm numele pentru design
                var arrAirport = await _context.Airport.FindAsync(arrivalId.Value);
                if (arrAirport != null) ArrivalCityName = arrAirport.City;
            }

            // 4. Aplicăm filtrul pentru DATĂ doar dacă a fost aleasă una
            if (flightDate.HasValue)
            {
                query = query.Where(f => f.DepartureTime.Date == flightDate.Value.Date);
            }

            // 5. La final, extragem rezultatele filtrate din baza de date
            Flight = await query.OrderBy(f => f.DepartureTime).ToListAsync();
        }
    }
}