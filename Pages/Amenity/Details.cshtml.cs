using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;

namespace Cailean_Razvan_Zboruri.Pages.Amenity
{
    public class DetailsModel : PageModel
    {
        private readonly AviationContext _context;

        public DetailsModel(AviationContext context)
        {
            _context = context;
        }

        public Models.Amenity Amenity { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            // Căutăm serviciul în baza de date pe baza ID-ului
            Amenity = await _context.Amenity.FirstOrDefaultAsync(m => m.ID == id);

            if (Amenity == null) return NotFound();

            return Page();
        }
    }
}