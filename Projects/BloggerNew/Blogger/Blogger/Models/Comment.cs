using System.ComponentModel.DataAnnotations;
using Blogger.Data;

namespace Blogger.Models
{
    public class Comment
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "*Content is Required"), StringLength(500, MinimumLength = 10, ErrorMessage = "Comment must be above 10 and below 500 characters.")]
        public string Content { get; set; }
        public DateTime CommentedAt { get; set; }

        public int PostId { get; set; }
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Post Post { get; set; }
    }
}
