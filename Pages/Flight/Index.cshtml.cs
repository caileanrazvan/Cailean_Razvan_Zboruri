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

        // Proprietăți pentru a reține ID-urile căutate (utile pentru paginare în HTML)
        public int? CurrentDepartureId { get; set; }
        public int? CurrentArrivalId { get; set; }

        // PARAMETRI NOI PENTRU SORTARE ȘI PAGINARE
        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public const int PageSize = 5; // Câte zboruri arătăm pe o pagină

        public async Task OnGetAsync(int? departureId, int? arrivalId, DateTime? flightDate, int passengers = 1, int pageIndex = 1)
        {
            // Preluăm starea curentă pentru paginare
            CurrentPage = pageIndex;
            SearchDate = flightDate;
            Passengers = passengers < 1 ? 1 : passengers;
            CurrentDepartureId = departureId;
            CurrentArrivalId = arrivalId;

            // 1. Definim interogarea de bază UNICĂ (Aducem toate zborurile)
            var query = _context.Flight
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .AsQueryable();

            // REGULĂ: Dacă NU este Admin, arătăm doar zborurile din viitor
            if (!User.IsInRole("Admin"))
            {
                query = query.Where(f => f.DepartureTime >= DateTime.Now);
            }

            // 2. Aplicăm filtrul pentru PLECARE
            if (departureId.HasValue)
            {
                query = query.Where(f => f.DepartureAirport.ID == departureId.Value);

                var depAirport = await _context.Airport.FindAsync(departureId.Value);
                if (depAirport != null) DepartureCityName = depAirport.City;
            }

            // 3. Aplicăm filtrul pentru SOSIRE
            if (arrivalId.HasValue)
            {
                query = query.Where(f => f.ArrivalAirport.ID == arrivalId.Value);

                var arrAirport = await _context.Airport.FindAsync(arrivalId.Value);
                if (arrAirport != null) ArrivalCityName = arrAirport.City;
            }

            // 4. Aplicăm filtrul pentru DATĂ
            if (flightDate.HasValue)
            {
                query = query.Where(f => f.DepartureTime.Date == flightDate.Value.Date);
            }

            // 5. SORTARE
            switch (SortOrder)
            {
                case "price_asc":
                    query = query.OrderBy(f => f.BasePrice);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(f => f.BasePrice);
                    break;
                case "date_desc":
                    query = query.OrderByDescending(f => f.DepartureTime);
                    break;
                default: // "date_asc" (implicit)
                    query = query.OrderBy(f => f.DepartureTime);
                    break;
            }

            // 6. PAGINARE
            var count = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(count / (double)PageSize);

            // Ne asigurăm că pagina cerută este validă
            if (CurrentPage > TotalPages && TotalPages > 0) CurrentPage = TotalPages;
            if (CurrentPage < 1) CurrentPage = 1;

            // 7. La final, extragem rezultatele filtrate, sortate și paginate din baza de date
            Flight = await query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            var flight = await _context.Flight.FindAsync(id);
            if (flight != null)
            {
                // Marcăm zborul ca anulat
                flight.IsCancelled = true;

                // Căutăm rezervările și le trecem pe "Anulat"
                var bookings = await _context.Booking
                    .Where(b => b.FlightID == id)
                    .ToListAsync();

                foreach (var booking in bookings)
                {
                    booking.PaymentStatus = "Anulat";
                }

                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./Index");
        }
    }
}