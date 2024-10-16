using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Models
{
    //Per site roles. Basically access levels and stuff.
    public class SiteRoles
    {
        [Key]
        public Guid Id { get; set; } //ID of the pass
        [Required]
        public string SafeName { get; set; } //the visual name what others see
        [Required]
        public string SiteId { get; set; } //ID of the site itself, to ensure it knows.
        public bool canRemove { get; set; } //Can it be removed by the owner themselves? If false, they can't remove.
        public bool canModify { get; set; } //Can they change the permission flags? If false, they can't. To prevent someone losing owner access.
        public string Permissions { get; set; } //permissions. no spaces, seperated by ;, ~ means remove, * at end is wildcard. example: "roles.*;~roles.add;~roles.remove"
    }
}
