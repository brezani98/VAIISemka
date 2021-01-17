using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VAIISemka.Data;
using VAIISemka.Models;
using VAIISemka.ViewModels;

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
            var posts = _context.Posts.Include(post => post.Category)
                                        .Include(post => post.Author)
                                        .ToList();

            posts = ConvertPostsToThumbnailStyle(posts);

            return View(posts);
        }

        [AllowAnonymous]
        public IActionResult FilterByCategory(int categoryId)
        {
            IQueryable<Post> query = _context.Posts.Include(post => post.Category)
                                                .Include(post => post.Author);

            if (categoryId == 0)
            {
                query = query.Where(post => post.Category == null);
            }
            else
            {
                query = query.Where(post => post.Category.Id == categoryId);
            }

            var postsByCategory = query.ToList();
            postsByCategory = ConvertPostsToThumbnailStyle(postsByCategory);

            return View("Index", postsByCategory);
        }

        private List<Post> ConvertPostsToThumbnailStyle(List<Post> posts)
        {
            posts = posts.OrderByDescending(post => post.CreateDate.Date)
                            .ThenByDescending(post => post.CreateDate.TimeOfDay)
                            .ToList();

            posts.ForEach(post => post.Body = Regex.Replace(post.Body, "<.*?>", string.Empty));
            posts.ForEach(post => post.Body = post.Body.Substring(0, Math.Min(post.Body.Length, 400)) );   
            for (int i = 0; i < posts.Count; i++)
            {
                if (posts[i].Body.Length >= 395)
                    posts[i].Body += "...";
            }

            return posts;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var postCategory = new PostCategoryViewModel()
            {
                Post = new Post(),
                Categories = GetCategoriesAsSelectListItems()
            };

            return View(postCategory);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PostCategoryViewModel postCategory)
        {
            postCategory.Post.Author = await _userManager.GetUserAsync(User);
            postCategory.Post.CreateDate = DateTime.Now;
            postCategory.Post.Category = _context.Categories.FirstOrDefault(category => category.Id == postCategory.Post.Category.Id);

            _context.Posts.Add(postCategory.Post);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Details(int id)
        {
            return View(_context.Posts.Include(post => post.Category).Include(post => post.Author).FirstOrDefault(post => post.Id == id));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var postCategory = new PostCategoryViewModel()
            {
                Post = _context.Posts.Include(post => post.Category).FirstOrDefault(post => post.Id == id),
                Categories = GetCategoriesAsSelectListItems()
            };

            return View(postCategory);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PostCategoryViewModel postCategory)
        {
            var original = _context.Posts.Include(post => post.Category).Include(post => post.Author).FirstOrDefault(original => original.Id == postCategory.Post.Id);

            original.Header = postCategory.Post.Header;
            original.Body = postCategory.Post.Body;
            original.ThumbnailImage = postCategory.Post.ThumbnailImage;

            if (original.Category == null)
            {
                original.Category = new Category();
            }

            if (original.Category.Id != postCategory.Post.Category.Id)
            {
                original.Category = _context.Categories.FirstOrDefault(category => category.Id == postCategory.Post.Category.Id);
            }
            
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
        public ActionResult UploadImage(PostCategoryViewModel postCategory, bool editing)
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
                    postCategory.Post.ThumbnailImage = fileName;
                }
            }

            ModelState.Clear();
            postCategory.Categories = GetCategoriesAsSelectListItems();

            if (editing)
            {
                return View("Edit", postCategory);
            }
            else
            {
                return View("Create", postCategory);
            }
        }

        [HttpPost]
        public ActionResult RemoveImage(PostCategoryViewModel postCategory, string img, bool editing)
        {
            if (postCategory.Post.ThumbnailImage == img)
            {
                var path = Path.Combine(_hostEnvironment.WebRootPath, @"images", img);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                postCategory.Post.ThumbnailImage = null;
            }

            ModelState.Clear();
            postCategory.Categories = GetCategoriesAsSelectListItems();

            if (editing)
            {
                return View("Edit", postCategory);
            }
            else
            {
                return View("Create", postCategory);
            }
        }

        public IActionResult CheckHeader(string header, int postID)
        {
            bool result = _context.Posts.Any(post => post.Header == header && post.Id != postID);
            return new JsonResult(result);
        }

        private List<SelectListItem> GetCategoriesAsSelectListItems()
        {
            return _context.Categories.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToList();
        }
    }
}
