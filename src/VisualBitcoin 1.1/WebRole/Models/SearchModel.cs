using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Storage;

namespace WebRole.Models
{
    public class SearchModel
    {
        [Required(ErrorMessage = "Champ vide !")]
        public string Name { get; set; }
    }
}