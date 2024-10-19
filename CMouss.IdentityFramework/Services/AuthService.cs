using CMouss.IdentityFramework.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{
    public class AuthService
    {

        #region User Login -1
        public AuthResult AuthUserLogin(string user, string? password)
        {
            return AuthUserLogin(user, password, "");
        }

        public AuthResult AuthUserLogin(string user, string password, string? ipAddress = null)
        {
            AuthResult result = new();
            List<Role> roles = IDFManager.Context.Roles.ToList();

            UserToken t = new UserToken();
            List<User> users = IDFManager.Context.Users
                .Include(o => o.Apps).Include(o => o.Roles)
                .Where(o => o.UserName.ToLower() == user.ToLower() && o.IsDeleted == false).ToList();
            if (users == null)
            {
                result.AuthenticationMode = IDFAuthenticationMode.User;
                result.SecurityValidationResult = SecurityValidationResult.IncorrectCredentials;
                return result;
            }
            if (users.Count == 0)
            {
                //throw new Exception(Messages.UserNotFound);
                result.AuthenticationMode = IDFAuthenticationMode.User;
                result.SecurityValidationResult = SecurityValidationResult.IncorrectCredentials;
                return result;
            }

            //Validate password
            if (password != Helpers.Decrypt(users[0].Password, users[0].PrivateKey))
            {
                //throw new Exception(Messages.IncorrectPassword);
                result.AuthenticationMode = IDFAuthenticationMode.User;
                result.SecurityValidationResult = SecurityValidationResult.IncorrectCredentials;
                return result;
            }

            t = IDFManager.UserTokenServices.Create(users[0].Id, ipAddress);
            IDFManager.UserSessionsManager.AddOrUpdate(users[0].Id, ipAddress);

            if (IDFManager.AllowUserMultipleSessions)
            {//Multiple Session by IP is Allowed
                t.User.LastIPAddress = ipAddress;
                IDFManager.Context.SaveChanges();
            }
            else
            {//Multiple Session by IP is Not Allowed
                //Validate IP Address
                if (!string.IsNullOrEmpty(t.User.LastIPAddress))
                {
                    if (t.User.LastIPAddress.ToLower() != ipAddress.ToLower())
                    {// Changed IP Address
                        List<UserToken> expiredTokens = IDFManager.Context.UserTokens.Where(o => o.IPAddress != ipAddress && o.UserId.ToLower() == users[0].Id).ToList();
                        IDFManager.Context.UserTokens.RemoveRange(expiredTokens);
                        t.User.LastIPAddress = ipAddress;
                        IDFManager.Context.SaveChanges();
                    }
                    else
                    {
                        t.User.LastIPAddress = ipAddress;
                        IDFManager.Context.SaveChanges();
                    }

                }



            }


            result = new(t);
            return result;
        }



        #endregion

        #region Authenticate UserToken -1
        public AuthResult AuthUserToken(string token, TokenValidationMode tokenValidationMode)
        {
            return AuthUserToken(token, tokenValidationMode, null);
        }

        public AuthResult AuthUserToken(string token, TokenValidationMode tokenValidationMode, string? ipAddress)
        {
            AuthResult result = new();
            result.AuthenticationMode = IDFAuthenticationMode.User;

            if (tokenValidationMode == TokenValidationMode.UseDefault)
            {
                tokenValidationMode = IDFManager.TokenValidationMode;
            }

            if (IDFManager.TokenValidationMode == TokenValidationMode.DecryptOnly)
            {
                try
                {
                    UserClaim claim = Helpers.DecryptUserToken(token);

                    result = claim.ToAuthResult();
                }
                catch (Exception ex)
                {
                    result.SecurityValidationResult = SecurityValidationResult.IncorrectToken;
                    return result;
                }



            }
            else
            {
                bool validation = false;
                List<UserToken> ts = IDFManager.Context.UserTokens
                    .Include(t => t.User).Include(o => o.User.Apps).Include(u => u.User.Roles).ThenInclude(r => r.Permissions)
                    .Where(o => o.Token.ToLower() == token.ToLower()).ToList();
                if (ts is null)
                {
                    result.AuthenticationMode = IDFAuthenticationMode.User;
                    result.SecurityValidationResult = SecurityValidationResult.IncorrectToken;
                    return result;
                }

                if (ts.Count < 1)
                {
                    result.AuthenticationMode = IDFAuthenticationMode.User;
                    result.SecurityValidationResult = SecurityValidationResult.IncorrectToken;
                    return result;
                }



                result.UserToken = ts[0];
                result.SecurityValidationResult = SecurityValidationResult.Ok;

            }
            return result;
        }
        #endregion



        #region Authenticate UserToken Role(s) -2

        /// <summary>
        /// Authenticate using User info only (UserToken). App authentication is not supported.
        /// Authorization is App based, multiple App is not supported with this function.
        /// </summary>
        /// <param name="token">UserToken for Authentication.</param>
        /// <param name="roleId">The Role which user should have access to.</param>

        /// <returns>Authentication & Authorization result</returns>
        /// <exception cref="InvalidTokenException"></exception>
        public AuthResult AuthUserTokenWithRole(string token, string roleId)
        {
            return AuthUserTokenWithRole(token, roleId);
        }


        /// <summary>
        /// Authenticate using User info only (UserToken). App authentication is not supported.
        /// Authorization is App based, multiple App is not supported with this function.
        /// </summary>
        /// <param name="token">UserToken for Authentication.</param>
        /// <param name="roleId">The Role which user should have access to.</param>
        /// <param name="tokenValidationMode">Set the validation mode to DecryptOnly if you want to validate the token only by decrypting it, or decrypt and validate the token using database</param>"
        /// <returns>Authentication & Authorization result</returns>
        /// <exception cref="InvalidTokenException"></exception>
        public AuthResult AuthUserTokenWithRole(string token, string roleId, TokenValidationMode tokenValidationMode)
        {
            AuthResult result = new();
            result.AuthenticationMode = IDFAuthenticationMode.User;
            bool validation = false;

            if (tokenValidationMode == TokenValidationMode.UseDefault)
            {
                tokenValidationMode = IDFManager.TokenValidationMode;
            }

            if (tokenValidationMode == TokenValidationMode.DecryptOnly)
            {
                try
                {
                    UserClaim claim = Helpers.DecryptUserToken(token);
                    List<Role> roles = Storage.Roles.Where(o => o.Id.ToLower() == roleId.ToLower()).ToList();
                    if (roles.Count == 0)
                    {
                        result.SecurityValidationResult = SecurityValidationResult.IncorrectToken;
                        return result;
                    }
                    result = claim.ToAuthResult();
                }
                catch (Exception ex)
                {
                    result.SecurityValidationResult = SecurityValidationResult.IncorrectToken;
                    return result;
                }
            }
            else
            {


                UserToken userToken = IDFManager.UserTokenServices.Validate(token, IDFManager.TokenValidationMode, null);
                if (userToken is null)
                {
                    result.SecurityValidationResult = SecurityValidationResult.IncorrectToken;
                    return result;
                }

                result.UserToken = userToken;
                validation = IDFManager.Context.Roles.Include(r => r.Users).Any(o => o.Id == roleId && o.Users.Any(u => u.Id == userToken.UserId));

                if (!validation)
                {
                    result.SecurityValidationResult = SecurityValidationResult.UnAuthorized;
                    return result;
                }

            }
            result.SecurityValidationResult = SecurityValidationResult.Ok;
            return result;
        }





        /// <summary>
        /// Authenticate using User info only (UserToken). App authentication is not supported.
        /// Authorization is App based, multiple App is supported with this function.
        /// </summary>
        /// <param name="token">UserToken for Authentication</param>
        /// <param name="roleIds">List of the required assigned Roles, having 1 atleast will authorize the user.</param>
        /// <returns>Authentication & Authorization result</returns>
        public AuthResult AuthUserTokenWithRoles(string token, List<string> roleIds)
        {
            return AuthUserTokenWithRoles(token, roleIds, TokenValidationMode.UseDefault);
        }

        /// <summary>
        /// Authenticate using User info only (UserToken). App authentication is not supported.
        /// Authorization is App based, multiple App is supported with this function.
        /// </summary>
        /// <param name="token">UserToken for Authentication</param>
        /// <param name="roleIds">List of the required assigned Roles, having 1 atleast will authorize the user.</param>
        /// <param name="tokenValidationMode">Set the validation mode to DecryptOnly if you want to validate the token only by decrypting it, or decrypt and validate the token using database</param>
        /// <returns>Authentication & Authorization result</returns>
        public AuthResult AuthUserTokenWithRoles(string token, List<string> roleIds, TokenValidationMode tokenValidationMode)
        {
            AuthResult result = new();
            result.AuthenticationMode = IDFAuthenticationMode.User;
            bool validation = false;

            if (tokenValidationMode == TokenValidationMode.UseDefault)
            {
                tokenValidationMode = IDFManager.TokenValidationMode;
            }

            if (tokenValidationMode == TokenValidationMode.DecryptOnly)
            {
                try
                {
                    UserClaim claim = Helpers.DecryptUserToken(token);
                    List<Role> roles = Storage.Roles.Where(o => roleIds.Contains(o.Id.ToLower())).ToList();
                    if (roles.Count == 0)
                    {
                        result.SecurityValidationResult = SecurityValidationResult.IncorrectToken;
                        return result;
                    }
                    result = claim.ToAuthResult();
                }
                catch (Exception ex)
                {
                    result.SecurityValidationResult = SecurityValidationResult.IncorrectToken;
                    return result;
                }
            }
            else
            {
                UserToken userToken = IDFManager.UserTokenServices.Validate(token, TokenValidationMode.UseDefault, "");
                if (userToken is null)
                {
                    result.SecurityValidationResult = SecurityValidationResult.IncorrectToken;
                    return result;
                }

                result.UserToken = userToken;
                validation = IDFManager.Context.Roles.Include(r => r.Users).Any(o => roleIds.Contains(o.Id) && o.Users.Any(u => u.Id == userToken.UserId));


                if (!validation)
                {
                    result.SecurityValidationResult = SecurityValidationResult.UnAuthorized;
                    return result;
                }

            }
            result.SecurityValidationResult = SecurityValidationResult.Ok;
            return result;
        }

        #endregion



    }
}
