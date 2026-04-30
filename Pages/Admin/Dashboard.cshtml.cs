using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cailean_Razvan_Zboruri.Data;

namespace Cailean_Razvan_Zboruri.Pages.Admin
{
    [Authorize(Roles = "Admin")] // Doar șeful are acces!
    public class DashboardModel : PageModel
    {
        private readonly AviationContext _context;

        public DashboardModel(AviationContext context)
        {
            _context = context;
        }

        public decimal TotalRevenue { get; set; }
        public int ActiveFlights { get; set; }
        public int PassengersThisMonth { get; set; }
        public string PopularDestination { get; set; }

        // Listele care vor fi trimise către Chart.js în HTML
        public List<string> ChartLabels { get; set; } = new List<string>();
        public List<int> ChartData { get; set; } = new List<int>();

        public async Task OnGetAsync()
        {
            // 1. Zboruri Active (programate în viitor)
            ActiveFlights = await _context.Flight.CountAsync(f => f.DepartureTime > DateTime.Now);

            // 2. Extragem rezervările plătite de luna aceasta pentru calcule
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var thisMonthBookings = await _context.Booking
                .Include(b => b.Passengers)
                .Include(b => b.Flight)
                .Where(b => b.BookingDate.Month == currentMonth &&
                            b.BookingDate.Year == currentYear &&
                            b.PaymentStatus == "Paid")
                .ToListAsync();

            // Pasageri luna aceasta
            PassengersThisMonth = thisMonthBookings.Sum(b => b.Passengers?.Count ?? 0);

            // Venituri Totale (Preț Bază Zbor * Număr Pasageri)
            TotalRevenue = thisMonthBookings.Sum(b => (b.Flight?.BasePrice ?? 0) * (b.Passengers?.Count ?? 0));

            // 3. Destinația cea mai populară (din toate rezervările)
            var popularFlight = await _context.Booking
                .Include(b => b.Flight)
                .ThenInclude(f => f.ArrivalAirport)
                .Where(b => b.Flight != null && b.Flight.ArrivalAirport != null)
                .GroupBy(b => b.Flight.ArrivalAirport.City)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync();

            PopularDestination = popularFlight ?? "Nedeterminat";

            // 4. Date pentru Grafic (Numărul de rezervări din ultimele 7 zile)
            for (int i = 6; i >= 0; i--)
            {
                var targetDate = DateTime.Now.Date.AddDays(-i);
                ChartLabels.Add(targetDate.ToString("dd MMM")); // Ex: 24 Mai

                var dailyBookings = await _context.Booking
                    .Where(b => b.BookingDate.Date == targetDate)
                    .CountAsync();

                ChartData.Add(dailyBookings);
            }
        }
    }
}