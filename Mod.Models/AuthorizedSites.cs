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
        public string AccessCode { get; set; } //code to access.
    }
}
