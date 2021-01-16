using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using VAIISemka.Models;

namespace VAIISemka.ViewModels
{
    public class PostCategoryViewModel
    {
        public Post Post { get; set; }
        public List<SelectListItem> Categories { get; set; }
    }
}
