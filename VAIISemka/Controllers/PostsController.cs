﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using VAIISemka.Data;
using VAIISemka.Models;

namespace VAIISemka.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PostsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            var posts = _context.Posts.ToList();

            posts.ForEach(post => post.Body = Regex.Replace(post.Body, "<.*?>", string.Empty));
            posts.ForEach(post => post.Body = post.Body.Substring(0, Math.Min(post.Body.Length, 500)) + "...");

            return View(_context.Posts.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            Post post = new Post
            {
                Comments = new List<Comment>(),
            };

            return View(post);
        }

        [HttpPost]
        public IActionResult Create(Post post)
        {
            _context.Posts.Add(post);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

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
        public IActionResult Edit(Post post)
        {
            var original = _context.Posts.FirstOrDefault(original => original.Id == post.Id);

            original.Header = post.Header;
            original.Body = post.Body;
            original.ThumbnailImage = post.ThumbnailImage;

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
