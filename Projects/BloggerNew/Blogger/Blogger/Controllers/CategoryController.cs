using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Blogger.Data;
using Blogger.Helpers.CustomAttributes;
using Blogger.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;

namespace Blogger.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;
        public CategoryController(ApplicationDbContext _CONTEXT, IWebHostEnvironment webHostEnvironment)
        {
            _context = _CONTEXT;
            this.webHostEnvironment = webHostEnvironment;
        }
        [Route("/Category/Lists")]
        public async Task<IActionResult> List()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await BindSelectList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                if(Image != null && !string.IsNullOrEmpty(Image.FileName))
                {
                    string path = Path.Combine(webHostEnvironment.WebRootPath, "Category", "Images", Image.FileName);
                    using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                    {
                        await Image.CopyToAsync(stream);
                    }
                }
                Category setCategory = new Category()
                {
                    Slug = category.Name.GetBlogUrl(),
                    Name = category.Name,
                    ParentId = category.ParentId,
                    Content = category.Content,
                    Status = category.Status,
                    Media = (Image != null && !string.IsNullOrEmpty(Image.FileName)) ? Image.FileName : "NONE"
                };
                await _context.Categories.AddAsync(setCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            else
            {
                await BindSelectList();
                ModelState.AddModelError("", "Check the Required Fields!");
                return View(category);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return NotFound("Invalid ID");
            var model = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (model != null)
            {
                await BindSelectList();
                return View(model);
            }
            return NotFound("Category NOT FOUND!");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category, IFormFile Image)
        {
            if (id <= 0)
                return NotFound("Invalid ID");
            if (ModelState.IsValid)
            {
                if (Image != null && !string.IsNullOrEmpty(Image.FileName))
                {
                    string path = Path.Combine(webHostEnvironment.WebRootPath, "Category", "Images", Image.FileName);
                    if (!System.IO.File.Exists(path))
                    {
                        using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                        {
                            await Image.CopyToAsync(stream);
                        }
                    }
                }

                var ExistingModel = await _context.Categories.FindAsync(id);
                if (ExistingModel != null)
                {
                    ExistingModel.Slug = category.Name.GetBlogUrl();
                    ExistingModel.Name = category.Name;
                    ExistingModel.ParentId = category.ParentId;
                    ExistingModel.Content = category.Content;
                    ExistingModel.Status = category.Status;
                    ExistingModel.Media = (Image != null && !string.IsNullOrEmpty(Image.FileName)) ? Image.FileName : "NONE";
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(List));
            }
            else
            {
                await BindSelectList();
                ModelState.AddModelError("", "Check Required Fields!");
                return View(category);
            }
        }
        public async Task<IActionResult> Details([FromRoute] int id)
        {
            if(id <= 0)
                return BadRequest("Invalid ID");
            var model = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if(model != null)
                return View(model);
            return NotFound("Categories Not Found!");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if(id <= 0)
                return NotFound("Invalid Id");
            var model = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            var models = await _context.Categories.Where(x => x.ParentId == model.Id).ToListAsync();
            if(model != null || models != null)
            {
                if(models.Any())
                {
                    foreach(var getModel in models)
                    {
                        string path = Path.Combine(webHostEnvironment.WebRootPath, "Category", "Images", getModel.Media);
                        if(!string.IsNullOrEmpty(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }
                }
                if(!string.IsNullOrEmpty(model.Media))
                {
                    string path = Path.Combine(webHostEnvironment.WebRootPath, "Category", "Images", model.Media);
                    System.IO.File.Delete(path);
                }
                _context.Categories.Remove(model);
                _context.Categories.RemoveRange(models);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(List));
        }

        private async Task BindSelectList()
        {
            ViewBag.Lists = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}