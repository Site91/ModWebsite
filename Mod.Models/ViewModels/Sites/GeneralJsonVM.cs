using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Models.ViewModels.Sites
{
    public class GeneralJsonVM : BaseSiteVM
    {
        public string Name { get; set; }
        public string? IconPath { get; set; }
        public string? BigImgPath { get; set; }
        public string ShortDescription { get; set; }
    }
}
