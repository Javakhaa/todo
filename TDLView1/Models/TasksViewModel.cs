using System.Collections.Generic;

namespace TDLView1.Models
{
    public class TasksViewModel
    {
        public List<TaskViewModel> Tasks { get; set; }
    }

    public class TaskViewModel
    {
        public int Id { get; set; }
        public bool IsDone { get; set; }
        public string Description { get; set; }
    }
}
