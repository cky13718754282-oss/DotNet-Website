using Geekspace.Data;
using Geekspace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Geekspace.Controllers
{
    [Authorize]
    public class LearningProgressController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public LearningProgressController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleSaved(int resourceId)
        {
            var progress = await GetOrCreateProgressAsync(resourceId);
            if (progress == null)
            {
                return NotFound();
            }

            progress.IsSaved = !progress.IsSaved;
            progress.LastUpdated = DateTime.Now;
            await SaveOrRemoveAsync(progress);

            TempData["SuccessMessage"] = progress.IsSaved
                ? "Resource saved to My Activity."
                : "Resource removed from saved items.";

            return RedirectToAction("Details", "Resource", new { id = resourceId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleCompleted(int resourceId)
        {
            var progress = await GetOrCreateProgressAsync(resourceId);
            if (progress == null)
            {
                return NotFound();
            }

            progress.IsCompleted = !progress.IsCompleted;
            progress.LastUpdated = DateTime.Now;
            await SaveOrRemoveAsync(progress);

            TempData["SuccessMessage"] = progress.IsCompleted
                ? "Resource marked as completed."
                : "Resource moved back to in progress.";

            return RedirectToAction("Details", "Resource", new { id = resourceId });
        }

        private async Task<UserLearningProgress?> GetOrCreateProgressAsync(int resourceId)
        {
            var resourceExists = await _context.LearningResources.AnyAsync(resource =>
                resource.Id == resourceId &&
                (resource.IsPublished || User.IsInRole("Admin") || User.IsInRole("Root")));

            if (!resourceExists)
            {
                return null;
            }

            var userId = _userManager.GetUserId(User)!;
            var progress = await _context.UserLearningProgresses
                .SingleOrDefaultAsync(item =>
                    item.UserId == userId &&
                    item.LearningResourceId == resourceId);

            if (progress != null)
            {
                return progress;
            }

            progress = new UserLearningProgress
            {
                UserId = userId,
                LearningResourceId = resourceId
            };
            _context.UserLearningProgresses.Add(progress);
            return progress;
        }

        private async Task SaveOrRemoveAsync(UserLearningProgress progress)
        {
            if (!progress.IsSaved && !progress.IsCompleted)
            {
                _context.UserLearningProgresses.Remove(progress);
            }

            await _context.SaveChangesAsync();
        }
    }
}
