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

namespace UlaWebAgsWF
{
    public partial class Login : System.Web.UI.Page
    {
        private DIMContainerDB_RevisedEntities dcre;
        private ServerUtilities utilities = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            ClearMessages();
            dcre = new DIMContainerDB_RevisedEntities();
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

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["SuccessMsg"] as string))
            {
                txtSuccessMsg.Text = HttpContext.Current.Session["SuccessMsg"].ToString();
                txtSuccessMsg.Visible = true;
                PnlSuccessMsg.Visible = true;
                HttpContext.Current.Session.Remove("SuccessMsg");
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
            UserMaster UserToBeAuthenticated = null;
            UserMaster AuthenticatedUser = null;
            List<sp_GetScreensFromRoleID_Result> UserAccessibleScreens = null;

            if (Page.IsValid)
            {
                UserToBeAuthenticated = new UserMaster() { UserName = txtUserName.Text, Password = txtPassword.Text };

                using (UserAuthentication UAuthentication = new UserAuthentication(ref UserToBeAuthenticated))
                {
                    AuthenticatedUser = UAuthentication.GetAuthenticatedUser();

                    if (AuthenticatedUser != null)
                    {
                        if (AuthenticatedUser.IsLoggedin)
                        {
                            txtFailureMsg.Text = "User " + AuthenticatedUser.UserName + " is already logged in.";
                            PnlFailureMsg.Visible = true;
                            return;
                        }

                        if (!AuthenticatedUser.IsActive)
                        {
                            txtFailureMsg.Text = "User " + AuthenticatedUser.UserName + " is not active. Contact system admin";
                            PnlFailureMsg.Visible = true;
                            return;
                        }

                        UserAccessibleScreens = dcre.sp_GetScreensFromRoleID(AuthenticatedUser.RoleID).ToList<sp_GetScreensFromRoleID_Result>();

                        using (UserAuthorization UAuthorization = new UserAuthorization(ref AuthenticatedUser, ref UserAccessibleScreens))
                        {
                            if (UAuthorization.canUserAccessHomePage(ref AuthenticatedUser))
                            {
                                HttpContext.Current.Session["UserAccessibleScreens"] = UserAccessibleScreens;
                                AuthenticatedUser.IsLoggedin = true;
                                UserMaster UserToLogInDB = dcre.UserMasters.Where(a => a.UserId.Equals(AuthenticatedUser.UserId)).First();
                                UserToLogInDB.IsLoggedin = true;
                                AuthenticatedUser.Password = null;
                                HttpContext.Current.Session["LoggedInUser"] = AuthenticatedUser;
                                HttpContext.Current.Session["SysIP"] = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).Select(s => s).First().ToString();
                                dcre.SaveChanges();
                                Response.Redirect("Default.aspx", false);
                                Context.ApplicationInstance.CompleteRequest();
                            }
                            else
                            {
                                txtFailureMsg.Text = "Access previlege to dashboard has been revoked for this user. Please contact system admin.";
                                PnlFailureMsg.Visible = true;
                                return;
                            }
                        }
                    }
                    else
                    {
                        txtFailureMsg.Text = "Invalid login credentials, contact system admin.";
                        PnlFailureMsg.Visible = true;
                    }
                }
            }
        }
    }
}