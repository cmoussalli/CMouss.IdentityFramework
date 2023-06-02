using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{
    public partial class IDFDBContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (IDFManager.DatabaseType == DatabaseType.SQLite)
            {
                options.UseSqlite(IDFManager.ConnectionString);//"Data Source=App_Data/OpenWaaS.db");
            }

            if (IDFManager.DatabaseType == DatabaseType.MSSQL)
            {
                options.UseSqlServer(IDFManager.ConnectionString);//"Data Source=App_Data/OpenWaaS.db");
            }



        }


        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<AppAccess> AppAccess { get; set; }

        public DbSet<AppPermissionType> AppPermissionTypes { get; set; }
        public DbSet<AppAccessPermission> AppAccessPermissions { get; set; }

        //public DbSet<RoleUser> RoleUser { get; set; }

        public DbSet<PermissionType> PermissionTypes { get; set; }
        public DbSet<Entity> Entities { get; set; }
        public DbSet<App> Apps { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        public DbSet<AttributeType> AttributeTypes { get; set; }
        public DbSet<AttributeItem> AttributeItems { get; set; }

        public DbSet<UserToken> UserTokens { get; set; }

        //public DbSet<RoleUser> RoleUser { get; set; }

        public void InsertMasterData()
        {
            //Add Default Records
            string adminRoleID = Helpers.GenerateId();
            //string adminUserID = Helpers.GenerateId();
            if(!string.IsNullOrEmpty( IDFManager.AdministratorRoleId) )
                { adminRoleID = IDFManager.AdministratorRoleId; }
            //Create Administrator Role
            IDFDBContext db = new IDFDBContext();
            List<Role> roles = db.Roles.Where(o => o.Title.ToLower() == IDFManager.AdministratorRoleName.ToLower()).ToList();
            if (roles.Count == 0)
            {
                IDFManager.RoleServices.Create(adminRoleID, IDFManager.AdministratorRoleName);
            }

            //Create Admin User
            List<User> users = db.Users.Where(o => o.UserName.ToLower() == IDFManager.AdministratorUserName.ToLower()).ToList();
            if (users.Count == 0)
            {
                string adminUserID = IDFManager.UserServices.Create(IDFManager.AdministratorUserName, IDFManager.AdministratorPassword, IDFManager.AdministratorUserName, IDFManager.AdministratorUserName + "@mail.com",false,true);
                IDFManager.UserServices.GrantRole(adminUserID, adminRoleID);

            }
        }


        public void CloseConnection()
        {
            this.CloseConnection();
        }

    }
}
