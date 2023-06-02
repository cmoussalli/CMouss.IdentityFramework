using CMouss.IdentityFramework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{
    public enum DatabaseType
    {
        MSSQL = 1,
        SQLite = 2
    }

    public enum IDGeneratorLevel
    {
        Guid32 = 1,
        Guid64 = 2,
        Guid96 = 3,
        Guid128 = 4
    }

    public enum DBLifeCycle
    {
        /// <summary>
        /// Instantiate the IdentityFrameworkDBContext when DBManager is configured and keep the context alive in DBManager (static class) to maintain easy access and fast response.
        /// </summary>
        InMemoryOnly = 1
            ,
        /// <summary>
        /// Don't keep a living instance of IdentityDBContext, developer have to handle it by Instantiate the context when needed.
        /// </summary>
        OnRequestOnly = 2
            ,
        Both = 3
    }

    public class LifeTime
    {
        public int Minutes { get; set; } = 0;
        public int Hours { get; set; } = 0;
        public int Days { get; set; } = 0;

        public LifeTime(int days, int hours, int minutes)
        {
            Days = days;
            Hours = hours;
            Minutes = minutes;
        }
    }

    public partial class IDFManagerConfig
    {
        public IDGeneratorLevel IDGeneratorLevel { get; set; } = IDGeneratorLevel.Guid32;
        public string AdministratorRoleName { get; set; } = "Administrators";
        public string AdministratorRoleId { get; set; } = "Administrators";
        public string AdministratorUserName { get; set; } = "Administrator";
        public string AdministratorPassword { get; set; } = "P@ssw0rd";
        public string DBConnectionString { get; set; }
        public int DefaultListPageSize { get; set; } = 10;
        public DatabaseType DatabaseType { get; set; }
        public DBLifeCycle DBLifeCycle { get; set; } = DBLifeCycle.Both;
        public bool IsActiveByDefault { get; set; } = true;
        public bool IsLockedByDefault { get; set; } = true;
        public bool MaintainUniqueAppTitleAcrossAllUsers { get; set; } = false;
        public LifeTime DefaultTokenLifeTime { get; set; } = new LifeTime(0, 0, 1);
        public LifeTime DefaultAppAccessLifeTime { get; set; } = new LifeTime(0, 0, 1);
    }

    public static partial class IDFManager
    {
        #region Props

        static string administratorRoleName = "Administrators";
        public static string AdministratorRoleName
        {
            get
            {
                return administratorRoleName;
            }
        }

        static string administratorRoleId = "Administrators";
        public static string AdministratorRoleId
        {
            get
            {
                return administratorRoleId;
            }
        }

        static string administratorUserName = "Administrator";
        public static string AdministratorUserName
        {
            get
            {
                return administratorUserName;
            }
        }

        static string administratorPassword = "P@ssw0rd";
        public static string AdministratorPassword
        {
            get
            {
                return administratorPassword;
            }
        }


        static string connectionString = "";
        public static string ConnectionString
        {
            get
            {
                return connectionString;
            }
        }

        static int defaultListPageSize = 10;
        public static int DefaultListPageSize
        {
            get
            {
                return defaultListPageSize;
            }
        }

        static DatabaseType databaseType = DatabaseType.SQLite;
        public static DatabaseType DatabaseType
        {
            get
            {
                return databaseType;
            }
        }

        static DBLifeCycle dBLifeCycle = DBLifeCycle.Both;
        public static DBLifeCycle DBLifeCycle
        {
            get
            {
                return dBLifeCycle;
            }
        }

        static bool isActiveByDefault = true;
        public static bool IsActiveByDefault { get { return isActiveByDefault; } }

        static bool isLockedByDefault = true;
        public static bool IsLockedByDefault { get { return isLockedByDefault; } }

        static bool maintainUniqueAppTitleAcrossAllUsers = false;
        public static bool MaintainUniqueAppTitleAcrossAllUsers { get { return maintainUniqueAppTitleAcrossAllUsers; } }

        static LifeTime tokenDefaultLifeTime;
        public static LifeTime TokenDefaultLifeTime { get { return tokenDefaultLifeTime; } }

        static LifeTime appAccessDefaultLifeTime;
        public static LifeTime AppAccessDefaultLifeTime { get { return tokenDefaultLifeTime; } }

        static IDGeneratorLevel iDGeneratorLevel;
        public static IDGeneratorLevel IDGeneratorLevel { get { return iDGeneratorLevel; } }


        static IDFDBContext IDFDBContext;
        public static IDFDBContext Context
        {
            set
            {
                IDFDBContext = value;
            }
            get
            {
                return IDFDBContext;
            }
        }

        #endregion


        public static void Configure(IDFManagerConfig config)
        {
            databaseType = config.DatabaseType;
            connectionString = config.DBConnectionString;
            defaultListPageSize = config.DefaultListPageSize;
            dBLifeCycle = config.DBLifeCycle;
            isActiveByDefault = config.IsActiveByDefault;
            isLockedByDefault = config.IsLockedByDefault;
            maintainUniqueAppTitleAcrossAllUsers = config.MaintainUniqueAppTitleAcrossAllUsers;
            appAccessDefaultLifeTime = config.DefaultAppAccessLifeTime;
            tokenDefaultLifeTime = config.DefaultTokenLifeTime;
            administratorRoleName = config.AdministratorRoleName;
            administratorUserName = config.AdministratorUserName;
            administratorPassword = config.AdministratorPassword;
            iDGeneratorLevel = config.IDGeneratorLevel;

            if (dBLifeCycle != DBLifeCycle.OnRequestOnly)
            {
                //Create Static Context
                IDFDBContext = new IDFDBContext();

            }

            //////Add Default Records
            ////Create Administrator Role
            //IDFDBContext db = new IDFDBContext();
            //db.Database.EnsureCreated();
            //List<Role> roles = db.Roles.Where(o => o.Title.ToLower() == administratorRoleName.ToLower()).ToList();
            //if (roles.Count == 0)
            //{
            //    RoleServices.Create(administratorRoleId, administratorRoleName);
            //}

            ////Create Admin User
            //List<User> users = db.Users.Where(o => o.UserName.ToLower() == administratorUserName.ToLower()).ToList();
            //if (users.Count == 0)
            //{
            //    string adminUserID= UserServices.Create( administratorUserName, "P@ssw0rd", administratorUserName, administratorUserName + "@mail.com");
            //    UserServices.GrantRole(adminUserID, administratorRoleId);

            //}

        }


        public static void CreateAdministrator()
        {
            IDFDBContext db = new IDFDBContext();
            List<Role> roles = db.Roles.Where(o => o.Title.ToLower() == administratorRoleName.ToLower()).ToList();
            if (roles.Count == 0)
            {
                RoleServices.Create(administratorRoleId, administratorRoleName);
            }

            //Create Admin User
            List<User> users = db.Users.Where(o => o.UserName.ToLower() == administratorUserName.ToLower()).ToList();
            if (users.Count == 0)
            {
                string adminUserID = UserServices.Create(administratorUserName, administratorPassword, administratorUserName, administratorUserName + "@mail.com");
                UserServices.GrantRole(adminUserID, administratorRoleId);

            }
        }

        public static RoleService RoleServices = new RoleService();
        public static UserService UserServices = new UserService();
        public static UserTokenService UserTokenServices = new UserTokenService();
        public static AppAccessService AppAccessServices = new AppAccessService();
        public static AuthService AuthService = new AuthService();



        public static void RefreshDBContext()
        {
            IDFDBContext = new IDFDBContext();
            RefreshDBContext_MasterData();
        }

        public static void RefreshDBContext_MasterData()
        {
            IDFManager.Context.Roles.ToList();
            IDFManager.Context.Entities.ToList();
            IDFManager.Context.PermissionTypes.ToList();
        }



    }
}
