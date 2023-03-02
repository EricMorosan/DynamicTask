using Proiect.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect.Models
{
    public class UserTeam
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? UserId { get; set; }
        public int? TeamId { get; set; }

        public virtual ApplicationUser? User { get; set; }
        public virtual Team? Team{ get; set; }
    }
}