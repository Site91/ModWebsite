using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Mod.Models.ViewModels.Sites
{
    public class SiteIndexVM
    {
        public AuthorizedSites SiteData { get; set; }
        public GeneralJsonVM SiteObj { get; set; }
        public string? HTMLPath { get; set; } //If a custom html is required...

    }
}
