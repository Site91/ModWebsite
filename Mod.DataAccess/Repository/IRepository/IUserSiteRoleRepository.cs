using Mod.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.DataAccess.Repository.IRepository
{
    public interface IUserSiteRoleRepository : IRepository<UserSiteRole>
    {
        public bool IsAuthorized(string userId, int siteId, string permission);
    }
}
