using Mod.DataAccess.Repository.IRepository;
using Mod.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.DataAccess.Repository
{
    public class SiteRolesRepository : Repository<SiteRoles>, ISiteRolesRepository
    {
        private ApplicationDbContext _db;
        public SiteRolesRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
