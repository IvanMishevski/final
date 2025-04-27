using final.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace final.Controllers
{
    public class CampController : Controller
    {
        private readonly CampingDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CampController(CampingDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Camp camp)
        {
            ModelState.Remove("PhotoPath");
            ModelState.Remove("Reviews");
            if (ModelState.IsValid)
            {
                // Handle file upload if a photo was provided
                if (camp.PhotoUpload != null && camp.PhotoUpload.Length > 0)
                {
                    // Create uploads directory if it doesn't exist
                    var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsDir))
                    {
                        Directory.CreateDirectory(uploadsDir);
                    }

                    // Generate a unique filename
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(camp.PhotoUpload.FileName);
                    var filePath = Path.Combine(uploadsDir, fileName);

                    // Save the file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await camp.PhotoUpload.CopyToAsync(fileStream);
                    }

                    // Save the relative path to the database
                    camp.PhotoPath = "/uploads/" + fileName;
                    camp.Reviews = new List<Review>();
                }

                _context.Camps.Add(camp);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(camp);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var camp = await _context.Camps
                .Include(c => c.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (camp == null)
            {
                return NotFound();
            }

            return View(camp);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var camp = await _context.Camps.FindAsync(id);
            if (camp == null)
            {
                return NotFound();
            }
            return View(camp);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Camp camp)
        {
            if (id != camp.Id)
            {
                return NotFound();
            }
            ModelState.Remove("PhotoPath");
            ModelState.Remove("Reviews");
            if (ModelState.IsValid)
            {
                try
                {
                    // Handle file upload if a new photo was provided
                    if (camp.PhotoUpload != null && camp.PhotoUpload.Length > 0)
                    {
                        // Create uploads directory if it doesn't exist
                        var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads");
                        if (!Directory.Exists(uploadsDir))
                        {
                            Directory.CreateDirectory(uploadsDir);
                        }

                        // Generate a unique filename
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(camp.PhotoUpload.FileName);
                        var filePath = Path.Combine(uploadsDir, fileName);

                        // Save the file
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await camp.PhotoUpload.CopyToAsync(fileStream);
                        }

                        // Delete old photo if exists
                        if (!string.IsNullOrEmpty(camp.PhotoPath))
                        {
                            var oldFilePath = Path.Combine(_environment.WebRootPath, camp.PhotoPath.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Save the relative path to the database
                        camp.PhotoPath = "/uploads/" + fileName;
                    }

                    _context.Update(camp);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id = camp.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CampExists(camp.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(camp);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var camp = await _context.Camps
                .FirstOrDefaultAsync(m => m.Id == id);
            if (camp == null)
            {
                return NotFound();
            }

            return View(camp);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var camp = await _context.Camps.FindAsync(id);
            if (camp == null)
            {
                return NotFound();
            }

            // Delete photo file if exists
            if (!string.IsNullOrEmpty(camp.PhotoPath))
            {
                var filePath = Path.Combine(_environment.WebRootPath, camp.PhotoPath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.Camps.Remove(camp);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool CampExists(int id)
        {
            return _context.Camps.Any(e => e.Id == id);
        }
        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // If no search term is provided, return all camps
                var allCamps = await _context.Camps.ToListAsync();
                return View("SearchResults", allCamps);
            }

            // Search for camps where the name contains the search term (case insensitive)
            var camps = await _context.Camps
                .Where(c => c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();

            return View("SearchResults", camps);
        }
    }
}
