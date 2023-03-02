using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Project>? Projects { get; set; }
        public virtual ICollection<Task>? Tasks { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }

        public virtual ICollection<UserTeam>? UserTeams { get; set; }

        [NotMapped]

        public IEnumerable<SelectListItem>? AllRoles { get; set; }

    }

}
