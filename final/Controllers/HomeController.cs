using final.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace final.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CampingDbContext _context;

        public HomeController(ILogger<HomeController> logger, CampingDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm)
        {
            IQueryable<Camp> campsQuery = _context.Camps
                .Include(c => c.Reviews);

            // Apply search filter if searchTerm is provided
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                campsQuery = campsQuery.Where(c =>
                    EF.Functions.Like(c.Name, $"%{searchTerm}%"));

                ViewData["SearchTerm"] = searchTerm;
            }

            var camps = await campsQuery.ToListAsync();
            return View(camps);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
