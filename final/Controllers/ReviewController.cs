using final.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace final.Controllers
{
    public class ReviewController : Controller
    {
        private readonly CampingDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewController(CampingDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyReviews()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            // Get all reviews by this user, including camp information
            var reviews = await _context.Reviews
                .Include(r => r.Camp)
                .Where(r => r.UserId == user.Id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(reviews);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(ReviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }

                var camp = await _context.Camps.FindAsync(model.CampId);
                if (camp == null)
                {
                    return NotFound();
                }

                var review = new Review
                {
                    Content = model.Content,
                    Rating = model.Rating,
                    CreatedAt = DateTime.Now,
                    UserId = user.Id,
                    CampId = model.CampId
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Camp", new { id = model.CampId });
            }

            // If we got this far, something failed, redisplay form
            return RedirectToAction("Details", "Camp", new { id = model.CampId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            // Only allow users to delete their own reviews or admins to delete any review
            if (review.UserId != user.Id && !User.IsInRole("Admin"))
            {
                return Unauthorized();
            }

            int campId = review.CampId;
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Camp", new { id = campId });
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            // Only allow users to edit their own reviews or admins to edit any review
            if (review.UserId != user.Id && !User.IsInRole("Admin"))
            {
                return Unauthorized();
            }

            var viewModel = new ReviewViewModel
            {
                CampId = review.CampId,
                Content = review.Content,
                Rating = review.Rating
            };

            ViewBag.ReviewId = id;
            ViewBag.CampName = (await _context.Camps.FindAsync(review.CampId))?.Name;

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int id, ReviewViewModel model)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            // Only allow users to edit their own reviews or admins to edit any review
            if (review.UserId != user.Id && !User.IsInRole("Admin"))
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                review.Content = model.Content;
                review.Rating = model.Rating;
                // Optionally update the timestamp to show it was edited
                // review.CreatedAt = DateTime.Now;

                _context.Update(review);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Camp", new { id = review.CampId });
            }

            ViewBag.ReviewId = id;
            ViewBag.CampName = (await _context.Camps.FindAsync(review.CampId))?.Name;

            return View(model);
        }
    }
}
