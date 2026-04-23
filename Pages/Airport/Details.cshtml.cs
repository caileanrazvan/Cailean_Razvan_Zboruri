using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;
using Microsoft.AspNetCore.Authorization;

namespace Cailean_Razvan_Zboruri.Pages.Airport
{
    [Authorize(Roles = "Admin")]
    public class DetailsModel : PageModel
    {
        private readonly Cailean_Razvan_Zboruri.Data.AviationContext _context;

        public DetailsModel(Cailean_Razvan_Zboruri.Data.AviationContext context)
        {
            _context = context;
        }

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
    }
}
