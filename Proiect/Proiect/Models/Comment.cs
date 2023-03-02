using System.ComponentModel.DataAnnotations;

namespace Proiect.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Continutul comentariului este obligatoriu")]
        public string content { get; set; } 

        public int? TaskId { get; set; }   
        public virtual Task? Task { get; set; }

        public string? UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
