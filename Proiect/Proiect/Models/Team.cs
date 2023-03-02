using System.ComponentModel.DataAnnotations;

namespace Proiect.Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Continutul comentariului este obligatoriu")]
        public string name { get; set; }
        
        public int? ProjectId { get; set; }    
        public virtual Project? Project { get; set; }

        public virtual ICollection<UserTeam>? UserTeams { get; set; }

    }
}
