using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault.Models;

namespace Notes.Models
{
    public class Notice

    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Ein Titel ist erforderlich!")]
        [Display(Name = "Titel")]
        [StringLength(50, ErrorMessage = "Die maximale Länge von 50 Zeichen wurde überschritten!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Eine Beschreibung ist erforderlich!")]
        [Display(Name = "Beschreibung")]
        public string Description { get; set; }

        [HiddenInput, Range(1, 5, ErrorMessage = "Es ist die Wichtigkeit zwischen 1 und 5 zu wählen!")]
        [Required]
        [Display(Name = "Wichtigkeit")]       
        public int Importance { get; set; }

        [Required(ErrorMessage = "Ein Erstelldatum ist erforderlich!")]
        [DataType(DataType.DateTime), DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:dd.MM.yyyy HH:mm:ss}")]
        [Display(Name = "Erstelldatum")]
        public DateTime Date { get; set; }

        [Display(Name = "Status")]
        public bool State { get; set; }

        public string CreatorId { get; set; }

        [Display(Name = "Ersteller")]
        public ApplicationUser Creator { get; set; }
    }
}
