using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cailean_Razvan_Zboruri.Pages.Flight
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly Cailean_Razvan_Zboruri.Data.AviationContext _context;

        public CreateModel(Cailean_Razvan_Zboruri.Data.AviationContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["DepartureAirportID"] = new SelectList(_context.Airport, "ID", "IataCode");
        ViewData["ArrivalAirportID"] = new SelectList(_context.Airport, "ID", "IataCode");
            return Page();
        }

        [BindProperty]
        public Cailean_Razvan_Zboruri.Models.Flight Flight { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            // VALIDARE: Verificăm dacă data este în trecut
            if (Flight.DepartureTime < DateTime.Now)
            {
                ModelState.AddModelError("Flight.DepartureTime", "Nu poți crea un zbor într-o dată din trecut.");
            }

            if (!ModelState.IsValid)
            {
                // Re-populăm listele pentru dropdown-uri dacă au apărut erori
                ViewData["DepartureAirportID"] = new SelectList(_context.Airport, "ID", "Name");
                ViewData["ArrivalAirportID"] = new SelectList(_context.Airport, "ID", "Name");
                return Page();
            }

            _context.Flight.Add(Flight);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
