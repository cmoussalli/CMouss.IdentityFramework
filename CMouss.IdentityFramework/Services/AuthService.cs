using CMouss.IdentityFramework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using CMouss.IdentityFramework.ViewModels;

namespace CMouss.IdentityFramework.Services
{
    public class AuthService
    {

        #region UserOnly

        #region User Login -1
        public AuthResult AuthUserLogin(string user, string? password)
        {
            return AuthUserLogin(user, password, "");
        }

        public AuthResult AuthUserLogin(string user, string password, string? ipAddress)
        {
            AuthResult result = new();
            List<Role> roles = IDFManager.Context.Roles.ToList();

            UserToken t = new UserToken();
            List<User> o = IDFManager.Context.Users
                .Include(o => o.Apps).Include(o => o.Roles)
                .Where(o => o.UserName.ToLower() == user.ToLower() && o.IsDeleted == false).ToList();
            if (o == null)
            {
                result.AuthenticationMode = IDFAuthenticationMode.User;
                result.SecurityValidationResult = SecurityValidationResult.IncorrectCredentials;
                return result;
            }
            if (o.Count == 0)
            {
                //throw new Exception(Messages.UserNotFound);
                result.AuthenticationMode = IDFAuthenticationMode.User;
                result.SecurityValidationResult = SecurityValidationResult.IncorrectCredentials;
                return result;
            }

            //Validate password
            if (password != Helpers.Decrypt(o[0].Password, o[0].PrivateKey))
            {
                //throw new Exception(Messages.IncorrectPassword);
                result.AuthenticationMode = IDFAuthenticationMode.User;
                result.SecurityValidationResult = SecurityValidationResult.IncorrectCredentials;
                return result;
            }


            t = IDFManager.UserTokenServices.Create(o[0].Id, ipAddress);
            //Multiple Session by IP
            if (IDFManager.AllowUserMultipleSessions)
            {
                t.User.LastIPAddress = ipAddress;
                IDFManager.Context.SaveChanges();
            }
            else
            {//Validate IP Address
                if (t.User.LastIPAddress is not null)
                {

                    if (t.User.LastIPAddress.ToLower() != ipAddress.ToLower())
                    {// Changed IP Address
                        List<UserToken> expiredTokens = IDFManager.Context.UserTokens.Where(o => o.IPAddress != ipAddress).ToList();
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
            if (tokenValidationMode == TokenValidationMode.UseDefault)
            {
                tokenValidationMode = IDFManager.TokenValidationMode;
            }

            if (IDFManager.TokenValidationMode == TokenValidationMode.DecryptOnly)
            {
                UserClaim claim = Helpers.DecryptUserToken(token);
                result.AuthenticationMode = IDFAuthenticationMode.User;
                result.SecurityValidationResult = SecurityValidationResult.Ok;

                result.UserToken = new();
                result.UserToken.User = new();
                result.UserToken.UserId = claim.UserId;
                result.UserToken.User.Id = claim.UserId;
                result.UserToken.User.UserName = claim.UserName;
                result.UserToken.User.FullName = claim.UserFullName;
                result.UserToken.User.Email = claim.EMail;
                result.UserToken.ExpireDate = claim.TokenExpireDate;

                result.AuthenticationMode = IDFAuthenticationMode.User;

                List<Role> roles = new();
                foreach (string r in claim.Roles)
                {
                    Role r0 = Storage.Roles.FirstOrDefault(rl => rl.Id == r);
                    if (r0 is not null)
                    {

                        if (r0.Id == r)
                        {
                            roles.Add(r0);
                        }
                    }
                }






                result.UserToken.User.Roles = roles;

                if (string.IsNullOrEmpty(ipAddress))
                {
                    result.UserToken.IPAddress = ipAddress;
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


        #region Authenticate UserToken Permission -1
        /// <summary>
        /// Authenticate using User info only (UserToken). App authentication is not supported.
        /// Authorization is Entity and PermissionType based, multiple PermissionType is not supported with this function.
        /// </summary>
        /// <param name="token">UserToken for Authentication.</param>
        /// <param name="entityPermission">The EntityPermission that the requester should have access to.</param>
        /// <returns>Authentication & Authorization result</returns>
        public AuthResult AuthUserTokenWithPermission(string token, EntityPermission entityPermission)
        {
            AuthResult result = new();
            result.AuthenticationMode = IDFAuthenticationMode.User;
            bool validation = false;
            List<UserToken> ts = IDFManager.Context.UserTokens
                .Include(t => t.User).ThenInclude(u => u.Roles).ThenInclude(r => r.Permissions)
                .Where(o => o.Token.ToLower() == token.ToLower()).ToList();
            if (ts is null)
            {
                result.SecurityValidationResult = SecurityValidationResult.IncorrectToken;
                return result;
            }
            if (ts.Count < 1)
            {
                result.SecurityValidationResult = SecurityValidationResult.IncorrectToken;
                return result;
            }

            result.UserToken = ts[0];
            validation = ts[0].User.Roles.Any(r => r.Permissions.Any(p => p.EntityId.ToLower() == entityPermission.EntityId.ToLower() && p.PermissionTypeId.ToLower().Contains(entityPermission.PermissionTypeId.ToLower())));

            if (!validation)
            {
                result.SecurityValidationResult = SecurityValidationResult.UnAuthorized;
                return result;
            }

            result.SecurityValidationResult = SecurityValidationResult.Ok;
            result.UserToken = ts[0];

            return result;
        }

        ///// <summary>
        ///// Authenticate using User info only (UserToken). App authentication is not supported.
        ///// Authorization is Entity and PermissionType based, multiple PermissionType is supported with this function.
        ///// </summary>
        ///// <param name="token">UserToken for Authentication.</param>
        ///// <param name="entityId">The Entity which user should have access to.</param>
        ///// <param name="permissionTypeIds">List of the required Entity Permissions, having 1 atleast will authorize the user.</param>
        ///// <returns>Authentication & Authorization result</returns>
        //public AuthResult AuthUserTokenWithPermissions(string token, string entityId, List<string> permissionTypeIds)
        //{
        //    AuthResult result = new();
        //    bool validation = false;
        //    List<UserToken> ts = IDFManager.Context.UserTokens.Where(o => o.Token.ToLower() == token.ToLower()).ToList();
        //    if (ts.Count < 1)
        //    {
        //        result.SecurityValidationResult = SecurityValidationResult.IncorrectToken;
        //        return result;
        //    }

        //    result.UserToken = ts[0];
        //    validation = ts[0].User.Roles.Any(r => r.Permissions.Any(p => p.EntityId.ToLower() == entityId.ToLower() && permissionTypeIds.Contains(p.PermissionTypeId)));

        //    if (!validation)
        //    {
        //        result.SecurityValidationResult = SecurityValidationResult.UnAuthorized;
        //        return result;
        //    }

        //    result.SecurityValidationResult = SecurityValidationResult.Ok;
        //    return result;
        //}
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
            AuthResult result = new();
            result.AuthenticationMode = IDFAuthenticationMode.User;
            bool validation = false;
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
            AuthResult result = new();
            result.AuthenticationMode = IDFAuthenticationMode.User;
            bool validation = false;
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

            result.SecurityValidationResult = SecurityValidationResult.Ok;
            return result;
        }

        #endregion



        #region Authenticate UserToken with Role(s) OR Permission -2

        public AuthResult AuthUserTokenWithRolesOrPermission(string token, string roleId, EntityPermission entityPermission)
        {
            List<string> roles = new() { roleId };
            return AuthUserTokenWithRolesOrPermission(token, roles, entityPermission);
        }

        public AuthResult AuthUserTokenWithRolesOrPermission(string token, List<string> roleIds, EntityPermission entityPermission)
        {
            AuthResult result = new();
            result.AuthenticationMode = IDFAuthenticationMode.User;
            bool validation = false;
            List<UserToken> ts = IDFManager.Context.UserTokens.Where(o => o.Token.ToLower() == token.ToLower()).ToList();
            if (ts is null)
            {
                result.SecurityValidationResult = SecurityValidationResult.IncorrectToken;
                return result;
            }
            if (ts.Count < 1)
            {
                result.SecurityValidationResult = SecurityValidationResult.IncorrectToken;
                return result;
            }

            result.UserToken = ts[0];
            validation = IDFManager.Context.Roles.Include(r => r.Users).Any(o => roleIds.Contains(o.Id) && o.Users.Any(u => u.Id == ts[0].UserId));

            //If Role Validation failed, try permission validation
            if (!validation)
            {
                validation = ts[0].User.Roles.Any(r => r.Permissions.Any(p => p.EntityId.ToLower() == entityPermission.EntityId.ToLower() && p.PermissionTypeId.ToLower().Contains(entityPermission.PermissionTypeId.ToLower())));
            }

            if (!validation)
            {
                result.SecurityValidationResult = SecurityValidationResult.UnAuthorized;
                return result;
            }

            result.SecurityValidationResult = SecurityValidationResult.Ok;
            return result;
        }

        #endregion



        #endregion








        #region AppOnly



        #endregion



    }


}
