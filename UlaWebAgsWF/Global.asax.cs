using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using DIMSContainerDBEFDLL;

namespace UlaWebAgsWF
{
    public class Global : HttpApplication
    {
        DIMContainerDB_RevisedEntities dcre = new DIMContainerDB_RevisedEntities();
        ServerUtilities utilities = new ServerUtilities();

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            if ((from screen in dcre.ScreenMasters select screen).Count() <= 0)
            {
                string[] UserAccessibleScreensNames = ConfigurationManager.AppSettings["DefaultScreenName"].ToString().Split(new char[] { ',' });
                string[] UserAccessibleScreensUrls = ConfigurationManager.AppSettings["DefaultScreenUrl"].ToString().Split(new char[] { ',' });

                for (int i = 0; i < UserAccessibleScreensNames.Length; i++)
                {
                    dcre.ScreenMasters.Add(new ScreenMaster() { ScreenName = UserAccessibleScreensNames[i], ScreenUrl = UserAccessibleScreensUrls[i] });
                }
                
                dcre.SaveChanges();
            }

            if ((from role in dcre.RoleMasters select role).Count() <= 0)
            {
                if ((from screen in dcre.ScreenMasters select screen).Count() > 0)
                {
                    DIMSContainerDBEFDLL.RoleMaster role = new DIMSContainerDBEFDLL.RoleMaster();
                    role.RoleName = ConfigurationManager.AppSettings["DefaultRoleName"].ToString();
                    bool isSuperUser = false;

                    if (Boolean.TryParse(ConfigurationManager.AppSettings["DefaultRoleIsSuperUser"].ToString(), out isSuperUser))
                        role.IsSuperUser = isSuperUser;

                    dcre.RoleMasters.Add(role);
                    dcre.SaveChanges();
                }
            }

            if ((from user in dcre.UserMasters select user).Count() <= 0)
            {
                DIMSContainerDBEFDLL.UserMaster user = new UserMaster();
                user.FirstName = ConfigurationManager.AppSettings["DefaultUserFirstName"].ToString();
                user.LastName = ConfigurationManager.AppSettings["DefaultUserLastName"].ToString();
                user.ContactNo = ConfigurationManager.AppSettings["DefaultUserContactNumber"].ToString();
                user.Designation = ConfigurationManager.AppSettings["DefaultUserDesignation"].ToString();
                user.UserName = ConfigurationManager.AppSettings["DefaultUserUserName"].ToString();
                user.Password = utilities.GetEncryptedMessage(ConfigurationManager.AppSettings["DefaultUserPassword"].ToString());
                user.EmailId = ConfigurationManager.AppSettings["DefaultUserEmailID"].ToString();
                user.IsActive = true;
                user.IsLoggedin = false;
                user.RoleID = dcre.RoleMasters.Where(a => a.IsSuperUser == true).Select(b => b.ID).First();

                dcre.UserMasters.Add(user);
                dcre.SaveChanges();
                HttpContext.Current.Session["PasswordResetRequest"] = true;
            }
        }

        void UserLogout()
        {
            if (HttpContext.Current.Session["LoggedInUser"] != null)
            {
                UserMaster user = ((UserMaster)HttpContext.Current.Session["LoggedInUser"]);

                if (user.IsLoggedin)
                {
                    int UserID = Int32.Parse(((UserMaster)HttpContext.Current.Session["LoggedInUser"]).UserId.ToString());

                    dcre.sp_UserLogout(UserID);
                }
            }
        }

        void Session_End(object sender, EventArgs e)
        {
            UserLogout();
        }

        void Application_End(object sender, EventArgs e)
        {
            UserLogout();
        }

        void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            if (exception.GetType().Equals(typeof(HttpException)))
            {
                if (exception.Message.Contains("NoCatch") || exception.Message.Contains("maxUrlLength"))
                    return;

                Server.Transfer("HttpErrorPage.aspx");
            }

            Response.Write("<h2>Global Page Error</h2>\n");
            Response.Write("<p>" + exception.Message + "</p>\n");
            Response.Write("Return to the <a href='Default.aspx'>Default Page</a>\n");
            Server.ClearError();
        }
    }
}