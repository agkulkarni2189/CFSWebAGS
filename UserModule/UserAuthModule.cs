using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DIMSContainerDBEFDLL;

namespace UserModule
{
    class UserAuthModule : IHttpModule
    {
        public void Dispose()
        {
            
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += Context_BeginRequest;
        }

        private void Context_BeginRequest(object sender, EventArgs e)
        {
            if (sender.GetType().Equals(typeof(HttpApplication)) && ((HttpApplication)sender).Context.Request.RawUrl.Contains(".aspx") && !((HttpApplication)sender).Context.Request.Url.AbsolutePath.Equals("Login.aspx"))
            {
                if (HttpContext.Current.Session["LoggedInUser"] == null || !((DIMSContainerDBEFDLL.UserMaster)HttpContext.Current.Session["LoggedInUser"]).IsLoggedin)
                {
                    HttpContext.Current.Session["ErrorMsg"] = "No user logged in";
                    HttpContext.Current.Response.Redirect("Login.aspx", true);
                }
            }
            else
            {
                DIMSContainerDBEFDLL.UserMaster LoggedInUser = (UserMaster)HttpContext.Current.Session["LoggedInUser"];
                List<DIMSContainerDBEFDLL.sp_GetScreensFromRoleID_Result> UserAccessibleScreens = (List<sp_GetScreensFromRoleID_Result>)HttpContext.Current.Session["UserAccessibleScreens"];

                using (UserAuthorization UAuth = new UserAuthorization(ref LoggedInUser, ref UserAccessibleScreens))
                {
                    if (!UAuth.canUserAccessPage(((HttpApplication)sender).Request.Url.AbsolutePath, ref LoggedInUser))
                    {
                        HttpContext.Current.Session["ErrorMsg"] = "User " + LoggedInUser.UserName + " has no access to requested module";
                        ((HttpApplication)sender).Context.Response.Redirect("Login.aspx", true);
                    }
                }

            }
        }
    }
}
