using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TDLView1.Models
{
    public class RegistrationViewModel
    {
        [Required(ErrorMessage = "Employee {0} is required")]
        public string Name { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RepeatPassword { get; set; }

    }
}