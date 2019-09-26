using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TDLView1.Models
{
    public class NotesViewModel
    {
        public List<NoteViewModel> Notes { get; set; }
    }

    public class NoteViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }
    
}