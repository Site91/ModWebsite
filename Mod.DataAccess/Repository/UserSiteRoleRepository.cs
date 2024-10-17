using Microsoft.EntityFrameworkCore;
using Mod.DataAccess.Repository.IRepository;
using Mod.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.DataAccess.Repository
{
    public class UserSiteRoleRepository : Repository<UserSiteRole>, IUserSiteRoleRepository
    {
        private ApplicationDbContext _db;
        public UserSiteRoleRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public bool IsAuthorized(string userId, int siteId, string permission)
        {
            IQueryable<UserSiteRole> query = dbSet;
            query.Include("Role");
            var allRoles = query.Where(u=>u.UserId == userId && u.Role.SiteId == siteId);
            int allowed = 1; //if they can do something. 2 is allowed, 1 = not, 0 = negative detected (~), so must be denied on this pass.
            var curPermSplit = permission.Split('.');
            //Prep the curPermSplit list (all possible wildcards & perms)
            for(int i = curPermSplit.Length - 1; i >= 0; i--)
            {
                string str = "";
                for (int j = 0; j <= i; j++)
                {
                    str += curPermSplit[j] + ".";
                }
                if (i != curPermSplit.Length - 1)
                    str += '*';
                else
                    str = str.Substring(0, str.Length - 1);
                curPermSplit[i] = str;
            }
            var negPermSplit = curPermSplit.Select(u => "~" + u); //the negative version of it.
            //start checking roles
            foreach(var e in allRoles)
            {
                allowed = 1;
                var permList = e.Role.Permissions.Split(';');
                if (permList.Contains("all")) //They got em ALL BABY!
                    allowed = 2; //now search for anything with ~ that denies it.
                foreach(var pass in permList)
                {
                    if (negPermSplit.Contains(pass))
                    {
                        allowed = 0;
                        break;
                    }
                    if (curPermSplit.Contains(pass))
                    {
                        allowed = 2;
                    }
                }
                if(allowed == 2)
                {
                    return true;
                }
            }
            return allowed == 2;
        }
    }
}
