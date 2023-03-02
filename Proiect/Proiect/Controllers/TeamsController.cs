//using AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect.Data;
using Proiect.Models;
using System;
using System.Data;
using System.Dynamic;
using Task = Proiect.Models.Task;

namespace Proiect.Controllers
{
    public class TeamsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public TeamsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "User, Admin")]
        public IActionResult New(int id)
        {
            var team = new Team();
            ViewBag.Id = id;
            return View(team);

        }
        [Authorize(Roles = "User, Admin")]

        [HttpPost]
        public IActionResult New(Team t)
        {
            try
            {   
                db.Teams.Add(t);
                db.SaveChanges();
                return Redirect("/Teams/Users/" + t.Id);
            }
            catch (Exception)
            {
                return View();
            }
        }
        [Authorize(Roles = "User,Admin")]
        public IActionResult Users(int id)
        {
            var userid = _userManager.GetUserId(User);
            var team = db.Teams.Find(id);
            var user_editor = db.Projects.Where(project => project.Id == team.ProjectId).Select(pr => pr.UserId);
            if (!user_editor.Any())
            {
                TempData["message"] = "Database error!";
                return RedirectToAction("Users", id);
            }
            if (user_editor.Contains(userid) || User.IsInRole("Admin"))
            {

                var users_search = db.Users.Where(a => 1 == 0);
                var search = "";

                // MOTOR DE CAUTARE
                if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
                {
                    search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                    users_search = db.Users.Where(usn => usn.UserName.Contains(search) &&
                                                        !usn.UserName.EndsWith("@test.com"))
                                            .OrderBy(a => a.UserName);
                }

                var project = db.Projects.Where(proj => proj.Id == team.ProjectId);
                var users = db.UserTeams.Include("User").Where(ut => ut.TeamId == id);
                ViewBag.Users = users;
                ViewBag.Project = project.First();
                ViewBag.AllUsers = users_search;
                ViewBag.Team = team;
                return View();
            }
            else
            {
                TempData["message"] = "Error! Nu ai acces";
                ViewBag.Message = TempData["message"];
                return Redirect("/Projects/Index");
            }
        }
        [Authorize(Roles = "User,Admin")]
        [HttpPost]
     
        public IActionResult Users2(int id, [FromForm] UserTeam requestUser)
        {
            var userid = _userManager.GetUserId(User);
            var team = db.Teams.Find(id);
            var project = db.Projects.Find(team.ProjectId);
            if (team == null)
            {
                TempData["message"] = "Database error!";
                return Redirect("Index");
            }
            else
            {
                if (userid == project.UserId || User.IsInRole("Admin"))
                {
                    UserTeam userteam = new UserTeam();
                    userteam.TeamId = id;
                    userteam.UserId = requestUser.UserId;
                    db.UserTeams.Add(userteam);
                    db.SaveChanges();
                    return Redirect("/Teams/Users/" + id);
                }
                else
                {
                    TempData["message"] = "Error! Nu ai acces";
                    ViewBag.Message = TempData["message"];
                    return RedirectToAction("Index");
                }
            }
        }
    }
}
