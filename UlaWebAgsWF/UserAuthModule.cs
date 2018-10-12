using DIMSContainerDBEFDLL;
using System;
using System.Collections.Generic;
using System.Web;

namespace UlaWebAgsWF
{
    public class UserAuthModule : IHttpModule
    {
        /// <summary>
        /// You will need to configure this module in the Web.config file of your
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: https://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpModule Members

        public void Dispose()
        {
            //clean-up code here.
        }

        public void Init(HttpApplication context)
        {
            // Below is an example of how you can handle LogRequest event and provide 
            // custom logging implementation for it
            context.LogRequest += new EventHandler(OnLogRequest);
            context.PreRequestHandlerExecute += Context_PreRequestHandlerExecute;
        }

        private void Context_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpContext CurrentContext = ((HttpApplication)sender).Context;
            DIMSContainerDBEFDLL.UserMaster LoggedInUser = null;
            List<DIMSContainerDBEFDLL.sp_GetScreensFromRoleID_Result> UserAccessibleScreens = null;

            if (CurrentContext.Request.RawUrl.Contains(".aspx") && !CurrentContext.Request.Url.AbsolutePath.Contains("/Login.aspx"))
            {
                if (CurrentContext.Session != null)
                {
                    if (CurrentContext.Session["LoggedInUser"] == null)
                    {
                        CurrentContext.Session["ErrorMsg"] = "No user logged in";
                        HttpContext.Current.Server.Transfer("Login.aspx", true);
                    }
                    else if (!((DIMSContainerDBEFDLL.UserMaster)CurrentContext.Session["LoggedInUser"]).IsLoggedin)
                    {
                        CurrentContext.Session["ErrorMsg"] = "User recently logged out";
                        HttpContext.Current.Server.Transfer("Login.aspx", true);
                    }
                    else
                    {
                        LoggedInUser = (UserMaster)(CurrentContext.Session["LoggedInUser"]);
                        UserAccessibleScreens = (List<sp_GetScreensFromRoleID_Result>)CurrentContext.Session["UserAccessibleScreens"];

                        if (CurrentContext.Session["PasswordResetRequest"] != null)
                        {
                            bool PasswordResetRequest = Boolean.Parse(CurrentContext.Session["PasswordResetRequest"] as string);

                            if (PasswordResetRequest)
                            {
                                HttpContext.Current.Session["InfoMsg"] = "First user, need to reset the password";
                                HttpContext.Current.Server.Transfer("PasswordReset.aspx", true);
                            }
                        }

                        using (UserAuthorization UAuth = new UserAuthorization(ref LoggedInUser, ref UserAccessibleScreens))
                        {
                            string[] UrlSep = ((HttpApplication)sender).Request.Url.AbsolutePath.Split(new char[] { '/' });

                            if (!UAuth.canUserAccessPage(UrlSep[UrlSep.Length - 1], ref LoggedInUser))
                            {
                                HttpContext.Current.Session["ErrorMsg"] = "User " + LoggedInUser.UserName + " has no access to requested module";
                                if (UrlSep[UrlSep.Length - 1] != "DamageImages.aspx")
                                    HttpContext.Current.Server.Transfer("Login.aspx", true);
                                else
                                    HttpContext.Current.Server.Transfer("Default.aspx", true);
                            }
                        }
                    }
                }
                else
                {
                    HttpContext.Current.Session["ErrorMsg"] = "Invalid application context";
                    CurrentContext.Server.Transfer("Login.aspx", false);
                }
            }
            else
            {
                HttpContext.Current.Session["IsTransactionPreviewEnabled"] = System.Configuration.ConfigurationManager.AppSettings["EnableContainerTransactionPreview"].ToString();

                if (CurrentContext.Session["LoggedInUser"] != null)
                {
                    LoggedInUser = (UserMaster)CurrentContext.Session["LoggedInUser"];

                    if (LoggedInUser.IsLoggedin && LoggedInUser.IsActive)
                    {
                        HttpContext.Current.Session["SuccessMsg"] = "User " + LoggedInUser.UserName + " is already logged in";
                        HttpContext.Current.Server.Transfer("Default.aspx", true);
                    }
                }
            }
        }

        private void Context_BeginRequest(object sender, EventArgs e)
        {
            
        }

        #endregion

        public void OnLogRequest(Object source, EventArgs e)
        {
            //custom logging logic can go here
        }
    }
}
