using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Models.ManageViewModels
{
    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "{0} muss mind. aus {2} und max. {1} Zeichen bestehen.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Neues Passwort")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bestätigtes Passwort")]
        [Compare("Password", ErrorMessage = "Das neue Passwort stimmt nicht mit dem bestätigten Passwort überein.")]
        public string ConfirmPassword { get; set; }

        public string StatusMessage { get; set; }
    }
}
