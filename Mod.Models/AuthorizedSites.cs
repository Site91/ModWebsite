using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Models
{
    public class AuthorizedSites
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [ValidateNever]
        public string AccessCode { get; set; } //code to access.
        [Required]
        [ValidateNever]
        public string Directory { get; set; } //Directory of the Site's configs.
        [Required]
        public string Name { get; set; } //Name of the site
        [Required]
        public string AccessURL { get; set; } //Extra area declaration to use to get to a specific site.


    }
}
