using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Geekspace.Data;
using Geekspace.ViewModels;

namespace Geekspace.Controllers
{
    // Deletion permissions are derived from ownership and role.
    [Authorize]
    public class ActivityController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ActivityController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User);
            var isRoot = User.IsInRole("Root");
            var isAdmin = User.IsInRole("Admin");

            IQueryable<Geekspace.Models.ResourceComment> query = _context.ResourceComments
                .Include(c => c.LearningResource);

            if (!isRoot && !isAdmin)
            {
                query = query.Where(c => c.UserId == currentUserId);
            }

            var comments = await query.OrderByDescending(c => c.PostedDate).ToListAsync();

            var userIds = comments.Select(c => c.UserId).Distinct().ToList();
            var usersDict = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.UserName ?? "Unknown");

            var canDeleteDict = new System.Collections.Generic.Dictionary<int, bool>();
            foreach (var comment in comments)
            {
                bool canDelete = false;
                bool isOwner = comment.UserId == currentUserId;
                
                if (isOwner)
                {
                    // Everyone can delete their own comments
                    canDelete = true;
                }
                else if (isRoot)
                {
                    // Root can delete any comment
                    canDelete = true;
                }
                else if (isAdmin)
                {
                    // Admin can delete user and admin comments, but not root comments
                    if (usersDict.ContainsKey(comment.UserId))
                    {
                        var author = await _userManager.FindByIdAsync(comment.UserId);
                        if (author != null && !await _userManager.IsInRoleAsync(author, "Root"))
                        {
                            canDelete = true;
                        }
                    }
                }
                canDeleteDict[comment.Id] = canDelete;
            }

            ViewBag.UserNames = usersDict;
            ViewBag.CanDeleteDict = canDeleteDict;
            ViewBag.CurrentUserId = currentUserId;
            ViewBag.IsRoot = isRoot;
            ViewBag.IsAdmin = isAdmin;

            var learningItems = await _context.UserLearningProgresses
                .Include(item => item.LearningResource)
                    .ThenInclude(resource => resource!.Category)
                .Where(item => item.UserId == currentUserId)
                .OrderByDescending(item => item.LastUpdated)
                .ToListAsync();

            return View(new ActivityIndexViewModel
            {
                Comments = comments,
                LearningItems = learningItems
            });
        }
    }
}
