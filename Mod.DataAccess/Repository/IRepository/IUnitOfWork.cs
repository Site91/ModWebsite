﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        //ICategoryRepository Category { get; }
        //ICoverTypeRepository CoverType { get; }
        //IProductRepository Product { get; }
        //ICompanyRepository Company { get; }
        ICreatorRepository User { get; }
        IAuthorizedSitesRepository AuthorizedSites { get; }
        ISiteRolesRepository SiteRoles { get; }
        IUserSiteRoleRepository UserSiteRole { get; }

        void Save();
    }
}
