using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using DIMSContainerDBEFDLL;
using DIMSContainerDBEFDLL.EntityProxies;

namespace UlaWebAgsWF
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Fill_MainMenu();
        }

        private void Fill_MainMenu()
        {

            MainMenuLiteral.Text = string.Empty;

            if (HttpContext.Current.Session["UserAccessibleScreens"] != null || (HttpContext.Current.Session["PasswordResetRequest"] != null && !Boolean.Parse(HttpContext.Current.Session["PasswordResetRequest"] as string)))
            {
                foreach (sp_GetScreensFromRoleID_Result uas in (List<sp_GetScreensFromRoleID_Result>)HttpContext.Current.Session["UserAccessibleScreens"])
                {
                    if (!uas.ScreenUrl.Equals("DamageImages.aspx"))
                        MainMenuLiteral.Text += "<li id='" + uas.ScreenName + "LinkContainer'><a runat='server' id='" + uas.ScreenName + "Link' href='" + uas.ScreenUrl + "'>" + uas.ScreenName + "</a></li>";
                }

                if (((UserMasterProxy)HttpContext.Current.Session["LoggedInUser"]).IsLoggedin)
                    LinkLogout.Visible = true;
            }
            else
                LinkLogout.Visible = false;
        }

        public void LinkLogout_Click(object sender, EventArgs e)
        {
            try
            {
                DIMContainerDB_Revised_DevEntities dcre = new DIMContainerDB_Revised_DevEntities();
                UserMasterProxy LoggedInUser = (UserMasterProxy)HttpContext.Current.Session["LoggedInUser"];

                if (LoggedInUser != null)
                {
                    UserMasterProxy UserToLogOutDB = (UserMasterProxy)dcre.UserMasters.Where(a => a.UserId.Equals(LoggedInUser.UserId)).First();
                    UserToLogOutDB.IsLoggedin = false;
                    UserToLogOutDB.DeviceID = null;
                    dcre.SaveChanges();
                    HttpContext.Current.Session.Remove("LoggedInUser");
                    HttpContext.Current.Session.Remove("UserAccessibleScreens");
                    HttpContext.Current.Session.Remove("LoggedInDevice");
                    HttpContext.Current.Session["SuccessMsg"] = "User logged out successfully";
                }
                else
                {
                    HttpContext.Current.Session["ErrorMsg"] = "No user logged in";
                }

                Response.Redirect("Login.aspx", true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}