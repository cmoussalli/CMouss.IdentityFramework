using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CMouss.IdentityFramework
{
    public class AppService
    {


        public App? Details(string id)
        {
            List<App> lst = IDFManager.Context.Apps
                .Include(o => o.Owner)
                .Where(o => o.Id == id && o.IsActive).ToList();
            if (lst.Count == 0)
            {
                throw new NotFoundException();
            }
            return lst[0];
        }

        public List<App> GetAll(bool includeDeleted)
        {
            if (includeDeleted)
            {
                return IDFManager.Context.Apps.ToList();
            }
            else
            {
                return IDFManager.Context.Apps.Where(o => o.IsDeleted == false).ToList();
            }
        }
        public List<App> GetAll(string userId, bool includeDeleted)
        {
            return IDFManager.Context.Apps.Where(o =>
            o.OwnerId.ToLower() == userId.ToLower()
            && (includeDeleted == true || o.IsDeleted == false)
            ).ToList();
        }

        public List<App> Search(Paging paging, AppsSearch appsSearch, UsersSearch ownerSearch)
        {
            return IDFManager.Context.Apps.Where(o =>
                (String.IsNullOrEmpty(appsSearch.Title) || o.Title.ToLower().Contains(appsSearch.Title.ToLower()))

                && (appsSearch.IsActive == null || appsSearch.IsActive == o.IsActive)
               && (appsSearch.IsDeleted == null || appsSearch.IsDeleted == o.IsDeleted)

                ).Skip(paging.Skip).Take(paging.PageSize).ToList();
        }


        public void Create(string id, string title, string ownerId)
        {
            List<App> lst = IDFManager.Context.Apps.Where(o =>
                o.Id.ToLower() == id.ToLower()
                || o.Title.ToLower() == title.ToLower()
                ).ToList(); ;
            if (lst.Count > 0)
            {
                if (lst[0].Id.ToLower() == id.ToLower()) { throw new Exception("Id already exists"); }
                if (IDFManager.MaintainUniqueAppTitleAcrossAllUsers)
                {
                    if (lst[0].Title.ToLower() == title.ToLower()) { throw new Exception("Username already exists"); }

                }
            }
            App o = new App();
            o.Id = id;
            o.Title = title;
            o.IsActive = false;
            o.IsDeleted = false;
            o.OwnerId = ownerId;
            IDFManager.Context.Apps.Add(o);
            IDFManager.Context.SaveChanges();

        }

        public void Update(string id, string title, bool isActive)
        {
            List<App> lst = IDFManager.Context.Apps.Where(o => o.Id == id).ToList();
            if (lst.Count > 0)
            {
                lst[0].Title = title;
                lst[0].IsActive = isActive;
            }
            IDFManager.Context.SaveChanges();
        }

        public void Delete(string id)
        {
            List<App> lst = IDFManager.Context.Apps.Where(o => o.Id == id).ToList();
            //TODO: Delete App
            IDFManager.Context.SaveChanges();
        }


        //public AppAccess AppLogin(string accessKey, string accessSecret)
        //{
        //    AppAccess appAccess = new();
        //    List<AppAccess> o = IDFManager.Context.AppAccess
        //            .Include(o => o.App).ThenInclude(o => o.Owner)
        //            .Include(o => o.AppAccessPermissions)
        //        .Where(o =>
        //            o.AccessKey.ToLower() == accessKey.ToLower()
        //            && o.AccessSecret.ToLower() == accessSecret.ToLower()
        //            && o.ExpireDate > DateTime.Now
        //        ).ToList();
        //    if (o == null)
        //    {
        //        throw new NotFoundException();
        //    }
        //    if (o.Count == 0)
        //    {
        //        throw new NotFoundException();
        //    }
        //    appAccess = o[0];

        //    return appAccess;
        //}

       

    }
}
