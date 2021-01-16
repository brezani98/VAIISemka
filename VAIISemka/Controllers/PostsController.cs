using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VAIISemka.Data;
using VAIISemka.Models;

namespace VAIISemka.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;

        public PostsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var posts = _context.Posts.Include(post => post.Author).ToList();

            posts.ForEach(post => post.Body = Regex.Replace(post.Body, "<.*?>", string.Empty));
            posts.ForEach(post => post.Body = post.Body.Substring(0, Math.Min(post.Body.Length, 400)) + "...");

            return View(_context.Posts.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            Post post = new Post();

            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Post post)
        {
            post.Author = await _userManager.GetUserAsync(User);
            post.CreateDate = DateTime.Now;

            _context.Posts.Add(post);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Details(int id)
        {
            return View(_context.Posts.FirstOrDefault(post => post.Id == id));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            return View(_context.Posts.FirstOrDefault(post => post.Id == id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Post post)
        {
            var original = _context.Posts.FirstOrDefault(original => original.Id == post.Id);

            original.Header = post.Header;
            original.Body = post.Body;
            original.ThumbnailImage = post.ThumbnailImage;
            original.CreateDate = DateTime.Now;
            
            if (original.Author == null)
            {
                original.Author = await _userManager.GetUserAsync(User);
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Remove(int id)
        {
            var postToRemove = _context.Posts.FirstOrDefault(post => post.Id == id);

            _context.Posts.Remove(postToRemove);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UploadImage(Post post, bool editing)
        {
            string webRootPath = _hostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            foreach (var file in files)
            {
                var uploads = Path.Combine(webRootPath, @"images");

                var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);

                using (var filestream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                {
                    file.CopyTo(filestream);
                    post.ThumbnailImage = fileName;
                }
            }

            ModelState.Clear();

            if (editing)
            {
                return View("Edit", post);
            }
            else
            {
                return View("Create", post);
            }
        }

        [HttpPost]
        public ActionResult RemoveImage(Post post, string img, bool editing)
        {
            if (post.ThumbnailImage == img)
            {
                var path = Path.Combine(_hostEnvironment.WebRootPath, @"images", img);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                post.ThumbnailImage = null;
            }

            ModelState.Clear();

            if (editing)
            {
                return View("Edit", post);
            }
            else
            {
                return View("Create", post);
            }
        }

        public IActionResult CheckHeader(string header, int postID)
        {
            bool result = _context.Posts.Any(post => post.Header == header && post.Id != postID);
            return new JsonResult(result);
        }
    }
}
