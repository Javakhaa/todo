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

        public ActionResult OrganizeToday(string searchValue)
        {
            var model = new TasksViewModel();

            using (var db = new DbConnectionDataContext())
            {
                var tasks = db.Tasks.Where(t => t.Description.ToLower().Contains(string.IsNullOrEmpty(searchValue) ? string.Empty : searchValue)).Select(task => new TaskViewModel
                {
                    Id = task.ID,
                    Description = task.Description,
                    IsDone = task.IsDone
                }).ToList();

                model.Tasks = tasks;
            }

            return PartialView("~/Views/Shared/_OrganizeToday.cshtml", model);
        }

        public ActionResult OrganizeNotes(string searchValue)
        {
            var model = new NotesViewModel();

            using (var db = new DbConnectionDataContext())
            {
                var notes = db.Notes.Where(note => note.Description.ToLower().Contains(string.IsNullOrEmpty(searchValue) ? string.Empty : searchValue)).Select(note => new NoteViewModel
                {
                    Id = note.ID,
                    Description = note.Description,
                }).ToList();

                model.Notes = notes;
            }

            return PartialView("~/Views/Shared/_OrganizeNotes.cshtml", model);
        }

        [HttpPost]
        public ActionResult AddNote(AddNoteRequest addNoteRequest)
        {
            var user = Session["user"] as User;
            Note note;

            using (var db = new DbConnectionDataContext())
            {
                note = new Note
                {
                    Description = addNoteRequest.Description,
                    UserId = user.ID,
                };

                db.Notes.InsertOnSubmit(note);
                db.SubmitChanges();
            }

            return Json(new { id = note.ID });
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

        public ActionResult DeleteNote(int id)
        {
            var user = Session["user"] as User;

            using (var db = new DbConnectionDataContext())
            {
                var note = db.Notes.SingleOrDefault(t => t.ID == id);


                db.Notes.DeleteOnSubmit(note);
                db.SubmitChanges();
            }

            return new EmptyResult();
        }

    }

   
}