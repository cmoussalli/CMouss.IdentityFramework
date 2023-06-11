using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{
    public class EntityPermission
    {
        public string EntityId { get; set; }

        public string PermissionTypeId { get; set; }


        public EntityPermission() { }
        public EntityPermission(string entityId, string permissionTypeId)
        {
            EntityId = entityId;
            PermissionTypeId = permissionTypeId;
        }

    }
}
