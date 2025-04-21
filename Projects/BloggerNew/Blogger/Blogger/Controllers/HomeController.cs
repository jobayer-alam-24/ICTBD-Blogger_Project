using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Blogger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Blogger.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Blogger.Controllers;


public class HomeController : Controller
{
    private readonly ApplicationDbContext _cotext;
    public HomeController(ApplicationDbContext context)
    {
        _cotext = context;
    }
    public IActionResult Index()
    {
        return View();
    }
    [HttpGet("/Details/{slug}")]
    public IActionResult Details(string slug)
    {
        var post = _cotext.Posts
     .Include(p => p.Category)
     .Include(p => p.User)
     .Include(p => p.Comments)
         .ThenInclude(c => c.ApplicationUser)
     .FirstOrDefault(p => p.Slug == slug);

        return View(post);
    }
    [HttpPost]
    public async Task<IActionResult> AddComment(string slug, int id, string comment)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (id >  0 || !string.IsNullOrEmpty(comment))
        {
            var post = await _cotext.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (post != null)
            {
                var commentObj = new Comment
                {
                    PostId = id,
                    ApplicationUserId = userId,
                    Content = comment,
                    CommentedAt = DateTime.UtcNow
                };
                _cotext.Comments.Add(commentObj);
                await _cotext.SaveChangesAsync();
            }
        }
        return Redirect($"/Details/{slug}");
    }
    [HttpPost]
    public async Task<IActionResult> DeleteComment(string slug, int id, string userId, int commentId)
    {
        TempData["Slug"] = slug;
        TempData["Id"] = id;
        TempData["UserId"] = userId;

        try
        {
            if (id > 0 && !string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(slug) && commentId > 0)
            {
                var post = _cotext.Posts.FirstOrDefault(p => p.Id == id);
                if (post != null)
                {
                    var comment = await _cotext.Comments.FindAsync(commentId);

                    if (comment != null)
                    {
                        _cotext.Comments.Remove(comment);
                       await _cotext.SaveChangesAsync();
                       
                    }
                   
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return Redirect($"/Details/{slug}");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
