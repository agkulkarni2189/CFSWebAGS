using DIMSContainerDBEFDLL;
using DIMSContainerDBEFDLL.EntityProxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UlaWebAgsWF
{
    public partial class PasswordReset : System.Web.UI.Page
    {
        private DIMContainerDB_Revised_DevEntities dcre = null;
        private UserMasterProxy FirstLoggedInUser = null;
        private ServerUtilities utilities = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            dcre = new DIMContainerDB_Revised_DevEntities();
            FirstLoggedInUser = (UserMasterProxy)HttpContext.Current.Session["LoggedInUser"];
            utilities = new ServerUtilities();

            if (!IsPostBack)
            {
                LblUserName.Text = FirstLoggedInUser.UserName;
            }
        }

        protected void PasswordResetSubmit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                UserMasterProxy UserFromDB = (UserMasterProxy)dcre.UserMasters.Where(a => a.UserId.Equals(FirstLoggedInUser.UserId)).FirstOrDefault();
                UserFromDB.Password = utilities.GetEncryptedMessage(NewPassword.Text);
                dcre.SaveChanges();
                HttpContext.Current.Session["PasswordResetRequest"] = "false";
                HttpContext.Current.Session["SuccessMsg"] = "Password reset successful";
                Response.Redirect("Default.aspx", true);
            }
        }

        protected void CustomValidatorNewPassword_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            if (!((from user in dcre.UserMasters where user.Password.Equals(NewPassword.Text) select user).Any()))
            {
                args.IsValid = true;
            }
        }
    }
}