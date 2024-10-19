using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{
    public static partial class IDFManager
    {

        public static class UserSessionsManager
        {

            public static void AddOrUpdate(string userId, string? ipAddress)
            {
                UserSession session = IDFManager.UserSessions.FirstOrDefault(o => o.UserId.ToLower() == userId.ToLower());
                if (session is null //New Session 
                    || session is not null && IDFManager.AllowUserMultipleSessions) //Existing Session but Multiple Session is Allowed)
                {
                    //Add Session
                    UserSession s = new();
                    s.UserId = userId;
                    s.CreateDate = DateTime.Now;
                    s.LastConnection = DateTime.Now;
                    s.ExpireDate = DateTime.Now + IDFManager.TokenDefaultLifeTime.ToTimeSpan();
                    if (!string.IsNullOrEmpty(ipAddress))
                    {
                        if (ipAddress.Length > 7)
                        {
                            s.IPAddress = ipAddress;
                        }
                    }

                    IDFManager.UserSessions.Add(s);

                }
                else
                {
                    if (session.IPAddress != ipAddress)
                    {
                        session.IPAddress = ipAddress;
                    }
                }

            }



            public static bool ValidateUserSession(string userId, string ipAddress)
            {
                if (IDFManager.AllowUserMultipleSessions)
                {
                    throw new Exception("AllowUserMultipleSessions is true.");
                }
                UserSession session = IDFManager.UserSessions.Find(o => o.UserId.ToLower() == userId.ToLower() && o.IPAddress == ipAddress);
                if (session is null)
                {
                    User user = IDFManager.userService.Details(userId);
                    if (user.LastIPAddress == ipAddress && !user.IsDeleted)
                    {
                        AddOrUpdate(userId, ipAddress);
                        return true;
                    }

                    return false;
                }
                if (session.ExpireDate < DateTime.Now)
                {
                    return false;
                }


                session.LastConnection = DateTime.Now;
                return true;
            }





        }
    }
}
