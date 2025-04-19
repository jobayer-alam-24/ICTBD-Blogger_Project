using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Blogger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Blogger.Data;
using Microsoft.EntityFrameworkCore;

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
            .Include(x => x.Category)
            .Include(x => x.User)
            .FirstOrDefault(p => p.Slug == slug);
        return View(post);
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
