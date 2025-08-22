using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework.BlazorUI
{
    public class AuthLayoutModel
    {
        public bool IsAuthenticated { get; set; } = false;
        public CMouss.IdentityFramework.User User { get; set; } = new();

        public string Token { get; set; } = "";

    }
}
