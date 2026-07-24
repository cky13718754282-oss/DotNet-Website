using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Geekspace.Data;
using Geekspace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Geekspace.Controllers
{
    public class ResourceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ResourceController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string? search, int? categoryId, ResourceType? type)
        {
            var resources = _context.LearningResources
                .Include(l => l.Category)
                .AsQueryable();

            if (!User.IsInRole("Admin") && !User.IsInRole("Root"))
            {
                resources = resources.Where(r => r.IsPublished);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                resources = resources.Where(r =>
                    r.Title.ToLower().Contains(term) ||
                    r.Description.ToLower().Contains(term) ||
                    (r.Content != null && r.Content.ToLower().Contains(term)) ||
                    (r.Category != null && r.Category.Name.ToLower().Contains(term)));
            }

            if (categoryId.HasValue)
            {
                resources = resources.Where(r => r.CategoryId == categoryId.Value);
            }

            if (type.HasValue)
            {
                resources = resources.Where(r => r.Type == type.Value);
            }

            ViewData["Search"] = search;
            ViewData["CategoryId"] = categoryId;
            ViewData["Type"] = type;
            ViewBag.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();

            return View(await resources.OrderByDescending(r => r.CreatedDate).ToListAsync());
        }


        [Authorize(Roles = "Admin,Root")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Root")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Content,Type,MediaUrl,CategoryId,CreatedDate,IsPublished")] LearningResource learningResource)
        {
            if (ModelState.IsValid)
            {
                _context.Add(learningResource);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Learning resource created successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", learningResource.CategoryId);
            return View(learningResource);
        }

        [Authorize(Roles = "Admin,Root")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learningResource = await _context.LearningResources.FindAsync(id);
            if (learningResource == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", learningResource.CategoryId);
            return View(learningResource);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Root")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Content,Type,MediaUrl,CategoryId,CreatedDate,IsPublished")] LearningResource learningResource)
        {
            if (id != learningResource.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(learningResource);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Learning resource updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LearningResourceExists(learningResource.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", learningResource.CategoryId);
            return View(learningResource);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learningResource = await _context.LearningResources
            .Include(l => l.Category)
            .Include(l => l.Comments)
            .FirstOrDefaultAsync(m => m.Id == id);
            if (learningResource == null)
            {
                return NotFound();
            }

            if (!learningResource.IsPublished &&
                !User.IsInRole("Admin") &&
                !User.IsInRole("Root"))
            {
                return NotFound();
            }

            // Build a lookup of user id -> display name so the view can show
            // who posted each comment without an extra navigation property.
            var userIds = learningResource.Comments.Select(c => c.UserId).Distinct().ToList();
            var authorNames = await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.UserName ?? "Unknown");
            ViewBag.CommentAuthors = authorNames;

            // Build a set of user IDs that belong to Root accounts, so the view
            // can hide the Delete button from Admins looking at a Root's comment
            // (the server-side check in CommentController is the real enforcement;
            // this just avoids showing a button that would be rejected anyway).
            var rootUserIds = new HashSet<string>();
            foreach (var uid in userIds)
            {
                var u = await _userManager.FindByIdAsync(uid);
                if (u != null && await _userManager.IsInRoleAsync(u, "Root"))
                {
                    rootUserIds.Add(uid);
                }
            }
            ViewBag.RootUserIds = rootUserIds;

            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId != null)
            {
                var progress = await _context.UserLearningProgresses
                    .AsNoTracking()
                    .SingleOrDefaultAsync(item =>
                        item.UserId == currentUserId &&
                        item.LearningResourceId == learningResource.Id);

                ViewBag.IsSaved = progress?.IsSaved ?? false;
                ViewBag.IsCompleted = progress?.IsCompleted ?? false;
            }

            return View(learningResource);
        }

        [Authorize(Roles = "Admin,Root")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learningResource = await _context.LearningResources
            .Include(l => l.Category)
            .FirstOrDefaultAsync(m => m.Id == id);
            if (learningResource == null)
            {
                return NotFound();
            }

            return View(learningResource);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,Root")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var learningResource = await _context.LearningResources.FindAsync(id);
            if (learningResource != null)
            {
                _context.LearningResources.Remove(learningResource);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Learning resource deleted.";
            return RedirectToAction(nameof(Index));
        }

        private bool LearningResourceExists(int id)
        {
            return _context.LearningResources.Any(e => e.Id == id);
        }
    }
}
