using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Utility
{
    public static class SD
    {
        public static bool AllowReports = true; //If in the future I add a console, I can temp disable reporting (in case I am having problems)

        public const string Role_User_Indi = "User";
        public const string Role_User_Dev = "Developer";
        public const string Role_JrMod = "JrModerator";
        public const string Role_Mod = "Moderator";
        public const string Role_Admin = "Admin";
        public const string Role_Manager = "Manager";

        //Roles which are addons (added to prev roles)
        public const string Role_Trusted = "Trusted";

        public enum RoleEnum
        {
            User_Indi = 0,
            User_Dev = 1,
            JrMod = 2,
            Mod = 3,
            Admin = 4,
            Manager = 5,
            Trusted = 6,
        }

        public static readonly Dictionary<RoleEnum, string[]> Roles = new Dictionary<RoleEnum, string[]>()
        {
            [RoleEnum.User_Indi] = new string[] { "User", "0" }, //0 = main roles, 1 = addon
            [RoleEnum.User_Dev] = new string[] { "Developer", "0" },
            [RoleEnum.JrMod] = new string[] { "JrModerator", "0" },
            [RoleEnum.Mod] = new string[] { "Moderator", "0" },
            [RoleEnum.Admin] = new string[] { "Admin", "0" },
            [RoleEnum.Manager] = new string[] { "Manager", "0" },
            [RoleEnum.Trusted] = new string[] { "Trusted", "1" }
        };

        public static readonly string[] DefaultInfractions = new string[]
        {
            "Other", //[0] always involves other stuff. Misc stuff.
            "Avatar Violation",//[1] always involves the avatar.
        };
        public static readonly int[] DefaultInfractionsFlags = new int[]
        {
            7,
            1
        };
    }
}
