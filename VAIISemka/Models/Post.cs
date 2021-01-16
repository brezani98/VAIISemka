using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VAIISemka.Models
{
    public class Post
    {
        public int Id { get; set; }
        public Category Category { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
        public string ThumbnailImage { get; set; }
        public DateTime CreateDate { get; set; }
        public IdentityUser Author { get; set; }
    }
}
