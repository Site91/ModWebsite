using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Models
{
    public class UserSiteRole
    {
        //Table to link user with site roles.
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; } //the ID of the user themselves.
        public Guid RoleId { get; set; } //ID of the role ID itself
        [ForeignKey("RoleId")]
        public SiteRoles Role { get; set; }
    }
}
