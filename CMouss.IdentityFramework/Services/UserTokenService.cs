using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{
    public class UserTokenService
    {

        public UserToken? Validate(string token)
        {

            List<UserToken> lst = IDFManager.Context.UserTokens.Include(o => o.User).Where(o =>
                o.Token == token
                && o.ExpireDate >= DateTime.Now
                ).ToList();
            if (lst.Count == 0)
            {
                return null;
            }
            return lst[0];
        }

        public void Delete(string token)
        {
            List<UserToken> lst = IDFManager.Context.UserTokens.Where(o =>
              o.Token == token
              ).ToList();
            if (lst.Count == 0)
            {
                throw new Exception("Not Found");
            }
            IDFManager.Context.UserTokens.Remove(lst[0]);
        }

        public UserToken Create(string userId)
        {
            return Create(userId, IDFManager.TokenDefaultLifeTime);
        }

        public UserToken Create(string userId, LifeTime lifeTime)
        {
            UserToken o = new UserToken();
            o.Token = Helpers.GenerateKey();
            o.ExpireDate = DateTime.Now.AddDays(lifeTime.Days).AddHours(lifeTime.Hours).AddMinutes(lifeTime.Minutes);
            o.UserId = userId;
            IDFManager.Context.UserTokens.Add(o);
            IDFManager.Context.SaveChanges();
            return o;
        }


        public int CleanExpiredUserTokens()
        {
            List<UserToken> lst = IDFManager.Context.UserTokens.Where(o =>
                o.ExpireDate < DateTime.UtcNow
            ).ToList();
            int result = lst.Count;
            IDFManager.Context.UserTokens.RemoveRange(lst);
            IDFManager.Context.SaveChanges();
            return result;
        }

        public int DeleteUserTokens(string userId)
        {
            List<UserToken> lst = IDFManager.Context.UserTokens.Where(o =>
                o.UserId == userId
            ).ToList();
            int result = lst.Count;
            IDFManager.Context.UserTokens.RemoveRange(lst);
            IDFManager.Context.SaveChanges();
            return result;
        }

    }
}
