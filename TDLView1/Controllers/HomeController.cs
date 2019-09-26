using System.Linq;
using System.Web.Mvc;
using TDLView1.Models;

namespace TDLView1.Controllers
{
    public class HomeController : Controller
    {
        // GET: Main
        public ActionResult Index()
        {


            return View();
        }

        public ActionResult Organize()
        {
            var user = Session["user"];
            if (user == null)
            {
                return RedirectToAction("login", "user");
            }
            return View();
        }

        public ActionResult OrganizeToday()
        {
            var model = new TasksViewModel();

            using (var db = new DbConnectionDataContext())
            {
                var tasks = db.Tasks.Select(task => new TaskViewModel
                {
                    Id = task.ID,
                    Description = task.Description,
                    IsDone = task.IsDone
                }).ToList();

                model.Tasks = tasks;
            }

            return PartialView("~/Views/Shared/_OrganizeToday.cshtml", model);
        }

        [HttpPost]
        public ActionResult AddTask(AddTaskRequest addTaskRequest)
        {
            var user = Session["user"] as User;
            Task task;

            using (var db = new DbConnectionDataContext())
            {
                task = new Task
                {
                    IsDone = false,
                    ProjectId = null,
                    Description = addTaskRequest.Description,
                    UserId = user.ID
                };

                db.Tasks.InsertOnSubmit(task);
                db.SubmitChanges();
            }

            return Json(new { id = task.ID });
        }

        [HttpPost]
        public ActionResult EditTask(EditTaskRequest editTaskRequest)
        {
            var user = Session["user"] as User;

            using (var db = new DbConnectionDataContext())
            {
                var task = db.Tasks.SingleOrDefault(t => t.ID == editTaskRequest.TaskId);

                task.IsDone = editTaskRequest.IsDone;

                db.SubmitChanges();
            }

            return new EmptyResult();
        }

        public ActionResult DeleteTask(int id)
        {
            var user = Session["user"] as User;

            using (var db = new DbConnectionDataContext())
            {
                var task = db.Tasks.SingleOrDefault(t => t.ID == id);


                db.Tasks.DeleteOnSubmit(task);
                db.SubmitChanges();
            }

            return new EmptyResult();
        }

    }

   
}