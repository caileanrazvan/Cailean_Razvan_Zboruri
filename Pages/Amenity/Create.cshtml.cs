using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;

namespace Cailean_Razvan_Zboruri.Pages.Amenity
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly AviationContext _context;
        private readonly IWebHostEnvironment _environment; // Pentru a accesa wwwroot

        public CreateModel(AviationContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult OnGet() { return Page(); }

        [BindProperty]
        public Models.Amenity Amenity { get; set; }

        [BindProperty]
        public IFormFile? UploadedImage { get; set; } // Proprietate pentru fișier

        public async Task<IActionResult> OnPostAsync()
        {
            if (UploadedImage != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(UploadedImage.FileName);
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "amenities");

                Directory.CreateDirectory(uploadsFolder); // Creează folderul dacă nu există
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await UploadedImage.CopyToAsync(stream);
                }

                Amenity.ImageUrl = "/uploads/amenities/" + fileName;
            }

            _context.Amenity.Add(Amenity);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}