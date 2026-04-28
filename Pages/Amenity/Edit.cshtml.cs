using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;

namespace Cailean_Razvan_Zboruri.Pages.Amenity
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly AviationContext _context;
        private readonly IWebHostEnvironment _environment;

        public EditModel(AviationContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Models.Amenity Amenity { get; set; }

        [BindProperty]
        public IFormFile? UploadedImage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();
            Amenity = await _context.Amenity.FirstOrDefaultAsync(m => m.ID == id);
            if (Amenity == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var amenityToUpdate = await _context.Amenity.FindAsync(Amenity.ID);
            if (amenityToUpdate == null) return NotFound();

            amenityToUpdate.Name = Amenity.Name;
            amenityToUpdate.Price = Amenity.Price;
            amenityToUpdate.Description = Amenity.Description;

            if (UploadedImage != null)
            {
                // Ștergem imaginea veche dacă există
                if (!string.IsNullOrEmpty(amenityToUpdate.ImageUrl))
                {
                    var oldFilePath = Path.Combine(_environment.WebRootPath, amenityToUpdate.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);
                }

                // Salvăm imaginea nouă
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(UploadedImage.FileName);
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "amenities");
                Directory.CreateDirectory(uploadsFolder);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await UploadedImage.CopyToAsync(stream);
                }

                amenityToUpdate.ImageUrl = "/uploads/amenities/" + fileName;
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}