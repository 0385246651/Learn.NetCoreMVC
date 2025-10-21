using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using App.Models.Blog;

namespace App.Areas.Blog.Models
{
    public class CreatePostModel : Post
    {
        [Display(Name = "Danh mục")]
        public int[] CategoryIDs { get; set; }
    }
}