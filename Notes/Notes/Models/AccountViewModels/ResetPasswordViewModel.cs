using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Models.AccountViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Das {0} muss mind. aus {2} und max. {1} Zeichen bestehen.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Passwort")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bestätigte Passwort")]
        [Compare("Password", ErrorMessage = "Das neue Passwort stimmt nicht mit dem bestätigten Passwort überein.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
