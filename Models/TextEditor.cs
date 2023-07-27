using System.ComponentModel.DataAnnotations;

namespace Blog7.Models
{
    public class TextEditor
    {
        [Required(ErrorMessage = "Please enter the post title.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Please select at least one category.")]
        public string[]? Category { get; set; }

        [Required(ErrorMessage = "Please enter the post content.")]
        public string? RichContent { get; set; }
    }
}
