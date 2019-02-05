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
using DIMSContainerDBEFDLL.EntityProxies;
using NLog;
using AutoMapper;

namespace UlaWebAgsWF
{
    public class Global : HttpApplication
    {
        DIMContainerDB_Revised_DevEntities dcre = new DIMContainerDB_Revised_DevEntities();
        ServerUtilities utilities = new ServerUtilities();
        private static Logger logger = LogManager.GetLogger("ActionLogger", typeof(Global));

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            CreateMappers();
            RegisterDefaultScreens();
            CreateDefaultRole();
        }

        void CreateDefaultUser()
        {
            if ((from user in dcre.UserMasters select user).Count() <= 0)
            {
                UserMaster user = new UserMaster();
                user.FirstName = ConfigurationManager.AppSettings["DefaultUserFirstName"].ToString();
                user.LastName = ConfigurationManager.AppSettings["DefaultUserLastName"].ToString();
                user.ContactNo = ConfigurationManager.AppSettings["DefaultUserContactNumber"].ToString();
                //user.Designation = ConfigurationManager.AppSettings["DefaultUserDesignation"].ToString();
                user.UserName = ConfigurationManager.AppSettings["DefaultUserUserName"].ToString();
                user.Password = utilities.GetEncryptedMessage(ConfigurationManager.AppSettings["DefaultUserPassword"].ToString());
                user.EmailId = ConfigurationManager.AppSettings["DefaultUserEmailID"].ToString();
                user.IsActive = true;
                user.IsLoggedin = false;
                //user.RoleID = dcre.RoleMasters.Where(a => a.IsSuperUser == true).Select(b => b.ID).First();

                dcre.UserMasters.Add(user);
                dcre.SaveChanges();
                HttpContext.Current.Session["PasswordResetRequest"] = "true";
                HttpContext.Current.Session["UsersCount"] = dcre.UserMasters.Count().ToString();

                logger.Trace(new LogMessageGenerator(() => { return "Default user " + user.UserName + " with super admin role created successfully."; }));
            }
        }

        void CreateMappers()
        {
            Mapper.Initialize(cfg => 
            {
                cfg.CreateMap<UserMaster, UserMasterProxy>();
                cfg.CreateMap<RoleMaster, RoleMasterProxy>();
                cfg.CreateMap<LocationTypeMaster, LocationTypeMasterProxy>();
                cfg.CreateMap<LocationMaster, LocationMasterProxy>();
                cfg.CreateMap<DeviceTypeMaster, DeviceTypeMasterProxy>();
                cfg.CreateMap<DeviceMaster, DeviceMasterProxy>();
                cfg.CreateMap<DamageTypeMaster, DamageTypeMasterProxy>();
                cfg.CreateMap<DamageTransaction, DamageTransactionProxy>();
                cfg.CreateMap<DamageTransactionDetail, DamageTransactionDetailProxy>();
                cfg.CreateMap<ContainerTypeMaster, ContainerTypeMasterProxy>();
                cfg.CreateMap<ContainerTransaction, ContainerTransactionProxy>();
                cfg.CreateMap<CameraPositionMaster, CameraPositionMasterProxy>();
                cfg.CreateMap<CameraDtlsTbl, CameraDtlsTblProxy>();
                cfg.CreateMap<ApplicationMaster, ApplicationMasterProxy>();
                cfg.CreateMap<LocationTypeDeviceTypeMappingMaster, LocationTypeDeviceTypeMappingMasterProxy>();
                cfg.CreateMap<LocationTypeRoleMappingMaster, LocationTypeRoleMappingMasterProxy>();
            });
        }

        void CreateDefaultRole()
        {
            DIMSContainerDBEFDLL.RoleMaster DefaultRole = null;
            if ((from role in dcre.RoleMasters select role).Count() <= 0)
            {
                if ((from screen in dcre.ScreenMasters select screen).Count() > 0)
                {
                    DefaultRole = new DIMSContainerDBEFDLL.RoleMaster
                    {
                        RoleName = ConfigurationManager.AppSettings["DefaultRoleName"].ToString()
                    };
                    bool isSuperUser = false;

                    if (Boolean.TryParse(ConfigurationManager.AppSettings["DefaultRoleIsSuperUser"].ToString(), out isSuperUser))
                        DefaultRole.IsSuperUser = isSuperUser;

                    dcre.RoleMasters.Add(DefaultRole);
                    dcre.SaveChanges();
                    logger.Trace(new LogMessageGenerator(() => { return "Default super admin role created successfully."; }));
                }
            }
        }

        void RegisterDefaultScreens()
        {
            if ((from screen in dcre.ScreenMasters select screen).Count() <= 0)
            {
                dcre.ScreenMasters.Add(new ScreenMaster { ScreenName = "Home", ScreenUrl = "Default.aspx" });
                dcre.ScreenMasters.Add(new ScreenMaster { ScreenName = "Screens", ScreenUrl = "ScreensMaster.aspx" });

                dcre.SaveChanges();

                logger.Trace(new LogMessageGenerator(() => { return "Default screens generated successfully."; }));
            }
        }

        void UserLogout()
        {
            if (HttpContext.Current.Session["LoggedInUser"] != null)
            {
                UserMasterProxy user = ((UserMasterProxy)HttpContext.Current.Session["LoggedInUser"]);

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
            logger.Trace(new LogMessageGenerator(() => { return "User logged out due to session end."; }));
        }

        void Application_End(object sender, EventArgs e)
        {
            UserLogout();
            logger.Trace(new LogMessageGenerator(() => { return "User logged out, Application instance end"; }));
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
            Response.Write("<p>" + exception.StackTrace + "</p>\n");
            Response.Write("Return to the <a href='"+HttpContext.Current.Request.UrlReferrer+"'>Previous Page</a>\n");

            logger.Error(new LogMessageGenerator(() => {
                return exception.Message;
            }));

            logger.Error(new LogMessageGenerator(() => {
                return exception.StackTrace;
            }));

            Server.ClearError();
        }

        void Application_AcquireRequestState(object sender, EventArgs e)
        {
            if(HttpContext.Current.Session == null || HttpContext.Current.Session["UsersCount"] == null || (HttpContext.Current.Session["UsersCount"] != null && Int32.Parse(HttpContext.Current.Session["UsersCount"] as string) <= 0))
            { 
                CreateDefaultUser();
            }
        }
    }
}