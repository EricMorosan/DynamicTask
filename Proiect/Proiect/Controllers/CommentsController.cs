using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Proiect.Data;
using Proiect.Models;
using System.Data;
using System.Dynamic;
using System.Threading.Tasks;
using Task = Proiect.Models.Task;

namespace Proiect.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CommentsController(
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
            var task = db.Tasks.Find(id);
            var project_id = db.Projects.Where(p => p.Id == task.ProjectId).Select(p => p.Id).First();
            var project = db.Projects.Find(project_id);
            var t_id = db.Teams.Where(t => t.ProjectId == project_id).Select(t => t.Id).First();
            var users = db.UserTeams.Where(ut => ut.TeamId == t_id).Select(ut => ut.UserId).ToList();
            var userid = _userManager.GetUserId(User);

            if (users.Contains(userid) || project.UserId == userid || User.IsInRole("Admin"))
            {
                var comment = new Comment();
                ViewBag.Id = id;
                return View(comment);
            }
            else
            {
                return RedirectToAction("Show","Projects", id);
            }

        }
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public IActionResult New(Comment c)
        {

            try
            {
                var userid = _userManager.GetUserId(User);
                c.UserId = userid;
                db.Comments.Add(c);
                db.SaveChanges();
                return Redirect("/Tasks/Show/" +  c.TaskId);
            }
            catch (Exception)
            {
                return View();
            }
        }
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
 
        public ActionResult Delete(int id)
        {
            var userid = _userManager.GetUserId(User);
            Comment comment = db.Comments.Find(id);
            int? id2 = comment.TaskId;
            if (userid == comment.UserId || User.IsInRole("Admin"))
            {
                db.Comments.Remove(comment);
                db.SaveChanges();
                return Redirect("/Tasks/Show/" + id2);
            }
            else
            {
                TempData["message"] = "Error! Nu tu ai creat comentariul";
                ViewBag.Message = TempData["message"];
                return Redirect("/Tasks/Show/" + id2);
            }
        }




        //[Authorize(Roles = "User, Admin")]

        //public IActionResult Edit(int id)
        //{
        //    Comment comment = db.Comments.Find(id);
        //    ViewBag.Comment = comment;
        //    return View();
        //}

        //[Authorize(Roles = "User, Admin")]
        //[HttpPost]
        //public ActionResult Edit(int id, Comment requestComment)
        //{
        //    Comment comment = db.Comments.Find(id);
        //    int? id2 = comment.TaskId;
        //    var userid = _userManager.GetUserId(User);
        //    if (userid == comment.UserId || User.IsInRole("Admin"))
        //    { 
        //        comment.content = requestComment.content;
        //        db.SaveChanges();
        //        return Redirect("/Tasks/Show/" + id2);
        //    }
        //    else
        //    {
        //        TempData["message"] = "Error! Nu tu ai creat comentariul";
        //        ViewBag.Message = TempData["message"];
        //        return Redirect("/Tasks/Show/" + id2);

        //    }

        //}

        //[Authorize(Roles = "User, Admin")]
        //public ActionResult Edit(int id)
        //{
        //    //Comment comm = db.Comments.Find(id);
        //    //ViewBag.Comment = comm;
        //    //if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
        //    //{
        //    //    return View(comm);
        //    //}
        //    //else
        //    //{
        //    //    return Redirect("/Tasks/Show/" + comm.TaskId);
        //    //}
        //    Comment c = db.Comments.Find(id);
        //    ViewBag.Comment = c;
        //    return View();
        //}

        //[HttpPost]
        //[Authorize(Roles = "User,Admin")]
        //public IActionResult Edit(int id, Comment requestComment)
        //{
        //    Comment comm = db.Comments.Find(id);
        //    try
        //    {
        //        if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
        //        { 
        //            //if (TryUpdateModel(comm))

        //            comm.content = requestComment.content;
        //            db.SaveChanges();

        //            return Redirect("/Tasks/Show/" + comm.TaskId);
        //        }
        //        else
        //        {
        //            return Redirect("/Tasks/Show/" + comm.TaskId);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return View();
        //    }
        //}

        // In acest moment vom implementa editarea intr-o pagina View separata
        // Se editeaza un comentariu existent
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {
            Comment comm = db.Comments.Find(id);

            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(comm);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati comentariul";
                return Redirect("/Tasks/Show/" + comm.TaskId);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Comment requestComment)
        {
            Comment comm = db.Comments.Find(id);

            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {

                try
                {
                    comm.content = requestComment.content;

                    db.SaveChanges();

                    return Redirect("/Tasks/Show/" + comm.TaskId);
                }
                catch (Exception)
                {
                    return RedirectToAction("Edit", comm.Id);
                }

            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                return Redirect("/Tasks/Show/" + comm.TaskId); ;
            }
        }


    }
}
