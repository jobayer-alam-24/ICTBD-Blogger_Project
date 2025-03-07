using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AspNetCoreGeneratedDocument;
using Blogger.Data;
using Blogger.Enums;
using Blogger.Helpers.CustomAttributes;
using Blogger.Models;
using Blogger.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Blogger.Controllers
{
    public class PostController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        public PostController(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = dbContext;
        }
        // private List<Post> _posts = PostService.GetAllPosts();

        //Search logic
        public async Task<IActionResult> List([FromQuery(Name = "search")] string search_key, Status? status)
        {
            List<Post> posts = new List<Post>();
            if (string.IsNullOrEmpty(search_key) && status == null)
            {
                posts = await _context.Posts.ToListAsync();
                return View(posts);
            }
            else if (status == null && !string.IsNullOrEmpty(search_key))
            {
                var post = await _context.Posts.Where(x => x.Content.ToLower().Contains(search_key.ToLower())).ToListAsync();
                return View(post);
            }
            else if (status != null && string.IsNullOrEmpty(search_key))
            {
                var post = await _context.Posts.Where(x => x.Status == status).ToListAsync();
                return View(post);
            }
            else
            {
                var post = await _context.Posts.Where(x => x.Content.ToLower().Contains(search_key.ToLower()) && x.Status == status).ToListAsync();
                return View(post);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await BindSelectListCategories();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Post post, int CategoryId, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                if(Image != null && Image.FileName != null)
                {
                    string extension = Path.GetExtension(Image.FileName);
                    long size = Image.Length;
                    if(extension.Equals(".jpg") || extension.Equals(".jpeg") || extension.Equals(".png"))
                    {
                        if(size <= 100000)
                        {
                            string path = Path.Combine(_webHostEnvironment.WebRootPath, "Post", "Images", Image.FileName);
                            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                            {
                                await Image.CopyToAsync(fileStream);
                            }
                        }
                        else
                        {
                            TempData["size_error"] = "File must be less than 100000";
                            return View(post);
                        }
                    }
                    else
                    {
                        TempData["type_error"] = "File type should be .jpg/.png/.jpeg";
                        return View(post);
                    }
                    
                }
                post.CategoryId = CategoryId;
                post.Slug = post.Title.GetBlogUrl();
                post.Media = (Image != null && !string.IsNullOrEmpty(Image.FileName)) ? Image.FileName : "NONE";
                await _context.Posts.AddAsync(post);
                await _context.SaveChangesAsync();
                TempData["success-messege"] = "Post created successfully!";
                return RedirectToAction("List");
            }
            ModelState.AddModelError(" ", "Check Required Fields!");
            await BindSelectListCategories();
            return View(post);
        }
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid Id");
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == id);
            if (post == null)
                return NotFound("Post Not Found!");
            await BindSelectListCategories();
            return View(post);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post post, int CategoryId, IFormFile Image)
        {
            if (ModelState.IsValid)
            {

                var ExistingPosts = _context.Posts.FirstOrDefault(x => x.Id == id);
                if(Image != null && !string.IsNullOrEmpty(Image.FileName))
                {
                    string extension = Path.GetExtension(Image.FileName);
                    long size = Image.Length;
                    if (extension.Equals(".jpg") || extension.Equals(".png") || extension.Equals(".jpeg"))
                    {
                        if(size <= 100000)
                        {
                            string path = Path.Combine(_webHostEnvironment.WebRootPath, "Post", "Images", Image.FileName);
                            if (ExistingPosts.Media != Image.FileName)
                            {
                                string SetPath = Path.Combine(_webHostEnvironment.WebRootPath, "Post", "Images", Image.FileName);
                                using (var fileStream = new FileStream(SetPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                                {
                                    await Image.CopyToAsync(fileStream);
                                }

                            }
                            else
                            {
                                using (var stream = new FileStream(path, FileMode.Truncate, FileAccess.ReadWrite, FileShare.Read))
                                {
                                    await Image.CopyToAsync(stream);
                                }
                            }
                        }
                        else
                        {
                            TempData["size_error"] = "File must be less than 100000";
                            return View(post);
                        }

                    }
                    else
                    {
                        TempData["type_error"] = "File type should be .jpg/.png/.jpeg";
                        return View(post);
                    }
                }
                ExistingPosts.Content = post.Content;
                ExistingPosts.CreatedAt = post.CreatedAt;
                ExistingPosts.PublishedAt = post.PublishedAt;
                ExistingPosts.UpdatedAt = post.UpdatedAt;
                ExistingPosts.Slug = post.Title.GetBlogUrl();
                ExistingPosts.Media = (Image != null && !string.IsNullOrEmpty(Image.FileName)) ? Image.FileName : "NONE";
                ExistingPosts.Status = post.Status;
                ExistingPosts.UserId = post.UserId;
                ExistingPosts.Title = post.Title;
                ExistingPosts.CategoryId = CategoryId;
                await _context.SaveChangesAsync(true);
                TempData["success-messege"] = "Post Updated Successfully!";
                return RedirectToAction(nameof(List));
            }
            ModelState.AddModelError(" ", "Check Required Fields!");
            await BindSelectListCategories();
            return View(post);
        }
        [HttpGet]
        public async Task<IActionResult> Details([FromRoute] int id)
        {
            if (id <= 0)
                return BadRequest("Invalid Id");
            var model = await _context.Posts.FirstOrDefaultAsync(x => x.Id == id);
            if (model != null)
                return View(model);
            return NotFound("Post Not Found!");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0)
                return BadRequest("Invalid Id");
            var model = await _context.Posts.FirstOrDefaultAsync(x => x.Id == id);
            if (model == null)
                return NotFound("Post Not Found!");
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "Post", "Images", model.Media);
            if(!string.IsNullOrEmpty(path))
            {
                System.IO.File.Delete(path);
            }
                _context.Posts.Remove(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));
        }
        private async Task BindSelectListCategories()
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}