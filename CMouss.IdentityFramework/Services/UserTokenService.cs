using CMouss.IdentityFramework.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{
    public class UserTokenService
    {

        public string GetUserIdUsingUsertoken(string token, TokenValidationMode tokenValidationMode)
        {

            if (tokenValidationMode == TokenValidationMode.UseDefault)
            {
                tokenValidationMode = IDFManager.TokenValidationMode;
            }
            if (tokenValidationMode == TokenValidationMode.DecryptOnly)
            {
                try
                {
                    UserClaim claim = Helpers.DecryptUserToken(token);
                    if (claim.TokenExpireDate < DateTime.UtcNow) { return null; }
                    return claim.UserId;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {

                try
                {
                    UserToken tok = IDFManager.Context.UserTokens.First(o =>
                        o.Token == token
                        && o.ExpireDate >= DateTime.Now
                        );
                    return tok.UserId;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }


        public UserToken? Validate(string token, TokenValidationMode tokenValidationMode)
        {
            return Validate(token, tokenValidationMode, null);
        }

        public UserToken? Validate(string token, TokenValidationMode tokenValidationMode, string? ipAddress)
        {

            UserToken result = new();
            if (tokenValidationMode == TokenValidationMode.UseDefault)
            {
                tokenValidationMode = TokenValidationMode.DecryptOnly;
            }
            if (tokenValidationMode == TokenValidationMode.DecryptOnly)
            {
                try
                {
                    UserClaim claim = Helpers.DecryptUserToken(token);
                    UserToken o = new UserToken();
                    o.UserId = claim.UserId;
                    o.User = new User() { Email = claim.EMail, FullName = claim.UserFullName, UserName = claim.UserName, Roles = Helpers.GetUserClaimRoles(claim), Id = o.UserId, LastIPAddress = claim.IPAddress };

                    result = o;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
                List<UserToken> lst = IDFManager.Context.UserTokens.Include(o => o.User).Where(o =>
                o.Token == token
                && o.ExpireDate >= DateTime.Now
                ).ToList();
                if (lst.Count == 0)
                {
                    return null;
                }
                result = lst[0];


                if (IDFManager.AllowUserMultipleSessions)
                {
                    if (!lst[0].User.LastIPAddress.Contains(ipAddress))
                    {
                        lst[0].User.LastIPAddress = ipAddress;
                        IDFManager.Context.SaveChanges();
                    }

                }
                else
                {
                    if (ipAddress is not null)
                    {
                        if (!lst[0].User.LastIPAddress.Contains(ipAddress))
                        {
                            List<UserToken> killTokens = IDFManager.Context.UserTokens.Where(o => o.UserId == lst[0].UserId && o.IPAddress.ToLower() != ipAddress.ToLower()).ToList();
                            IDFManager.Context.RemoveRange(killTokens);
                            lst[0].User.LastIPAddress = ipAddress;
                            IDFManager.Context.SaveChanges();
                        }
                    }

                }


            }


            return result;
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
            return Create(userId, IDFManager.TokenDefaultLifeTime, "");
        }
        public UserToken Create(string userId, string ipAddress)
        {
            return Create(userId, IDFManager.TokenDefaultLifeTime, ipAddress);
        }

        public UserToken Create(string userId, LifeTime lifeTime, string ipAddress)
        {
            UserToken o = new UserToken();
            User user = IDFManager.Context.Users.Include("Roles").First(o => o.Id == userId);
            if (user.IsDeleted == true) { throw new Exception("User is deleted"); }
            if (user.IsLocked == true) { throw new Exception("User is locked"); }
            string x = JsonSerializer.Serialize(Helpers.GenerateUserClaim(user));
            o.Token = Helpers.Encrypt(x, IDFManager.TokenEncryptionKey);

            o.ExpireDate = DateTime.Now.AddDays(lifeTime.Days).AddHours(lifeTime.Hours).AddMinutes(lifeTime.Minutes);
            o.UserId = userId;
            o.IPAddress = ipAddress;
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
