using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework.Services
{
    public class AuthService
    {

        #region UserOnly

        #region Authenticate UserToken
        public AuthResult AuthUserToken(string token)
        {
            AuthResult result = new();
            bool roleValidation = false;
            List<UserToken> ts = IDFManager.Context.UserTokens
                .Include(t => t.User).ThenInclude(u => u.Roles).ThenInclude(r => r.Permissions)
                .Where(o => o.Token.ToLower() == token.ToLower()).ToList();
            if (ts.Count < 1)
            {
                result.SecurityValidationResult = SecurityValidationResult.IncorrectToken;
                return result;
            }
            else
            {
                result.UserToken = ts[0];
                result.SecurityValidationResult = SecurityValidationResult.Ok;
            }
            return result;
        }
        #endregion


        #region Authenticate UserToken Permission-s
        /// <summary>
        /// Authenticate using User info only (UserToken). App authentication is not supported.
        /// Authorization is Entity and PermissionType based, multiple PermissionType is not supported with this function.
        /// </summary>
        /// <param name="token">UserToken for Authentication.</param>
        /// <param name="entityId">The Entity which user should have access to.</param>
        /// <param name="permissionTypeId">The required Entity Permission.</param>
        /// <returns>Authentication & Authorization result</returns>
        public AuthResult AuthUserTokenWithPermission(string token, string entityId, string permissionTypeId)
        {
            AuthResult result = new();
            bool roleValidation = false;
            List<UserToken> ts = IDFManager.Context.UserTokens
                .Include(t => t.User).ThenInclude(u => u.Roles).ThenInclude(r => r.Permissions)
                .Where(o => o.Token.ToLower() == token.ToLower()).ToList();
            if (ts.Count < 1)
            {
                result.SecurityValidationResult = SecurityValidationResult.IncorrectToken;
                return result;
            }
            else
            {
                result.UserToken = ts[0];
                roleValidation = ts[0].User.Roles.Any(r => r.Permissions.Any(p => p.EntityId.ToLower() == entityId.ToLower() && p.PermissionTypeId.ToLower().Contains(permissionTypeId.ToLower())));
            }
            if (roleValidation)
            {
                result.SecurityValidationResult = SecurityValidationResult.Ok;
                result.UserToken = ts[0];
            }
            else
            {
                result.SecurityValidationResult = SecurityValidationResult.UnAuthorized;
                return result;
            }
            return result;
        }

        /// <summary>
        /// Authenticate using User info only (UserToken). App authentication is not supported.
        /// Authorization is Entity and PermissionType based, multiple PermissionType is supported with this function.
        /// </summary>
        /// <param name="token">UserToken for Authentication.</param>
        /// <param name="entityId">The Entity which user should have access to.</param>
        /// <param name="permissionTypeIds">List of the required Entity Permissions, having 1 atleast will authorize the user.</param>
        /// <returns>Authentication & Authorization result</returns>
        public AuthResult AuthUserTokenWithPermissions(string token, string entityId, List<string> permissionTypeIds)
        {
            AuthResult result = new();
            bool roleValidation = false;
            List<UserToken> ts = IDFManager.Context.UserTokens.Where(o => o.Token.ToLower() == token.ToLower()).ToList();
            if (ts.Count < 1)
            {
                result.SecurityValidationResult = SecurityValidationResult.IncorrectToken;
                return result;
            }
            else
            {
                result.UserToken = ts[0];
                roleValidation = ts[0].User.Roles.Any(r => r.Permissions.Any(p => p.EntityId.ToLower() == entityId.ToLower() && permissionTypeIds.Contains(p.PermissionTypeId)));
            }
            if (roleValidation)
            {
                result.SecurityValidationResult = SecurityValidationResult.Ok;
            }
            else
            {
                result.SecurityValidationResult = SecurityValidationResult.UnAuthorized;
                return result;
            }
            return result;
        }
        #endregion


        #region Authenticate UserToken Role-s
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
            bool roleValidation = false;
            UserToken userToken = IDFManager.UserTokenServices.Validate(token);
            if (userToken is null)
            {
                result.SecurityValidationResult = SecurityValidationResult.IncorrectToken;
                return result;
            }
            else
            {
                result.UserToken = userToken;
                roleValidation = IDFManager.Context.Roles.Include(r => r.Users).Any(o => o.Id == roleId && o.Users.Any(u => u.Id == userToken.UserId));
            }
            if (roleValidation)
            {
                result.SecurityValidationResult = SecurityValidationResult.Ok;
            }
            else
            {
                result.SecurityValidationResult = SecurityValidationResult.UnAuthorized;
                return result;
            }
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
            bool roleValidation = false;
            UserToken userToken = IDFManager.UserTokenServices.Validate(token);
            if (userToken is null)
            {
                result.SecurityValidationResult = SecurityValidationResult.IncorrectToken;
                return result;
            }
            else
            {
                result.UserToken = userToken;
                roleValidation = IDFManager.Context.Roles.Include(r => r.Users).Any(o => roleIds.Contains(o.Id) && o.Users.Any(u => u.Id == userToken.UserId));
            }
            if (roleValidation)
            {
                result.SecurityValidationResult = SecurityValidationResult.Ok;
            }
            else
            {
                result.SecurityValidationResult = SecurityValidationResult.UnAuthorized;
                return result;
            }
            return result;
        }
        #endregion




        #endregion




        #region AppOnly



        #endregion



    }


}
