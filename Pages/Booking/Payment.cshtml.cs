using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;
using Stripe;

namespace Cailean_Razvan_Zboruri.Pages.Booking
{
    public class PaymentModel : PageModel
    {
        private readonly AviationContext _context;
        private readonly IConfiguration _configuration;

        public PaymentModel(AviationContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public Models.Booking Booking { get; set; }
        public decimal TotalAmount { get; set; }

        // Proprietăți pentru Stripe
        public string ClientSecret { get; set; }
        public string StripePublishableKey { get; set; }

        public async Task<IActionResult> OnGetAsync(int? bookingId)
        {
            if (bookingId == null) return NotFound();

            Booking = await _context.Booking
                .Include(b => b.Flight).ThenInclude(f => f.DepartureAirport)
                .Include(b => b.Flight).ThenInclude(f => f.ArrivalAirport)
                .Include(b => b.Passengers)
                    .ThenInclude(p => p.Amenities) // NOU: Includem și serviciile extra alese de pasageri!
                .FirstOrDefaultAsync(m => m.ID == bookingId);

            if (Booking == null) return NotFound();
            if (Booking.PaymentStatus == "Paid") return RedirectToPage("./BookingSummary", new { bookingId = Booking.ID });

            // CALCULUL CORECT AL TOTALULUI (Bilete + Servicii Extra)
            int passengersCount = Booking.Passengers?.Count ?? 1;

            // 1. Prețul de bază pentru toți pasagerii
            decimal basePriceTotal = (Booking.Flight?.BasePrice ?? 0) * passengersCount;

            // 2. Adunăm prețul tuturor serviciilor extra (Amenities)
            decimal amenitiesTotal = Booking.Passengers?
                .SelectMany(p => p.Amenities ?? new List<Models.Amenity>())
                .Sum(a => a.Price) ?? 0;

            TotalAmount = basePriceTotal + amenitiesTotal;

            // 1. Pregătim cheia publică pentru frontend
            StripePublishableKey = _configuration["Stripe:PublishableKey"];

            // 2. Creăm intenția de plată (Payment Intent) pe serverele Stripe
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(TotalAmount * 100), // Stripe cere suma în cenți
                Currency = "eur",
                Metadata = new Dictionary<string, string>
                {
                    { "BookingId", Booking.ID.ToString() }
                }
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);

            // 3. Trimitem secretul către frontend pentru a autoriza tranzacția
            ClientSecret = intent.ClientSecret;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int bookingId, string paymentIntentId)
        {
            if (string.IsNullOrEmpty(paymentIntentId))
            {
                return BadRequest("Eroare de securitate: ID-ul plății lipsește.");
            }

            // SECURITATE PRO: Verificăm direct la Stripe stadiul plății!
            var service = new PaymentIntentService();
            var intent = await service.GetAsync(paymentIntentId);

            // Dacă hackerul a simulat apelul POST, dar plata la Stripe nu este completă, îl oprim.
            if (intent.Status != "succeeded")
            {
                return BadRequest("Eroare: Plata nu a fost confirmată de bancă. Vă rugăm să încercați din nou.");
            }

            // Când ajungem aici, știm 100% sigur că banii au intrat în cont.
            var bookingToUpdate = await _context.Booking.FindAsync(bookingId);
            if (bookingToUpdate == null) return NotFound();

            // Ne asigurăm că nu emitem două coduri dacă din greșeală s-a făcut POST de două ori
            if (bookingToUpdate.PaymentStatus == "Paid")
            {
                return RedirectToPage("./BookingSummary", new { bookingId = bookingId });
            }

            bookingToUpdate.PaymentStatus = "Paid";

            // Generăm un cod de referință premium (ex: ANGEL-A8F2B9)
            string generatedCode = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
            bookingToUpdate.TransactionId = $"ANGEL-{generatedCode}";

            await _context.SaveChangesAsync();

            return RedirectToPage("./BookingSummary", new { bookingId = bookingId });
        }
    }
}