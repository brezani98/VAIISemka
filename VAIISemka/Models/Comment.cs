using Microsoft.AspNetCore.Identity;

namespace VAIISemka.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public IdentityUser Author { get; set; }
    }
}
