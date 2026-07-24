using Geekspace.Data;
using Geekspace.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Geekspace.Controllers
{
    [Authorize(Roles = "Admin,Root")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var recentComments = await _context.ResourceComments
                .Include(comment => comment.LearningResource)
                .OrderByDescending(comment => comment.PostedDate)
                .Take(5)
                .ToListAsync();

            var authorIds = recentComments.Select(comment => comment.UserId).Distinct().ToList();
            var authorNames = await _context.Users
                .Where(user => authorIds.Contains(user.Id))
                .ToDictionaryAsync(
                    user => user.Id,
                    user => user.UserName ?? "Unknown");

            var model = new AdminDashboardViewModel
            {
                ResourceCount = await _context.LearningResources.CountAsync(),
                PublishedResourceCount = await _context.LearningResources.CountAsync(resource => resource.IsPublished),
                CategoryCount = await _context.Categories.CountAsync(),
                UserCount = await _context.Users.CountAsync(),
                CommentCount = await _context.ResourceComments.CountAsync(),
                SavedCount = await _context.UserLearningProgresses.CountAsync(progress => progress.IsSaved),
                CompletedCount = await _context.UserLearningProgresses.CountAsync(progress => progress.IsCompleted),
                RecentResources = await _context.LearningResources
                    .Include(resource => resource.Category)
                    .OrderByDescending(resource => resource.CreatedDate)
                    .Take(5)
                    .ToListAsync(),
                RecentComments = recentComments,
                CommentAuthors = authorNames
            };

            return View(model);
        }
    }
}
