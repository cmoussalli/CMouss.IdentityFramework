using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CMouss.IdentityFramework
{
    public class AppAccessService
    {

        public List<AppAccess> GetAll(string? appId, string? userId)
        {
            return IDFManager.Context.AppAccess.Include(o => o.App).ThenInclude(o => o.Owner).Where(o =>
            o.AppId.ToLower() == appId.ToLower()
            || o.UserId.ToLower() == userId.ToLower()
            ).ToList();
        }

        public AppAccess? Validate(string appKey, string appSecret)
        {
            List<AppAccess> lst = IDFManager.Context.AppAccess
                .Include(o => o.App).ThenInclude(o => o.Owner)
                .Include(o => o.AppAccessPermissions)
                .Where(o =>
                    o.AccessKey == appKey
                    && o.AccessSecret == appSecret
                    && o.ExpireDate >= DateTime.UtcNow
                ).ToList();

            if (lst.Count == 0)
            {
                return null;
            }

            return lst[0];
        }


        public void Delete(string appKey, string appSecret)
        {
            List<AppAccess> lst = IDFManager.Context.AppAccess.Where(o =>
              o.AccessKey == appKey
              && o.AccessSecret == appSecret
              && o.ExpireDate >= DateTime.UtcNow
              ).ToList();
            if (lst.Count == 0)
            {
                throw new NotFoundException();
            }
            IDFManager.Context.AppAccess.Remove(lst[0]);
            IDFManager.Context.SaveChanges();
        }

        public AppAccess Generate(string userId, string appId)
        {
            return Generate(userId, appId, IDFManager.AppAccessDefaultLifeTime);
        }
        public AppAccess Generate(string userId, string appId, LifeTime lifeTime)
        {
            AppAccess o = new AppAccess();
            o.Id = Helpers.GenerateId();
            o.AccessKey = Helpers.GenerateKey();
            o.AccessSecret = Helpers.GenerateKey();
            o.UserId = userId;
            o.AppId = appId;
            o.ExpireDate = DateTime.Now.AddDays(lifeTime.Days).AddHours(lifeTime.Hours).AddMinutes(lifeTime.Minutes);
            IDFManager.Context.AppAccess.Add(o);
            IDFManager.Context.SaveChanges();
            return o;
        }

        public void RefreshAccessSecret(string userId, string appId, LifeTime lifeTime)
        {
            List<AppAccess> lst = IDFManager.Context.AppAccess.Where(o =>
            o.UserId == userId
            && o.AppId == appId
            ).ToList();
            if (lst.Count == 0)
            {
                throw new NotFoundException();
            }
            lst[0].AccessSecret = Helpers.GenerateKey();
            lst[0].ExpireDate = DateTime.Now.AddDays(lifeTime.Days).AddHours(lifeTime.Hours).AddMinutes(lifeTime.Minutes);
            IDFManager.Context.SaveChanges();
        }

        public int CleanExpiredAppAccesss()
        {
            List<AppAccess> lst = IDFManager.Context.AppAccess.Where(o =>
                o.ExpireDate < DateTime.UtcNow
            ).ToList();
            int result = lst.Count;
            IDFManager.Context.AppAccess.RemoveRange(lst);
            IDFManager.Context.SaveChanges();
            return result;
        }


        #region GetAppPermissionTypes
        public List<AppPermissionType> GetAppPermissionTypes(string appAccessId)
        {
            List<AppPermissionType> result = new();
            List<AppAccessPermission> appAccessPermissions = IDFManager.Context.AppAccessPermissions.Where(o =>
                o.AppAccessId == appAccessId
            ).ToList();

            result.AddRange(appAccessPermissions.Select(o => o.AppPermissionType));

            return result;
        }
        public List<AppPermissionType> GetAppPermissionTypes(string appAccessKey, string appAccessSecret)
        {
            List<AppPermissionType> result = new();
            List<AppAccessPermission> appAccessPermissions = IDFManager.Context.AppAccessPermissions.Where(o =>
                o.AppAccess.AccessKey == appAccessKey
                && o.AppAccess.AccessSecret == appAccessSecret
            ).ToList();

            result.AddRange(appAccessPermissions.Select(o => o.AppPermissionType));

            return result;
        }
        #endregion

        #region Grant Permission
        public void GrantPermission(string appAccessid, string appPermissionTypeId)
        {
            //Validate AppAccess
            List<AppAccess> appAccesses = IDFManager.Context.AppAccess.Include(o => o.AppAccessPermissions).Where(o => o.Id.ToLower() == appAccessid.ToLower()).ToList();
            if (appAccesses.Count == 0) { throw new AppAccessNotFoundException() ; }
            //Valdiate AppPermissionType  
            List<AppPermissionType> appPermissionTypes = IDFManager.Context.AppPermissionTypes.Where(o => o.Id.ToLower() == appPermissionTypeId.ToLower()).ToList();
            if (appPermissionTypes.Count == 0) { throw new AppPermissionTypeNotFoundException (); }

            if (appAccesses[0].AppAccessPermissions.Where(o => o.AppAccessId.ToLower() == appAccessid.ToLower() && appPermissionTypeId.ToLower() == o.AppPermissionTypeId.ToLower()).Count() > 0)
            {
                throw new AlreadyExistException();
            }

            AppAccessPermission appAccessPermission = new();
            appAccessPermission.Id = Helpers.GenerateId();
            appAccessPermission.AppAccessId = appAccessid;
            appAccessPermission.AppPermissionTypeId = appPermissionTypeId;
            IDFManager.Context.AppAccessPermissions.Add(appAccessPermission);
            IDFManager.Context.SaveChanges();
        }
        #endregion


        #region Revoke Permission
        public void RevokePermission(string appAccessid, string appPermissionTypeId)
        {
            ////Validate AppAccess
            //List<AppAccess> appAccesses = IDFManager.Context.AppAccess.Include(o => o.AppAccessPermissions).Where(o => o.Id.ToLower() == appAccessid.ToLower()).ToList();
            //if (appAccesses.Count == 0) { throw new Exception(Messages.AppAccessNotFound); }
            ////Valdiate AppPermissionType  
            //List<AppPermissionType> appPermissionTypes = IDFManager.Context.AppPermissionTypes.Where(o => o.Id.ToLower() == appPermissionTypeId.ToLower()).ToList();
            //if (appPermissionTypes.Count == 0) { throw new Exception(Messages.AppAccessNotFound); }

            List<AppAccessPermission> appAccessPermissions = IDFManager.Context.AppAccessPermissions.Where(o =>
                o.AppAccessId.ToLower() == appAccessid.ToLower()
                && o.AppPermissionTypeId.ToLower() == appPermissionTypeId.ToLower()
            ).ToList();
            if (appAccessPermissions.Count == 0)
            {
                throw new NotFoundException();
            }
            foreach (AppAccessPermission a in appAccessPermissions)
            {
                IDFManager.Context.AppAccessPermissions.Remove(a);
                IDFManager.Context.SaveChanges();
            }

        }
        #endregion

        #region ValidateAppAccessPermission
        public bool ValidateAppAccessPermission(string appAccessId, string appPermissionTypeId)
        {
            bool result = false;
            List<AppAccessPermission> appAccessPermissions = IDFManager.Context.AppAccessPermissions.Where(o =>
                o.AppAccessId.ToLower() == appAccessId.ToLower()
                && o.AppPermissionTypeId.ToLower() == appPermissionTypeId.ToLower()
            ).ToList();

            if (appAccessPermissions.Count > 0)
            {
                result = true;
            }
            return result;
        }

        #endregion

        #region ValidateAppAccessPermission
        public bool ValidateAppAccessPermission(string appAccessKey, string appAccessSecret, string appPermissionTypeId)
        {
            bool result = false;

            List<AppAccessPermission> appAccessPermissions = IDFManager.Context.AppAccessPermissions.Where(o =>
                o.AppAccess.AccessKey.ToLower() == appAccessKey.ToLower()
                && o.AppAccess.AccessSecret.ToLower() == appAccessSecret.ToLower()
                //&& o.AppPermissionTypeId.ToLower() == appPermissionTypeId.ToLower()
            ).ToList();

            if (appAccessPermissions.Count < 1)
            {
                throw new InvalidAppAccessKeyOrSecretException();
            }
            if (appAccessPermissions.Any(o => o.AppPermissionTypeId.ToLower() == appPermissionTypeId.ToLower()))
            {
                result = true;
            }
            return result;
        }

        #endregion


    }
}
