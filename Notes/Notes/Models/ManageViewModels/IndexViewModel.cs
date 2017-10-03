using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Models.ManageViewModels
{
    public class IndexViewModel
    {
        [Display(Name = "Name")]
        public string UserName { get; set; }

        [Display(Name = "Vorname")]
        public string SecondName { get; set; }

        [Display(Name = "E-Mail Bestätigung")]
        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Telefon Nr.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Statusnachricht")]
        public string StatusMessage { get; set; }
    }
}
