using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Notes.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Name")]
        public string FirstName { get; set; }

        [Display(Name = "Vorname")]
        public string SecondName { get; set; }

        public string Roles { get; set; }
        public virtual List<Notice> Notes { get; set; }
    }
}
