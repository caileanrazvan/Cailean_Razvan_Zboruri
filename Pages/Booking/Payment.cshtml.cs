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
                .FirstOrDefaultAsync(m => m.ID == bookingId);

            if (Booking == null) return NotFound();
            if (Booking.PaymentStatus == "Paid") return RedirectToPage("./Index");

            int passengersCount = Booking.Passengers?.Count ?? 1;
            TotalAmount = (Booking.Flight?.BasePrice ?? 0) * passengersCount;

            // 1. Pregătim cheia publică pentru frontend
            StripePublishableKey = _configuration["Stripe:PublishableKey"];

            // 2. Creăm intenția de plată (Payment Intent) pe serverele Stripe
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(TotalAmount * 100), // Stripe cere suma în cenți (ex: 50 EUR = 5000 cenți)
                Currency = "eur",
                Metadata = new Dictionary<string, string>
                {
                    { "BookingId", Booking.ID.ToString() }
                }
            };

            var service = new PaymentIntentService();
            var intent = service.Create(options);

            // 3. Trimitem secretul către frontend pentru a autoriza tranzacția
            ClientSecret = intent.ClientSecret;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int bookingId, string paymentIntentId)
        {
            // Când ajungem aici, Stripe a confirmat deja că banii au fost luați.
            var bookingToUpdate = await _context.Booking.FindAsync(bookingId);
            if (bookingToUpdate == null) return NotFound();

            bookingToUpdate.PaymentStatus = "Paid";

            // Generăm un cod de referință premium (ex: ANGEL-A8F2B9)
            string generatedCode = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
            bookingToUpdate.TransactionId = $"ANGEL-{generatedCode}";

            await _context.SaveChangesAsync();

            // Redirecționăm către pagina BookingSummary și îi pasăm ID-ul rezervării
            return RedirectToPage("./BookingSummary", new { bookingId = bookingId });
            // Notă: Dacă în pagina ta parametrul se numește altfel (ex: bookingId), 
            // modifică mai sus în consecință: new { bookingId = bookingId }
        }
    }
}