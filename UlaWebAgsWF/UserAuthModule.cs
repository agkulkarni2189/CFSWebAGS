using DIMSContainerDBEFDLL;
using NLog;
using System;
using System.Collections.Generic;
using System.Web;

namespace UlaWebAgsWF
{
    public class UserAuthModule : IHttpModule
    {
        private static Logger logger = LogManager.GetLogger("ErrorLogger", typeof(UserAuthModule));

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
            //context.LogRequest += new EventHandler(OnLogRequest);
            context.PreRequestHandlerExecute += Context_PreRequestHandlerExecute;
        }

        private void Context_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            DIMSContainerDBEFDLL.EntityProxies.UserMasterProxy LoggedInUser = null;
            List<DIMSContainerDBEFDLL.sp_GetScreensFromRoleID_Result> UserAccessibleScreens = null;

            if (HttpContext.Current.Request.RawUrl.Contains(".aspx") && !HttpContext.Current.Request.Url.AbsolutePath.Contains("/Login.aspx"))
            {
                if (HttpContext.Current.Session != null)
                {
                    if (HttpContext.Current.Session["LoggedInUser"] == null)
                    {
                        HttpContext.Current.Session["ErrorMsg"] = "No user logged in";

                        logger.Info(new LogMessageGenerator(() =>
                        {
                            return "Redirecting to log in page, no user logged in";
                        }));

                        HttpContext.Current.Server.Transfer("Login.aspx", true);
                    }
                    else if (!((DIMSContainerDBEFDLL.EntityProxies.UserMasterProxy)HttpContext.Current.Session["LoggedInUser"]).IsLoggedin)
                    {
                        HttpContext.Current.Session["ErrorMsg"] = "User recently logged out";

                        logger.Info(new LogMessageGenerator(() =>
                        {
                            return "Redirecting to log in page, user recently logged out";
                        }));

                        HttpContext.Current.Server.Transfer("Login.aspx", true);
                    }

                    LoggedInUser = (DIMSContainerDBEFDLL.EntityProxies.UserMasterProxy)(HttpContext.Current.Session["LoggedInUser"]);
                    UserAccessibleScreens = (List<sp_GetScreensFromRoleID_Result>)HttpContext.Current.Session["UserAccessibleScreens"];

                    using (UserAuthorization UAuth = new UserAuthorization(ref LoggedInUser, ref UserAccessibleScreens))
                    {
                        string[] UrlSep = ((HttpApplication)sender).Request.Url.AbsolutePath.Split(new char[] { '/' });

                        if (!UAuth.canUserAccessPage(UrlSep[UrlSep.Length - 1]))
                        {
                            if (UrlSep[UrlSep.Length - 1] == "Default.aspx")
                            {
                                HttpContext.Current.Server.Transfer("Login.aspx", true);
                                HttpContext.Current.Session["ErrorMsg"] = "User " + LoggedInUser.ToString() + "\nhas no access to " + UrlSep[UrlSep.Length - 1].Split(new char[] { '.' })[0] + " module, redirecting to log in page";

                                logger.Info(new LogMessageGenerator(() =>
                                {
                                    return "User " + LoggedInUser.ToString() + "\nhas no access to " + UrlSep[UrlSep.Length - 1].Split(new char[] { '.' })[0] + " module, redirecting to log in page";
                                }));
                            }
                            else
                            {
                                HttpContext.Current.Server.Transfer("Default.aspx", true);
                                HttpContext.Current.Session["ErrorMsg"] = "User " + LoggedInUser.ToString() + "\nhas no access to " + UrlSep[UrlSep.Length - 1].Split(new char[] { '.' })[0] + " module, redirecting to dashboard";

                                logger.Info(new LogMessageGenerator(() =>
                                {
                                    return "User " + LoggedInUser.ToString() + "\nhas no access to " + UrlSep[UrlSep.Length - 1].Split(new char[] { '.' })[0] + " module, redirecting to dashboard";
                                }));
                            }
                        }
                    }
                }
                else
                {
                    HttpContext.Current.Session["ErrorMsg"] = "Invalid application context";

                    logger.Info(new LogMessageGenerator(() =>
                    {
                        return "No session set, invalid application context";
                    }));

                    HttpContext.Current.Server.Transfer("Login.aspx", false);
                }
            }
        }

        #endregion
    }
}
