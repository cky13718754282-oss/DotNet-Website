using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Geekspace.Data;
using Geekspace.Models;

namespace Geekspace.Controllers
{
    // Handles the discussion feature attached to each learning resource.
    // Any authenticated (registered) member may post a comment;
    // only the comment's author or an Admin may delete it.
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CommentController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: Comment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int learningResourceId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["CommentError"] = "Comment cannot be empty.";
                return RedirectToAction("Details", "Resource", new { id = learningResourceId });
            }

            if (content.Length > 1000)
            {
                TempData["CommentError"] = "Comment cannot exceed 1000 characters.";
                return RedirectToAction("Details", "Resource", new { id = learningResourceId });
            }

            var userId = _userManager.GetUserId(User);

            var comment = new ResourceComment
            {
                LearningResourceId = learningResourceId,
                UserId = userId!,
                Content = content,
                PostedDate = DateTime.Now
            };

            _context.ResourceComments.Add(comment);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Your comment was posted.";

            return RedirectToAction("Details", "Resource", new { id = learningResourceId });
        }

        // POST: Comment/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _context.ResourceComments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            bool isOwner = comment.UserId == currentUserId;
            bool isCurrentRoot = User.IsInRole("Root");
            bool isCurrentAdmin = User.IsInRole("Admin");

            bool allowed;

            if (isOwner)
            {
                // Anyone may delete their own comment, regardless of role.
                allowed = true;
            }
            else if (isCurrentRoot)
            {
                // Root may delete any comment from anyone, including other Root or Admin accounts.
                allowed = true;
            }
            else if (isCurrentAdmin)
            {
                // Admin may delete anyone's comment EXCEPT a comment posted by a Root account.
                var commentAuthor = await _userManager.FindByIdAsync(comment.UserId);
                bool authorIsRoot = commentAuthor != null && await _userManager.IsInRoleAsync(commentAuthor, "Root");
                allowed = !authorIsRoot;
            }
            else
            {
                // Plain users may only delete their own comments (handled by isOwner above).
                allowed = false;
            }

            if (!allowed)
            {
                return Forbid();
            }

            int resourceId = comment.LearningResourceId;
            _context.ResourceComments.Remove(comment);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Comment deleted.";

            return RedirectToAction("Details", "Resource", new { id = resourceId });
        }
    }
}
