using System.ComponentModel.DataAnnotations;

namespace MVC_1.Models
{
    public class CreatePost:Post
    {
        [Display(Name ="Danh mục")]
        [Required]
        public int[]? CategoryID {set;get;}
    }
}