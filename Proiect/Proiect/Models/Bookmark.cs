using System.ComponentModel.DataAnnotations;

namespace Proiect.Models
{
    public class Bookmark
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Continutul comentariului este obligatoriu")]
        public string title { get; set; }

    }
}
