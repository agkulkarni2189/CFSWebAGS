using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DIMSContainerDBEFDLL;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using NLog;
using DIMSContainerDBEFDLL.EntityProxies;
using AutoMapper;
using System.Net.NetworkInformation;

namespace UlaWebAgsWF
{
    public partial class Login : System.Web.UI.Page
    {
        private DIMContainerDB_Revised_DevEntities dcre;
        private ServerUtilities utilities = null;
        private static Logger logger = LogManager.GetLogger("UserLoginLogger", typeof(Login));

        protected void Page_Load(object sender, EventArgs e)
        {
            ClearMessages();
            dcre = new DIMContainerDB_Revised_DevEntities();
            this.Page.Title = "Login";
            utilities = new ServerUtilities();

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["ErrorMsg"] as string))
            {
                string message = HttpContext.Current.Session["ErrorMsg"].ToString();
                HttpContext.Current.Session.Remove("ErrorMsg");

                txtFailureMsg.Text = message;
                txtFailureMsg.Visible = true;
                PnlFailureMsg.Visible = true;
            }
            else if (!string.IsNullOrEmpty(HttpContext.Current.Session["SuccessMsg"] as string))
            {
                txtSuccessMsg.Text = HttpContext.Current.Session["SuccessMsg"].ToString();
                txtSuccessMsg.Visible = true;
                PnlSuccessMsg.Visible = true;
                HttpContext.Current.Session.Remove("SuccessMsg");
            }
            else if (HttpContext.Current.Session["LoggedInUser"] != null)
            {
                UserMasterProxy LoggedInUser = (UserMasterProxy)HttpContext.Current.Session["LoggedInUser"];

                if (LoggedInUser.IsLoggedin && LoggedInUser.IsActive)
                {
                    HttpContext.Current.Session["SuccessMsg"] = "User " + LoggedInUser.UserName + " is already logged in";

                    logger.Info(new LogMessageGenerator(() =>
                    {
                        return "User " + LoggedInUser.UserName + " is already logged in, redirecting to dashboard";
                    }));

                    Response.Redirect("Default.aspx", false);
                }
            }
        }

        private void ClearMessages()
        {
            txtSuccessMsg.Text = string.Empty;
            txtFailureMsg.Text = string.Empty;
            PnlFailureMsg.Visible = false;
            PnlSuccessMsg.Visible = false;
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            UserMasterProxy UserToBeAuthenticated = null;
            UserMasterProxy AuthenticatedUser = null;
            List<sp_GetScreensFromRoleID_Result> UserAccessibleScreens = null;

            if (Page.IsValid)
            {
                logger.Info(new LogMessageGenerator(() => {
                    return "Initiating user log in process";
                }));

                UserToBeAuthenticated = new UserMasterProxy() { UserName = txtUserName.Text, Password = txtPassword.Text };

                using (UserAuthentication UAuthentication = new UserAuthentication(ref UserToBeAuthenticated))
                {
                    AuthenticatedUser = UAuthentication.GetAuthenticatedUser();

                    if (AuthenticatedUser != null)
                    {
                        if (AuthenticatedUser.IsLoggedin)
                        {
                            throw new InvalidOperationException("User " + AuthenticatedUser.UserName + " is already logged in.");
                        }

                        if (!AuthenticatedUser.IsActive)
                        {
                            throw new InvalidOperationException("User " + AuthenticatedUser.UserName + " is not active, unable to log in");
                        }

                        UserAccessibleScreens = dcre.sp_GetScreensFromRoleID(AuthenticatedUser.DesignationID).ToList<sp_GetScreensFromRoleID_Result>();

                        using (UserAuthorization UAuthorization = new UserAuthorization(ref AuthenticatedUser, ref UserAccessibleScreens))
                        {
                            if (UAuthorization.canUserAccessHomePage(ref AuthenticatedUser))
                            {
                                string CurrentDeviceMAC = NetworkInterface.GetAllNetworkInterfaces().Where(a => a.OperationalStatus.Equals(OperationalStatus.Up)).Select(b => b.GetPhysicalAddress()).FirstOrDefault().ToString();
                                DeviceMasterProxy CurrentDevice = Mapper.Map<DeviceMasterProxy>(dcre.DeviceMasters.Where(a => a.DeviceIP.Equals(Request.UserHostAddress)).FirstOrDefault());

                                if (CurrentDevice == null)
                                {
                                    throw new InvalidOperationException("This device is not registered with the system");
                                }
                                else
                                {
                                    using (UserAuthorization UserDevAuth = new UserAuthorization(ref AuthenticatedUser, ref CurrentDevice))
                                    {
                                        if (UserDevAuth.canUserAccessDevice())
                                        {
                                            HttpContext.Current.Session["UserAccessibleScreens"] = UserAccessibleScreens;
                                            AuthenticatedUser.IsLoggedin = true;
                                            UserMasterProxy UserToLogInDB = Mapper.Map<UserMasterProxy>(dcre.UserMasters.Where(a => a.UserId.Equals(AuthenticatedUser.UserId)).First());
                                            UserToLogInDB.IsLoggedin = true;
                                            UserToLogInDB.DeviceID = CurrentDevice.ID;
                                            AuthenticatedUser.Password = null;

                                            HttpContext.Current.Session["LoggedInUser"] = AuthenticatedUser;
                                            HttpContext.Current.Session["LoggedInDevice"] = CurrentDevice;
                                            dcre.SaveChanges();

                                            logger.Info(new LogMessageGenerator(() => {
                                                return "Log in successful for\n" + AuthenticatedUser.ToString() + "\naccessing from system: " + CurrentDevice.ToString();
                                            }));

                                            if (HttpContext.Current.Session["PasswordResetRequest"] != null)
                                            {
                                                bool PasswordResetRequest = Boolean.Parse(HttpContext.Current.Session["PasswordResetRequest"] as string);

                                                if (PasswordResetRequest)
                                                {
                                                    HttpContext.Current.Session["InfoMsg"] = "First user, need to reset the password";

                                                    logger.Info(new LogMessageGenerator(() =>
                                                    {
                                                        return "User loggin in first time, redirecting to password reset page";
                                                    }));

                                                    Response.Redirect("PasswordReset.aspx", false);
                                                }
                                            }
                                            else
                                            {
                                                logger.Info(new LogMessageGenerator(() =>
                                                {
                                                    return "Redirecting to dashboard";
                                                }));

                                                Response.Redirect("Default.aspx", false);
                                            }
                                        }
                                        else
                                        {
                                            throw new InvalidOperationException("User " + AuthenticatedUser.ToString() + " is not allowed to access the application from device: " + CurrentDevice.ToString());
                                        }
                                    }
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException("Access previlege to dashboard has been revoked for " + AuthenticatedUser.ToString() + ". Please contact system admin.");
                            }
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("User log in failed due to invalid log in credentials");
                    }
                }
            }
        }
    }
}