using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using VAIISemka.Data;
using VAIISemka.Models;

namespace VAIISemka.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;

        public CategoriesController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }


        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();

            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            Category category = new Category();

            return View(category);
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            _context.Add(category);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            var original = _context.Categories.FirstOrDefault(original => original.Id == category.Id);

            original.Name = category.Name;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Remove(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
