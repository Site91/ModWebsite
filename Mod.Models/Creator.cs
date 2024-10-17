using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Models
{
    public class Creator : IdentityUser
    {
        //NOTE: Creator id's are STRINGS, not INTS
        public string? Avatar { get; set; }
        [DefaultValue("Active")]
        [ValidateNever]
        public string Status { get; set; }
        public string? AboutMe { get; set; }
        public string? MCID { get; set; }
        public string? SiteRoles { get; set; }

    }
}
