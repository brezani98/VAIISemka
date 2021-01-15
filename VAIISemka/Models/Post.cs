using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace VAIISemka.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
        public string ThumbnailImage { get; set; }
        public IdentityUser Author { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
