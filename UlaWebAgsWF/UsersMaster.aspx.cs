using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DIMSContainerDBEFDLL;
using DIMSContainerDBEFDLL.EntityProxies;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace UlaWebAgsWF
{
    public partial class UsersMaster : System.Web.UI.Page, IWebAGSClass
    {
        private DIMContainerDB_Revised_DevEntities dcde = null;
        private string ErrorMsg = string.Empty;
        private static ConcurrentDictionary<int, User> UsersList = null;
        private ServerUtilities utilities = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            dcde = new DIMContainerDB_Revised_DevEntities();

            utilities = new ServerUtilities();

            if (!IsPostBack)
            {
                List<User> Users = dcde.UserMasters.Join(dcde.DesignationMasters, um => um.DesignationID, dm => dm.ID, (um, dm) => new { um, dm }).Select(u => new UlaWebAgsWF.User { UserId = u.um.UserId, FirstName = u.um.FirstName, LastName = u.um.LastName, ContactNo = u.um.ContactNo, Designation = u.dm.DesignationName, DesignationID = u.dm.ID, EmailId = u.um.EmailId, isUserActive = u.um.IsActive, UserName = u.um.UserName }).ToList<User>();
                UsersList = new ConcurrentDictionary<int, UlaWebAgsWF.User>();
                Users.ForEach(new Action<UlaWebAgsWF.User>((u) => { UsersList.TryAdd(u.UserId, u); }));
     
                this.Page.Title = "User Master";
                Fill_UsersDGV();
                FillDesignationDD();
            }
        }

        private void ClearMessages()
        {
            SuccessMsgTxt.Text = string.Empty;
            SuccessMsgTxt.Visible = false;
            SuccessMsg.Visible = false;
            FailureMsgTxt.Text = string.Empty;
            FailureMsgTxt.Visible = false;
            FailureMsg.Visible = false;
        }

        protected void UsersDGV_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView gridView = (GridView)sender;
            gridView.EditIndex = e.NewEditIndex;
            Fill_UsersDGV();
        }

        protected void Fill_UsersDGV()
        {
            if (UsersList != null && UsersList.Count > 0)
            {
                List<User> AllUsers = UsersList.Values.ToList<User>();
                UsersDGV.DataSource = AllUsers;
                UsersDGV.DataBind();
                UsersDGV.Visible = true;
            }
        }

        private void FillDesignationDD()
        {
            UserDesignationDD.DataTextField = "DesignationName";
            UserDesignationDD.DataValueField = "ID";
            List<DIMSContainerDBEFDLL.DesignationMaster> Designations = dcde.DesignationMasters.ToList<DIMSContainerDBEFDLL.DesignationMaster>();

            UserDesignationDD.DataSource = Designations;
            UserDesignationDD.DataBind();
        }

        protected void UsersDGV_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            UsersDGV.EditIndex = -1;
            Fill_UsersDGV();
            ClearMessages();
            SuccessMsgTxt.Text = "User edit cancelled successfully";
            SuccessMsgTxt.Visible = true;
            SuccessMsg.Visible = true;
        }

        protected void UsersDGV_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            ClearMessages();
            SuccessMsgTxt.Text = "User edited Successfully";
            SuccessMsgTxt.Visible = true;
            SuccessMsg.Visible = true;
        }

        protected void UsersDGV_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int UserID = Int32.Parse(UsersDGV.DataKeys[e.RowIndex].Value.ToString());

                DIMSContainerDBEFDLL.EntityProxies.UserMasterProxy UserToBeDeleted = (DIMSContainerDBEFDLL.EntityProxies.UserMasterProxy)dcde.UserMasters.Where(a => a.UserId == UserID).First();
                UlaWebAgsWF.User user = new UlaWebAgsWF.User();

                if (UserToBeDeleted.DesignationMaster.RoleDesignationMappingMasters.FirstOrDefault().RoleMaster.IsSuperUser != null && (bool)UserToBeDeleted.DesignationMaster.RoleDesignationMappingMasters.FirstOrDefault().RoleMaster.IsSuperUser && dcde.UserMasters.Where(a => a.DesignationMaster.RoleDesignationMappingMasters.FirstOrDefault().RoleMaster.IsSuperUser == true).Count() <= 1)
                {
                    FailureMsgTxt.Text = "Can not delete last super user";
                    FailureMsgTxt.Visible = true;
                    FailureMsg.Visible = true;
                }
                else
                {
                    dcde.UserMasters.Remove(UserToBeDeleted);

                    if (dcde.SaveChanges() > 0)
                    {
                        Task task = new Task(new Action(() =>
                        {
                            while (UsersList.TryRemove(UserToBeDeleted.UserId, out user))
                                UsersList.TryRemove(UserToBeDeleted.UserId, out user);
                        }), TaskCreationOptions.AttachedToParent);

                        task.Start();
                        task.Wait();
                        ClearMessages();
                        SuccessMsgTxt.Text = "User deleted successfully";
                        SuccessMsgTxt.Visible = true;
                        SuccessMsg.Visible = true;
                    }

                    Fill_UsersDGV();
                }
            }
            catch (Exception ex)
            {
                throw new HttpException(403, "Failed to delete the user due to unexpected error", ex);
            }
        }

        protected void UsersDGV_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            ClearMessages();
            SuccessMsgTxt.Text = "User deleted Successfully";
            SuccessMsgTxt.Visible = true;
            SuccessMsg.Visible = true;
        }

        protected void CerateNewUserLink_Click(object sender, EventArgs e)
        {
            
        }

        protected void NewUserBtnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (Page.IsValid)
                {
                    UserMasterProxy LoggedInUser = (UserMasterProxy)HttpContext.Current.Session["LoggedInUser"];
                    string NewUserFirstName = NewUserFN.Text;
                    string NewUserLastName = NewUserLN.Text;
                    string NewUserContactNumber = NewUserCN.Text;
                    string NewUserMailID = NewUserEmail.Text;
                    int DesignationID = Int32.Parse(UserDesignationDD.SelectedValue);
                    string NewUserUN = NewUserUserName.Text;
                    string NewUserpass = utilities.GetEncryptedMessage(NewUserPassword.Text);

                    bool isUserActive = false;
                    bool? IsRoleSuperUser = dcde.DesignationMasters.Where(a => a.ID.Equals(DesignationID)).FirstOrDefault().RoleDesignationMappingMasters.FirstOrDefault().RoleMaster.IsSuperUser;//dcde.RoleMasters.Where(a => a.ID.Equals(RoleID)).Select(b => b.IsSuperUser).FirstOrDefault();

                    if (IsRoleSuperUser != null && (bool)IsRoleSuperUser)
                        isUserActive = true;
                    else
                        isUserActive = UserActiveCB.Checked;

                    dcde.UserMasters.Add(new UserMaster() { FirstName = NewUserFirstName, LastName = NewUserLastName, ContactNo = NewUserContactNumber, EmailId = NewUserMailID, DesignationID = DesignationID, UserName = NewUserUN, Password = NewUserpass, IsActive = isUserActive });
                    if (dcde.SaveChanges() > 0)
                    {
                        User NewUser = dcde.UserMasters.Join(dcde.RoleMasters.AsEnumerable(), a => a.DesignationMaster.RoleDesignationMappingMasters.FirstOrDefault().RoleID, b => b.ID, (a, b) => new User { UserId = a.UserId, FirstName = a.FirstName, LastName = a.LastName, ContactNo = a.ContactNo, Designation = a.DesignationMaster.DesignationName, DesignationID = (int)a.DesignationID, EmailId = a.EmailId, UserName = a.UserName, isUserActive = isUserActive }).OrderByDescending(a => a.UserId).First();

                        Task task = new Task(new Action(() =>
                        {
                            UsersList.TryAdd(NewUser.UserId, NewUser);
                        }), TaskCreationOptions.AttachedToParent);

                        task.Start();
                        task.Wait();
                        ClearMessages();
                        SuccessMsgTxt.Text = "User created Successfully";
                        SuccessMsgTxt.Visible = true;
                        SuccessMsg.Visible = true;
                        //CreateNewUserPanel.Visible = false;
                    }

                    Fill_UsersDGV();
                }
            }
            catch (Exception ex)
            {
                throw new HttpException(403, "Failed to create user. Try again or contact system administrator", ex);
            }
        }

        protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
        {

            if (dcde.UserMasters.Where(a => a.UserName == args.Value).Select(b => b).ToList<UserMaster>().Count > 0)
            {
                CustomNewUserUserName.ErrorMessage = "User name must be unique";
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void UsersDGV_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                int RowIndex = e.RowIndex;
                GridViewRow UserRow = UsersDGV.Rows[RowIndex];
                int UserID = Int32.Parse(UsersDGV.DataKeys[e.RowIndex].Value.ToString());

                UserMasterProxy UserToBeEdited = (DIMSContainerDBEFDLL.EntityProxies.UserMasterProxy)(from user in dcde.UserMasters where user.UserId == UserID select user).First();

                TableCellCollection cc = UserRow.Cells;
                UserToBeEdited.FirstName = ((TextBox)UserRow.FindControl("FirstNameTextBox")).Text;
                UserToBeEdited.LastName = ((TextBox)UserRow.FindControl("LastNameTextBox")).Text;
                UserToBeEdited.ContactNo = ((TextBox)UserRow.FindControl("ContactNoTextBox")).Text;
                UserToBeEdited.EmailId = ((TextBox)UserRow.FindControl("EmailIdTextBox")).Text;
                UserToBeEdited.DesignationID = Int32.Parse(((DropDownList)UserRow.FindControl("UserDGVDesignationDD")).SelectedValue);

                if (UserToBeEdited.DesignationID != null && (bool)UserToBeEdited.DesignationMaster.RoleDesignationMappingMasters.FirstOrDefault().RoleMaster.IsSuperUser && dcde.UserMasters.Where(a => a.DesignationMaster.RoleDesignationMappingMasters.FirstOrDefault().RoleMaster.IsSuperUser == true && a.IsActive == true).Count() <= 1 && !((CheckBox)UserRow.FindControl("UserAcitve")).Checked)
                    UserToBeEdited.IsActive = true;
                else
                    UserToBeEdited.IsActive = ((CheckBox)UserRow.FindControl("UserAcitve")).Checked;


                User UpdatedUser = new UlaWebAgsWF.User(UserID, ((TextBox)UserRow.FindControl("FirstNameTextBox")).Text, ((TextBox)UserRow.FindControl("LastNameTextBox")).Text, ((TextBox)UserRow.FindControl("ContactNoTextBox")).Text, ((TextBox)UserRow.FindControl("EmailIdTextBox")).Text, ((DropDownList)UserRow.FindControl("UserDGVDesignationDD")).SelectedItem.Text, Int32.Parse(((DropDownList)UserRow.FindControl("UserDGVDesignationDD")).SelectedValue), ((CheckBox)UserRow.FindControl("UserAcitve")).Checked);

                if (dcde.SaveChanges() > 0)
                {
                    Task task = new Task(new Action(() =>
                    {
                        UsersList.TryUpdate(UserID, UpdatedUser, UsersList.Where(s => s.Key.Equals(UserID)).Select(t => t.Value).First());
                    }), TaskCreationOptions.AttachedToParent);

                    task.Start();
                    task.Wait();

                    UsersDGV.EditIndex = -1;
                }

                Fill_UsersDGV();
            }
            catch (Exception ex)
            {
                throw new HttpException(403, "Failed to update user. Try again or contact system administrator", ex);
            }
        }

        public void SetMessage(string Message)
        {
            this.ErrorMsg = Message;
        }

        public string GetMessage()
        {
            return this.ErrorMsg;
        }
    }

    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string ContactNo { get; set; }
        public string EmailId { get; set; }
        public string Designation { get; set; }
        public int DesignationID { get; set; }
        public bool isUserActive { get; set; }

        public User()
        { }

        public User(int UserID, string FirstName, string LastName, string ContactNo, string EmailID, string Designation, int DesignationID, bool isUserActive, string UserName = "") : this()
        {
            this.UserId = UserID;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.ContactNo = ContactNo;
            this.EmailId = EmailID;
            this.Designation = Designation;
            this.DesignationID = DesignationID;
            this.isUserActive = isUserActive;

            if (!string.IsNullOrEmpty(UserName))
                this.UserName = UserName;
        }

        public override bool Equals(object obj1)
        {
            bool isUserEqual = false;
            try
            {
                if (obj1 != null && obj1.GetType().Equals(typeof(User)) && ((User)obj1).FirstName.Equals(this.FirstName) && ((User)obj1).LastName.Equals(this.LastName) && ((User)obj1).ContactNo.Equals(this.ContactNo) && ((User)obj1).EmailId.Equals(this.EmailId) && ((User)obj1).DesignationID.Equals(this.DesignationID) &&  ((User)obj1).isUserActive.Equals(this.isUserActive))
                {
                    isUserEqual = true;
                }
            }
            catch (Exception ex)
            { }

            return isUserEqual;
        }
    }
}