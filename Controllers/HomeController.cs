using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Geekspace.Models;
using Geekspace.Data;

namespace Geekspace.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    // GET: / — shows the newest published resources on the landing page.
    public async Task<IActionResult> Index()
    {
        var latestResources = await _context.LearningResources
            .Include(r => r.Category)
            .Where(r => r.IsPublished)
            .OrderByDescending(r => r.CreatedDate)
            .Take(6)
            .ToListAsync();

        ViewBag.ResourceCount = await _context.LearningResources.CountAsync(r => r.IsPublished);
        ViewBag.CategoryCount = await _context.Categories.CountAsync();
        ViewBag.CommentCount = await _context.ResourceComments.CountAsync();
        ViewBag.Categories = await _context.Categories
            .Include(c => c.Resources)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return View(latestResources);
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
