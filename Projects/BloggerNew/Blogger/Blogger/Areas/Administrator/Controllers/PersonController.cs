using Blogger.Areas.Administrator.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blogger.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    [AllowAnonymous]
    public class PersonController : Controller
    {
        [Route("/Person/Create")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Route("/Person/Create")]
        public IActionResult Create(Person person)
        {
            if (ModelState.IsValid)
            {
                return Content("Valid");
            }
            else
            {
                return View(person);
            }
        }
    }
}
