using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mod.Models;

namespace Mod.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Creator> ApplicationUsers { get; set; }
        public DbSet<AuthorizedSites> AuthorizedSites { get; set; }
        public DbSet<SiteRoles> SiteRoles { get; set; }
        public DbSet<UserSiteRole> UserSiteRole { get; set; }
    }
}
