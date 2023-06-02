using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CMouss.IdentityFramework
{
    public class UserService
    {

        public User Details(string id)
        {
            List<User> lst = IDFManager.Context.Users.Include(o => o.Apps).Where(o => o.Id == id && o.IsDeleted == false).ToList();
            if (lst.Count == 0)
            {
                throw new NotFoundException();
            }
            return lst[0];
        }

        public List<User> GetAll(bool includeDeleted)
        {
            if (includeDeleted)
            {
                return IDFManager.Context.Users.ToList();
            }
            else
            {
                return IDFManager.Context.Users.Where(o => o.IsDeleted == false).ToList();
            }
        }

        public List<User> Search(Paging paging, UsersSearch usersSearch)
        {
            return IDFManager.Context.Users.Where(o =>
                (String.IsNullOrEmpty(usersSearch.UserName) || o.UserName.ToLower().Contains(usersSearch.UserName.ToLower()))
                && (String.IsNullOrEmpty(usersSearch.FullName) || o.FullName.ToLower().Contains(usersSearch.FullName.ToLower()))
                && (String.IsNullOrEmpty(usersSearch.Email) || o.Email.ToLower().Contains(usersSearch.Email.ToLower()))
                && (usersSearch.IsActive == null || usersSearch.IsActive == o.IsActive)
                && (usersSearch.IsDeleted == null || usersSearch.IsDeleted == o.IsDeleted)
                && (usersSearch.IsLocked == null || usersSearch.IsLocked == o.IsLocked)

                ).Skip(paging.Skip).Take(paging.PageSize).ToList();
        }


        public void Register(string userName, string password, string fullName, string email)
        {
            List<User> lst = IDFManager.Context.Users.Where(o =>
                o.UserName.ToLower() == userName.ToLower()
                || o.Email.ToLower() == email.ToLower()
                ).ToList();
            if (lst.Count > 0)
            {
                if (lst[0].UserName.ToLower() == userName.ToLower()) { throw new Exception("Username already exists"); }
                if (lst[0].Email.ToLower() == email.ToLower()) { throw new Exception("EMail already exists"); }
            }
            User o = new User();
            o.Id = Helpers.GenerateId();
            o.UserName = userName;
            o.FullName = fullName;
            o.Email = email;
            o.PrivateKey = Helpers.GenerateKey();
            o.Password = Helpers.Encrypt(password, o.PrivateKey);
            o.CreateDate = DateTime.Now;
            o.IsActive = IDFManager.IsActiveByDefault;
            o.IsLocked = IDFManager.IsLockedByDefault;
            o.IsDeleted = false;
            IDFManager.Context.Users.Add(o);
            IDFManager.Context.SaveChanges();

        }

        public string Create(string userName, string password, string fullName, string email)
        {
            return Create(userName, password, fullName, email, IDFManager.IsLockedByDefault, IDFManager.IsActiveByDefault);
        }
        public string Create(string userName, string password, string fullName, string email, bool isLocked, bool isActive)
        {
            List<User> lst = IDFManager.Context.Users.Where(o =>

                o.UserName.ToLower() == userName.ToLower()
                || o.Email.ToLower() == email.ToLower()
                ).ToList();
            if (lst.Count > 0)
            {
                if (lst[0].UserName.ToLower() == userName.ToLower()) { throw new Exception("Username already exists"); }
                if (lst[0].Email.ToLower() == email.ToLower()) { throw new Exception("EMail already exists"); }
            }
            User o = new User();
            o.Id = Helpers.GenerateId();
            o.UserName = userName;
            o.FullName = fullName;
            o.Email = email;
            o.PrivateKey = Helpers.GenerateKey();
            o.Password = Helpers.Encrypt(password, o.PrivateKey);
            o.CreateDate = DateTime.Now;
            o.IsActive = isActive;
            o.IsLocked = isLocked;
            o.IsDeleted = false;
            IDFManager.Context.Users.Add(o);
            IDFManager.Context.SaveChanges();
            return o.Id;
        }

        public void Update(string id, string fullName, string email, bool isLocked, bool isActive)
        {
            List<User> lst = IDFManager.Context.Users.Where(o => o.Id == id).ToList();
            if (IDFManager.Context.Users.Any(o =>
                 o.Id != id
                     && (o.FullName.ToLower() == fullName.ToLower()
                     || o.Email.ToLower() == email.ToLower()
                     )
                )
            )
            {
                throw new DuplicateValuesAreNotAllowedException();
            }
            if (lst.Count > 0)
            {
                lst[0].FullName = fullName;
                lst[0].Email = email;
                lst[0].IsActive = isActive;
                lst[0].IsLocked = isLocked;
            }
            else
            {
                throw new NotFoundException();
            }
            IDFManager.Context.SaveChanges();
        }

        public void Delete(string id)
        {
            User user = IDFManager.Context.Users.First(o => o.Id == id);
            if (user == null)
            {
                throw new NotFoundException();
            }
            else
            {
                user.IsDeleted = true;
                //Delete all user tokens
                UserTokenService userTokenService = new UserTokenService();
                userTokenService.DeleteUserTokens(id);
            }
            IDFManager.Context.SaveChanges();
        }

        public void Lock(string id)
        {
            List<User> lst = IDFManager.Context.Users.Where(o => o.Id == id).ToList();
            if (lst.Count > 0)
            {
                lst[0].IsLocked = true;
            }
            else
            {
                throw new NotFoundException();
            }
            IDFManager.Context.SaveChanges();
        }
        public void UnLock(string id)
        {
            List<User> lst = IDFManager.Context.Users.Where(o => o.Id == id).ToList();
            if (lst.Count > 0)
            {
                lst[0].IsLocked = false;
            }
            else
            {
                throw new NotFoundException();
            }
            IDFManager.Context.SaveChanges();
        }

        //public void Delete(string id)
        //{
        //    List<User> lst = IDFManager.Context.Users.Where(o => o.Id == id).ToList();
        //    if (lst.Count > 0)
        //    {
        //        lst[0].IsDeleted = true;
        //    }
        //    else
        //    {
        //        throw new NotFoundException();
        //    }
        //    IDFManager.Context.SaveChanges();
        //}


        public void ChangePassword(string id, string newPassword, bool changePrivateKey)
        {
            User o = IDFManager.Context.Users.Find(id);
            if (o == null)
            {
                throw new NotFoundException();
            }
            if (changePrivateKey)
            {
                o.PrivateKey = Helpers.GenerateKey();
            }
            o.Password = Helpers.Encrypt(newPassword, o.PrivateKey);
            IDFManager.Context.SaveChanges();

        }

        public void ChangeMyPassword(string userName, string oldPassword, string newPassword, bool changePrivateKey)
        {
            User o = IDFManager.Context.Users.First(o => o.UserName.ToLower() == userName.ToLower());
            if (o == null)
            {
                throw new NotFoundException();
            }
            if (Helpers.Decrypt(o.Password, o.PrivateKey) != oldPassword)
            {
                throw new IncorrectPasswordException();
            }
            if (changePrivateKey)
            {
                o.PrivateKey = Helpers.GenerateKey();
            }
            o.Password = Helpers.Encrypt(newPassword, o.PrivateKey);
            IDFManager.Context.SaveChanges();

        }

        public UserToken UserLogin(string user, string password)
        {
            List<Role> roles = IDFManager.Context.Roles.ToList();

            UserToken t = new UserToken();
            List<User> o = IDFManager.Context.Users.Include(o => o.Apps).Where(o => o.UserName.ToLower() == user.ToLower() && o.IsDeleted == false).ToList();
            if (o == null)
            {
                throw new NotFoundException();
            }
            if (o.Count == 0)
            {
                throw new NotFoundException();
            }

            if (password != Helpers.Decrypt(o[0].Password, o[0].PrivateKey))
            {
                throw new IncorrectPasswordException();
            }


            t = IDFManager.UserTokenServices.Create(o[0].Id);
            return t;
        }


        #region Get Roles
        public List<Role> GetRoles(string userId)
        {
            List<User> users = IDFManager.Context.Users.Include(o => o.Roles).Where(o => o.Id == userId).ToList();
            if (users.Count == 0) { throw new NotFoundException(); }
            return users[0].Roles;
        }
        #endregion

        #region Grant Role
        public void GrantRole(string userId, string roleId)
        {
            List<User> users = IDFManager.Context.Users.Include(o => o.Roles).Where(o => o.Id == userId).ToList();
            if (users.Count == 0) { throw new UserNotFoundException(); }
            List<Role> roles = IDFManager.Context.Roles.Where(o => o.Id == roleId).ToList();
            if (roles.Count == 0) { throw new RoleNotFoundException(); }

            if (users[0].Roles.Contains(roles[0]))
            {
                throw new AlreadyExistException();
            }

            users[0].Roles.Add(roles[0]);
            //IDFManager.Context.RoleUsers.Add(new RoleUser { })
            IDFManager.Context.SaveChanges();
        }
        #endregion

        #region Revoke Role
        public void RevokeRole(string userId, string roleId)
        {
            List<User> users = IDFManager.Context.Users.Where(o => o.Id == userId).ToList();
            if (users.Count == 0) { throw new UserNotFoundException(); }
            List<Role> roles = IDFManager.Context.Roles.Where(o => o.Id == roleId).ToList();
            if (roles.Count == 0) { throw new RoleNotFoundException(); }

            List<Role> userRoles = users[0].Roles.Where(o =>
               o.Id == roleId
                ).ToList();
            if (userRoles.Count == 0) { throw new NotFoundException(); }
            users[0].Roles.Remove(userRoles[0]);
            IDFManager.Context.SaveChanges();
        }
        #endregion


        #region Validate User Role
        public bool ValidateUserRole(string userId, string roleId)
        {
            bool result = false;
            result = IDFManager.Context.Roles.Include(r => r.Users).Any(o => o.Id == roleId && o.Users.Any(u => u.Id == userId));
            if (result) { return result; }

            return result;
        }
        public bool ValidateUserRole(string userId, List<string> roleIds)
        {
            bool result = false;
            result = IDFManager.Context.Roles.Include(r => r.Users).Any(o => roleIds.Contains(o.Id) && o.Users.Any(u => u.Id == userId));
            if (result) { return result; }

            return result;
        }
        #endregion

        #region Validate User Permission
        public bool ValidateUserPermission(string userId, string entityId, string permissionTypeId)
        {
            return IDFManager.Context.Permissions.Any(p =>
                p.Role.Users.Any(u => u.Id.Trim() == userId.Trim())
                && p.EntityId == entityId
                && p.PermissionTypeId.Trim() == permissionTypeId.Trim()
            );
        }
        public bool ValidateUserPermission(string userId, string entityId, List<string> permissionTypeIds)
        {
            return IDFManager.Context.Permissions.Any(p =>
                p.Role.Users.Any(u => u.Id.Trim() == userId.Trim())
                && p.EntityId == entityId
                && permissionTypeIds.Contains(p.PermissionTypeId)
            );
        }
        #endregion

        #region Validate User Role or Permission
        public bool ValidateUserRoleOrPermission(string userID, string rolesId, string entity, string permissionTypeId)
        {
            bool result = false;
            if (ValidateUserRole(userID, rolesId))
            {
                return true;
            }
            if (ValidateUserPermission(userID, entity, permissionTypeId))
            {
                return true;
            }
            return result;
        }
        public bool ValidateUserRoleOrPermission(string userID, List<string> rolesIds, string entity, string permissionTypeId)
        {
            bool result = false;
            if (ValidateUserRole(userID, rolesIds))
            {
                return true;
            }
            if (ValidateUserPermission(userID, entity, permissionTypeId))
            {
                return true;
            }
            return result;
        }
        public bool ValidateUserRoleOrPermission(string userID, string rolesId, string entity, List<string> permissionTypeIds)
        {
            bool result = false;
            if (ValidateUserRole(userID, rolesId))
            {
                return true;
            }
            if (ValidateUserPermission(userID, entity, permissionTypeIds))
            {
                return true;
            }
            return result;
        }
        public bool ValidateUserRoleOrPermission(string userID, List<string> rolesIds, string entity, List<string> permissionTypeIds)
        {
            bool result = false;
            if (ValidateUserRole(userID, rolesIds))
            {
                return true;
            }
            if (ValidateUserPermission(userID, entity, permissionTypeIds))
            {
                return true;
            }
            return result;
        }
        #endregion



        #region Get User Permissions
        public List<EntityAllowedActionsModel> GetUserPermissions(string userId)
        {
            List<EntityAllowedActionsModel> result = new();
            List<User> users = IDFManager.Context.Users.Include(o => o.Roles).ThenInclude(r => r.Permissions).Where(o => o.Id == userId).ToList();
            if (users.Count == 0) { throw new NotFoundException(); }
            List<Permission> permissions = users[0].Roles.SelectMany(r => r.Permissions).ToList();
            List<Permission> permissionsWithRelatedData = IDFManager.Context.Permissions.Include(p => p.Entity).Include(p => p.PermissionType).Where(p => permissions.Select(s => s.Id).Contains(p.Id)).ToList();   /*.Where(p => permissions.Any(a => a.Id == p.Id)).ToList();*/

            foreach (Permission p in permissionsWithRelatedData)
            {
                EntityAllowedActionsModel ae = new();
                List<EntityAllowedActionsModel> aes = result.Where(o => o.Entity.Id == p.EntityId).ToList();
                if (aes.Count == 0)
                {
                    ae.Entity = p.Entity;
                    ae.AllowedActions.Add(p.PermissionType);
                    result.Add(ae);
                }
                else
                {
                    ae = aes[0];
                    if (!ae.AllowedActions.Any(a => a.Id == p.PermissionTypeId))
                    {
                        ae.AllowedActions.Add(p.PermissionType);
                    }
                }
            }

            //foreach (Permission p in permissionsWithRelatedData)
            //{
            //    if (result.Count(o => o.Entity.Id == p.EntityId && o.PermissionType.Id == p.PermissionTypeId) == 0)
            //    {

            //        result.Add(new AllowedAction { Entity = p.Entity, PermissionType = p.PermissionType });
            //    }
            //}

            return result;
        }
        #endregion

        #region Get User Entitys
        public List<PermissionType> GetUserEntityPermissions(string userId, string entityId)
        {
            List<PermissionType> result = new();
            List<Permission> permissions = IDFManager.Context.Permissions.Where(p =>
                p.Role.Users.Any(u => u.Id.Trim() == userId.Trim())
                && p.EntityId == entityId
                ).ToList();
            result = permissions.Select(o => o.PermissionType).Distinct().ToList();
            return result;
        }
        #endregion







        #region Validate UserToken Role or Permission
        public bool ValidateTokenRoleOrPermission(string token, string rolesId, string entity, string permissionTypeId)
        {
            bool result = false;
            UserToken userToken = IDFManager.UserTokenServices.Validate(token);
            if (userToken == null)
            {
                throw new InvalidTokenException();
            }
            if (ValidateUserRole(userToken.UserId, rolesId))
            {
                return true;
            }
            if (ValidateUserPermission(userToken.UserId, entity, permissionTypeId))
            {
                return true;
            }
            return result;
        }
        public bool ValidateTokenRoleOrPermission(string token, List<string> rolesIds, string entity, string permissionTypeId)
        {
            bool result = false;
            UserToken userToken = IDFManager.UserTokenServices.Validate(token);
            if (userToken == null)
            {
                throw new InvalidTokenException();
            }
            if (ValidateUserRole(userToken.UserId, rolesIds))
            {
                return true;
            }
            if (ValidateUserPermission(userToken.UserId, entity, permissionTypeId))
            {
                return true;
            }
            return result;
        }
        public bool ValidateTokenRoleOrPermission(string token, string rolesId, string entity, List<string> permissionTypeIds)
        {
            bool result = false;
            UserToken userToken = IDFManager.UserTokenServices.Validate(token);
            if (userToken == null)
            {
                throw new InvalidTokenException();
            }
            if (ValidateUserRole(userToken.UserId, rolesId))
            {
                return true;
            }
            if (ValidateUserPermission(userToken.UserId, entity, permissionTypeIds))
            {
                return true;
            }
            return result;
        }
        public bool ValidateTokenRoleOrPermission(string token, List<string> rolesIds, string entity, List<string> permissionTypeIds)
        {
            bool result = false;
            UserToken userToken = IDFManager.UserTokenServices.Validate(token);
            if (userToken == null)
            {
                throw new InvalidTokenException();
            }
            if (ValidateUserRole(userToken.UserId, rolesIds))
            {
                return true;
            }
            if (ValidateUserPermission(userToken.UserId, entity, permissionTypeIds))
            {
                return true;
            }
            return result;
        }
        #endregion








    }
}
