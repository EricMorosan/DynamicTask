using System.ComponentModel.DataAnnotations;

namespace Proiect.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Titlul este obligatoriu")]
        [StringLength(100, ErrorMessage = "Titlul nu poate avea mai mult de 100 de caractere")]
        public string title { get; set; }
        public string? content { get; set; }

        [Required(ErrorMessage = "Data de inceput este obligatorie")]
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }

        public string? UserId { get; set; }

        public virtual ApplicationUser User { get; set; }


    }
}
