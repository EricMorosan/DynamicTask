using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Proiect.Data;
using Proiect.Models;
using System.Data;
using System.Dynamic;
using System.Threading.Tasks;
using Task = Proiect.Models.Task;

namespace Proiect.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public TasksController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public List<Comment> GetComment()
        {
            var comments = new List<Comment>();
            var comments1 = db.Comments;
            foreach (var comment in comments1)
                comments.Add(comment);
            return comments;
        }
        public ActionResult Show(int id)
        {
            var task = db.Tasks.Find(id);
            var project_id = db.Projects.Where(p => p.Id == task.ProjectId).Select(p => p.Id).First();
            var project = db.Projects.Find(project_id);
            var t_id = db.Teams.Where(t => t.ProjectId == project_id).Select(t => t.Id).First();
            var users = db.UserTeams.Where(ut => ut.TeamId == t_id).Select(ut => ut.UserId).ToList();
            var userid = _userManager.GetUserId(User);
            if (users.Contains(userid) || project.UserId == userid || User.IsInRole("Admin"))
            {
                dynamic mymodel2 = new ExpandoObject();
                mymodel2.Task = db.Tasks.Find(id);
                mymodel2.Comments = GetComment();
                return View(mymodel2);
            }
            else
            {
                return RedirectToAction("Index", "Projects");
            }
        }


        [Authorize(Roles = "User, Admin")]
        public IActionResult New(int id)
        {

            var project_id = id;
            var project = db.Projects.Find(project_id);//proiect din care face parte taskul
            var t_id = db.Teams.Where(t => t.ProjectId == project_id).Select(t => t.Id).First();//id ul echipei care are project_id
            var users = db.UserTeams.Where(ut => ut.TeamId == t_id).Select(ut => ut.UserId).ToList();
            var userid = _userManager.GetUserId(User);
            if ( project.UserId == userid || User.IsInRole("Admin"))
            {
                var task2 = new Task();
                ViewBag.Id = id;
                var tid = db.Teams.Where(t => t.ProjectId == id).Select(t => t.Id).First();
                var teamusers = db.UserTeams.Include("User").Where(ut => ut.TeamId == tid);
                ViewBag.Users = teamusers;
                ViewBag.teamid = tid;
                return View(task2);
            }
            else
            {
                return RedirectToAction("Index", "Projects");
            }
        }
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public IActionResult New(Task t)
        {
            try
            {
                db.Tasks.Add(t);
                db.SaveChanges();
                return Redirect("/Projects/Show/" + t.ProjectId);
            }
            catch (Exception)
            {
                return View();
            }
        }
        [Authorize(Roles = "User, Admin")]
        public IActionResult Edit(int id)
        {
            var task = db.Tasks.Find(id);
            var project_id = db.Projects.Where(p => p.Id == task.ProjectId).Select(p => p.Id).First();
            var project = db.Projects.Find(project_id);
            var t_id = db.Teams.Where(t => t.ProjectId == project_id).Select(t => t.Id).First();
            var users = db.UserTeams.Where(ut => ut.TeamId == t_id).Select(ut => ut.UserId).ToList();
            var userid = _userManager.GetUserId(User);
            if (users.Contains(userid) || project.UserId == userid || User.IsInRole("Admin"))
            {
                Task task2 = db.Tasks.Find(id);
                ViewBag.Task = task2;
                return View(task2);
            }
            else
            {
                return RedirectToAction("Show", "Projects", project_id);
            }
        }
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public ActionResult Edit([FromForm] int TaskId, [FromForm] string status)
        {
            Task task = db.Tasks.Find(TaskId);
            try 
            { 
            
                task.status = status;

                db.SaveChanges();
                return Redirect("/Projects/Show/" + task.ProjectId);
            }
            catch (Exception)
            {
                return RedirectToAction("Edit", task.Id);
            }
        }

        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Task task = db.Tasks.Find(id);
            var project_id = db.Projects.Where(p => p.Id == task.ProjectId).Select(p => p.Id).First();
            var project = db.Projects.Find(project_id);
            if (_userManager.GetUserId(User) == project.UserId || User.IsInRole("Admin"))
            {
                int? id2 = task.ProjectId;
                var comms = db.Comments.Where(c => c.TaskId == id);
                foreach(var comm in comms)
                    db.Comments.Remove(comm);
                db.Tasks.Remove(task);
                db.SaveChanges();
                return Redirect("/Projects/Show/" + id2);
            }
            else
            {
                return RedirectToAction("Index", "Projects");
            }
        }
    }
}
