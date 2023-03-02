using System.ComponentModel.DataAnnotations;

namespace Proiect.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Continutul comentariului este obligatoriu")]
        public string content { get; set; }

        [Required(ErrorMessage = "Continutul comentariului este obligatoriu")]
        public string status { get; set; }

        [Required(ErrorMessage = "Continutul comentariului este obligatoriu")]
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }

        public int? ProjectId { get; set; } 
        public virtual Project? Project { get; set; }

        public int? BookmarkId { get; set; }
        public virtual Bookmark? Bookmark { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

    //public enum Status
    //{
    //    ToDo,
    //    Doing,
    //    Done
    //}
}
