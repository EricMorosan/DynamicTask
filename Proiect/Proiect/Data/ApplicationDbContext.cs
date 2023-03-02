using Proiect.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Task = Proiect.Models.Task;

namespace Proiect.Data
{ 
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<Comment> Comments { get; set; }    
        public DbSet<Team> Teams { get; set; }  
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks{ get; set; }

        public DbSet<UserTeam> UserTeams { get; set; }

        protected override void OnModelCreating(ModelBuilder
modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // definire primary key compus
            modelBuilder.Entity<UserTeam>()
            .HasKey(ab => new {ab.Id, ab.UserId, ab.TeamId});
            // definire relatii cu modelele Bookmark si Article (FK)
            modelBuilder.Entity<UserTeam>()
            .HasOne(ab => ab.User)
            .WithMany(ab => ab.UserTeams)
            .HasForeignKey(ab => ab.UserId);
            modelBuilder.Entity<UserTeam>()
            .HasOne(ab => ab.Team)
            .WithMany(ab => ab.UserTeams)
            .HasForeignKey(ab => ab.TeamId);
        }
    }
}
