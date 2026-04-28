using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;

namespace Cailean_Razvan_Zboruri.Pages.Contact
{
    public class IndexModel : PageModel
    {
        private readonly AviationContext _context;

        public IndexModel(AviationContext context)
        {
            _context = context;
        }

        public ContactInfo ContactInfo { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Căutăm prima înregistrare de contact
            ContactInfo = await _context.ContactInfo.FirstOrDefaultAsync();

            // Dacă nu există în baza de date, generăm noi una implicită
            if (ContactInfo == null)
            {
                ContactInfo = new ContactInfo
                {
                    Email = "suport@angelairlines.com",
                    Phone = "+40 700 000 000",
                    Address = "Strada Aviației Nr. 1, Cluj-Napoca, România",
                    WorkingHours = "Luni - Vineri: 08:00 - 20:00",
                    Description = "Suntem aici pentru a vă oferi cea mai bună experiență de zbor. Nu ezitați să ne contactați pentru orice întrebare!"
                };

                _context.ContactInfo.Add(ContactInfo);
                await _context.SaveChangesAsync();
            }

            return Page();
        }
    }
}