using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{
    public class EntityAllowedActionsModel
    {
        public Entity Entity { get; set; }
        public List<PermissionType> AllowedActions { get; set; } = new();
    }

}
