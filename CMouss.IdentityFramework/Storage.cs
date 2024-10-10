using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{
    public static class Storage
    {
        
        public static List<Role> Roles { get; set; }
        public static List<App> Apps { get; set; }
        public static List<Entity> Entities { get; set; }
        public static List<PermissionType> PermissionTypes { get; set; }
        public static List<AppPermissionType> AppPermissionTypes { get; set; }
        public static List<AttributeType> AttributeTypes { get; set; }


    }
}
