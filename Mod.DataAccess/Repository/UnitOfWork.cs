using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mod.DataAccess.Repository.IRepository;

namespace Mod.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            User = new CreatorRepository(_db);
            AuthorizedSites = new AuthorizedSitesRepository(_db);
        }
        public ICreatorRepository User { get; private set; }
        public IAuthorizedSitesRepository AuthorizedSites { get; private set; }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
