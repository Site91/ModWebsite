using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Mod.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }
        public void Initialize()
        {
            //migrations if not applied
            try
            {
                if(_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
                //Create folder
                string dir = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.FullName;
                Directory.CreateDirectory(dir + "/ServerInfo");
            }
            catch (Exception ex)
            {

            }

            //create roles if not created
            //if (_roleManager.Roles.Count() < SD.Roles.Count())
            //{
            //    foreach (var role in SD.Roles)
            //    {
            //        if (!_roleManager.RoleExistsAsync(role.Value[0]).GetAwaiter().GetResult())
            //            _roleManager.CreateAsync(new IdentityRole(role.Value[0])).GetAwaiter().GetResult();
            //    }
            //}
        }
    }
}
