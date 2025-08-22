using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework.BlazorUI
{
    public static class IDFBlazorUIConfig
    {
        public static string HomeURL { get; set; } = "";
        public static string AuthHomeURL { get; set; } = "";


        public static string LoginRedirectURL { get; set; } = "/login";
        public static string SignupRedirectURL { get; set; } = "/signup";
        public static string AfterLogoutRedirectURL { get; set; } = "/login";

        public static FormLabels FormLabels { get; set; } = new();

    }


    public class FormLabels
    {
        public string UserName { get; set; } = "Username";
        public string Password { get; set; } = "Password";
        public string LoginButton { get; set; } = "Login";
        public string SignupButton { get; set; } = "Signup";
        public string ResetPassword { get; set; } = "Reset Password";


    }
}
