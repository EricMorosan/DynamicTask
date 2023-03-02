using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Proiect.Data;
using Proiect.Models;
using System;
using System.Data;
using System.Dynamic;
using Task = Proiect.Models.Task;

namespace Proiect.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        
        public ProjectsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        private List<Project> GetProjects()
        {
            var projects = new List<Project>();
            var projects1 = db.Projects;
            foreach (var project in projects1)
                projects.Add(project);
            return projects;
        }

        public List<Task> GetTask()
        {
            var tasks = new List<Task>();
            var tasks1 = db.Tasks;
            foreach (var task in tasks1)
                tasks.Add(task);
            return tasks;
        }

        [Authorize(Roles = "User, Admin")]

        public IActionResult Index()
        {
            var userid = _userManager.GetUserId(User);
            var projecteditorid = db.Projects.Where(pr => pr.UserId == userid).Select(p => p.Id);
            var teamids = db.UserTeams.Where(ut => ut.UserId == userid).Select(ut => ut.TeamId);
            var projectids = db.Teams.Where(t => teamids.Contains(t.Id)).Select(t => t.ProjectId);
            var projects = db.Projects.Where(p => projectids.Contains(p.Id) || projecteditorid.Contains(p.Id));
            ViewBag.Projects = projects;
            return View();

        }

        [Authorize(Roles = "User, Admin")]

        public ActionResult Show(int id)
        {
            var t_id = db.Teams.Where(t => t.ProjectId == id).Select(t => t.Id).First();
            var users = db.UserTeams.Where(u => u.TeamId == t_id).Select(ut => ut.UserId).ToList();
            var userid = _userManager.GetUserId(User);
            var p = db.Projects.Find(id);
            if (users.Contains(userid) || userid == p.UserId || User.IsInRole("Admin"))
            {
                dynamic mymodel = new ExpandoObject();
                mymodel.Project = db.Projects.Find(id);
                mymodel.Tasks = GetTask();
                return View(mymodel);
            }
            else
            {
                return RedirectToAction("Index");   
            }
        }

        [Authorize(Roles = "User, Admin")]

        public IActionResult New()
        {
            var project = new Project();
            return View(project);   

        }
        [Authorize(Roles = "User, Admin")]

        [HttpPost]
        public IActionResult New(Project p)
        {
            try
            {
                p.UserId = _userManager.GetUserId(User);
                db.Projects.Add(p);
                db.SaveChanges();
                return Redirect("/Teams/New/" + p.Id);
            }
            catch (Exception)
            {
                return View();
            }
        }

        [Authorize(Roles = "User, Admin")]

        public IActionResult Edit(int id)
        {
            Project project = db.Projects.Find(id);
            if (_userManager.GetUserId(User) == project.UserId || User.IsInRole("Admin"))
            {
                ViewBag.Project = project;
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public ActionResult Edit(int id, Project requestProject)
        {
            Project project = db.Projects.Find(id);
            try
            {
                project.title = requestProject.title;
                project.content = requestProject.content;
                project.start_date =  requestProject.start_date;
                project.end_date = requestProject.end_date; 
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToAction("Edit", project.Id);
            }
        }
        [Authorize(Roles = "User, Admin")]


        [HttpPost]
        public ActionResult Delete(int id)
        {
            Project project = db.Projects.Find(id);
            if (_userManager.GetUserId(User) == project.UserId || User.IsInRole("Admin"))
             {
                var team = db.Teams.Include("UserTeams").Where(t => t.ProjectId == id).First();
                db.Teams.Remove(team);
                db.Projects.Remove(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

  
    }
}
