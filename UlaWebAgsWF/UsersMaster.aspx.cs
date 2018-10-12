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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace UlaWebAgsWF
{
    public partial class UsersMaster : System.Web.UI.Page, IWebAGSClass
    {
        private DIMContainerDB_RevisedEntities dcde = null;
        private string ErrorMsg = string.Empty;
        private static ConcurrentDictionary<int, User> UsersList = null;
        private ServerUtilities utilities = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            dcde = new DIMContainerDB_RevisedEntities();

            utilities = new ServerUtilities();

            if (!IsPostBack)
            {
                List<User> Users = (from
                                        um in dcde.UserMasters
                                    join
                                        rm in dcde.RoleMasters
                                    on
                                        um.RoleID equals rm.ID
                                    select new User
                                    {
                                        UserId = um.UserId,
                                        FirstName = um.FirstName,
                                        LastName = um.LastName,
                                        ContactNo = um.ContactNo,
                                        EmailId = um.EmailId,
                                        Designation = um.Designation,
                                        Role = rm.RoleName,
                                        isUserActive = um.IsActive
                                    }).ToList<User>();


                UsersList = new ConcurrentDictionary<int, UlaWebAgsWF.User>();
                foreach (User user in Users)
                {
                    UsersList.TryAdd(user.UserId, user);
                }

                this.Page.Title = "User Master";
                Fill_UsersDGV();
                FillRolesDD();
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

        private void FillRolesDD()
        {
            UserRolesDD.DataTextField = "RoleName";
            UserRolesDD.DataValueField = "ID";
            List<DIMSContainerDBEFDLL.RoleMaster> Roles = dcde.RoleMasters.ToList<DIMSContainerDBEFDLL.RoleMaster>();

            UserRolesDD.DataSource = Roles;
            UserRolesDD.DataBind();
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

                UserMaster UserToBeDeleted = dcde.UserMasters.Where(a => a.UserId == UserID).First();
                UlaWebAgsWF.User user = new UlaWebAgsWF.User();

                dcde.UserMasters.Remove(UserToBeDeleted);

                if (dcde.SaveChanges() > 0)
                {
                    Task task = new Task(new Action(() => {
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
            catch (Exception ex)
            {
                FailureMsgTxt.Text = "Failed to delete user. Try again or contact system administrator";
                FailureMsgTxt.Visible = true;
                FailureMsg.Visible = true;
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
                    var userStore = new UserStore<IdentityUser>();
                    var manager = new UserManager<IdentityUser>(userStore);
                    var user = new IdentityUser() { UserName = NewUserUserName.Text };
                    IdentityResult result = manager.Create(user, new PasswordHasher().HashPassword(NewUserPassword.Text));

                    if (result.Succeeded)
                    {
                        SuccessMsgTxt.Text = "User created Successfully";
                        SuccessMsgTxt.Visible = true;
                        SuccessMsg.Visible = true;
                        CreateNewUserPanel.Visible = false;
                    }
                    else
                    {
                        FailureMsgTxt.Text = result.Errors.FirstOrDefault();
                        FailureMsgTxt.Visible = true;
                        FailureMsg.Visible = true;
                    }

                    string NewUserFirstName = NewUserFN.Text;
                    string NewUserLastName = NewUserLN.Text;
                    string NewUserContactNumber = NewUserCN.Text;
                    string NewUserMailID = NewUserEmail.Text;
                    string NewUserDesig = NewUserDesignation.Text;
                    string NewUserUN = NewUserUserName.Text;
                    string NewUserpass = utilities.GetEncryptedMessage(NewUserPassword.Text);
                    int RoleID = Int32.Parse(UserRolesDD.SelectedValue);

                    bool isUserActive = false;

                    if (RoleID == 1)
                        isUserActive = true;
                    else
                        isUserActive = UserActiveCB.Checked;

                    dcde.UserMasters.Add(new UserMaster() { FirstName = NewUserFirstName, LastName = NewUserLastName, ContactNo = NewUserContactNumber, EmailId = NewUserMailID, Designation = NewUserDesig, UserName = NewUserUN, Password = NewUserpass, RoleID = RoleID, IsActive = isUserActive });
                    if (dcde.SaveChanges() > 0)
                    {
                        User NewUser = dcde.UserMasters.Join(dcde.RoleMasters.AsEnumerable(), a => a.RoleID, b => b.ID, (a, b) => new User { UserId = a.UserId, FirstName = a.FirstName, LastName = a.LastName, ContactNo = a.ContactNo, Designation = a.Designation, EmailId = a.EmailId, Role = b.RoleName, UserName = a.UserName, RoleID = a.RoleID, isUserActive = isUserActive }).OrderByDescending(a => a.UserId).First();

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
                        CreateNewUserPanel.Visible = false;
                    }

                    Fill_UsersDGV();
                }
            }
            catch (Exception ex)
            {
                ClearMessages();
                FailureMsgTxt.Text = "Failed to create user. Try again or contact system administrator";
                FailureMsgTxt.Visible = true;
                FailureMsg.Visible = true;
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

                UserMaster UserToBeEdited = (from user in dcde.UserMasters where user.UserId == UserID select user).First();

                TableCellCollection cc = UserRow.Cells;
                UserToBeEdited.FirstName = ((TextBox)UserRow.FindControl("FirstNameTextBox")).Text;
                UserToBeEdited.LastName = ((TextBox)UserRow.FindControl("LastNameTextBox")).Text;
                UserToBeEdited.ContactNo = ((TextBox)UserRow.FindControl("ContactNoTextBox")).Text;
                UserToBeEdited.EmailId = ((TextBox)UserRow.FindControl("EmailIdTextBox")).Text;
                UserToBeEdited.Designation = ((TextBox)UserRow.FindControl("DesignationTextBox")).Text;
                UserToBeEdited.RoleID = Int32.Parse(((DropDownList)UserRow.FindControl("UserDGVRoleDD")).SelectedValue);
                UserToBeEdited.IsActive = ((CheckBox)UserRow.FindControl("UserAcitve")).Checked;

                User UpdatedUser = new UlaWebAgsWF.User(UserID, ((TextBox)UserRow.FindControl("FirstNameTextBox")).Text, ((TextBox)UserRow.FindControl("LastNameTextBox")).Text, ((TextBox)UserRow.FindControl("ContactNoTextBox")).Text, ((TextBox)UserRow.FindControl("EmailIdTextBox")).Text, ((TextBox)UserRow.FindControl("DesignationTextBox")).Text, ((DropDownList)UserRow.FindControl("UserDGVRoleDD")).SelectedItem.Text.ToString(), Int32.Parse(((DropDownList)UserRow.FindControl("UserDGVRoleDD")).SelectedItem.Value), ((CheckBox)UserRow.FindControl("UserAcitve")).Checked);

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
                ClearMessages();
                FailureMsgTxt.Text = "Failed to update user. Try again or contact system administrator";
                FailureMsgTxt.Visible = true;
                FailureMsg.Visible = true;
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
        public string Role { get; set; }
        public int RoleID { get; set; }
        public bool isUserActive { get; set; }

        public User()
        { }

        public User(int UserID, string FirstName, string LastName, string ContactNo, string EmailID, string Designation, string Role,int RoleID, bool isUserActive, string UserName = "") : this()
        {
            this.UserId = UserID;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.ContactNo = ContactNo;
            this.EmailId = EmailID;
            this.Designation = Designation;
            this.Role = Role;
            this.RoleID = RoleID;
            this.isUserActive = isUserActive;

            if (!string.IsNullOrEmpty(UserName))
                this.UserName = UserName;
        }

        public override bool Equals(object obj1)
        {
            bool isUserEqual = false;
            try
            {
                if (obj1 != null && obj1.GetType().Equals(this.GetType()))
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