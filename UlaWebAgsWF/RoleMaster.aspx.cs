using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DIMSContainerDBEFDLL;
using Microsoft.AspNet.Identity;

namespace UlaWebAgsWF
{
    public partial class RoleMaster : System.Web.UI.Page, IWebAGSClass
    {
        DIMContainerDB_RevisedEntities dcre = null;
        private static ConcurrentDictionary<DIMSContainerDBEFDLL.RoleMaster, List<RoleScreenMapping>> RoleScreenMappingDict = null;
        private static List<ScreenMaster> AllScreens = null;
        private string ErrorMsg = string.Empty;
        private List<sp_GetScreensFromRoleID_Result> UserAccessibleScreens = null;
        private DIMSContainerDBEFDLL.UserMaster LoggedInUser = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            //UserAccessibleScreens = (List<sp_GetScreensFromRoleID_Result>)HttpContext.Current.Session["UserAccessibleScreens"];
            //LoggedInUser = (UserMaster)HttpContext.Current.Session["LoggedInUser"];

            dcre = new DIMContainerDB_RevisedEntities();

            //if (HttpContext.Current.Session["LoggedInUser"] == null || !((DIMSContainerDBEFDLL.UserMaster)Session["LoggedInUser"]).IsLoggedin)
            //{
            //    HttpContext.Current.Session["ErrorMsg"] = "No user logged in";
            //    Response.Redirect("Login.aspx", true);
            //    //this.SetMessage("No user logged in");
            //    //new SiteMaster().LinkLogout_Click(this, new EventArgs());
            //}

            if (!IsPostBack)
            {
                Response.Cache.SetExpires(System.DateTime.UtcNow.AddMinutes(-1));
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();

                RoleScreenMappingDict = new ConcurrentDictionary<DIMSContainerDBEFDLL.RoleMaster, List<RoleScreenMapping>>();
                AllScreens = dcre.ScreenMasters.ToList<ScreenMaster>();

                foreach (DIMSContainerDBEFDLL.RoleMaster role in dcre.RoleMasters.AsEnumerable())
                {
                    List<RoleScreenMapping> RoleScreenMaps = new List<RoleScreenMapping>();
                    foreach (RoleScreenMapping RoleScreensMapping in dcre.RoleScreenMappings.Where(a => a.RoleID.Equals(role.ID)).Select(b => b).AsEnumerable())
                    {
                        RoleScreenMaps.Add(RoleScreensMapping);
                    }

                    RoleScreenMappingDict.TryAdd(role, RoleScreenMaps);
                }

                Fill_ScreenCBL(ref ScreenAuthorizedFor);
                Fill_RolesDGV();
            }

            //using (UserAuthorization UAuth = new UserAuthorization(ref LoggedInUser, ref UserAccessibleScreens))
            //{
            //    if (!UAuth.canUserAccessPage(Request.Url.AbsolutePath, ref LoggedInUser))
            //    {
            //        HttpContext.Current.Session["ErrorMsg"] = "User " + LoggedInUser.UserName + " has no access to Roles module";
            //        Response.Redirect("Default.aspx", true);
            //        //this.SetMessage("Logged in user is not authorized to access User Master");
            //        //new SiteMaster().RedirectHomePage(this, new EventArgs());
            //    }
            //}
        }

        private void ClearMessages()
        {
            SuccessMsgText.Text = string.Empty;
            SuccessMsgText.Visible = false;
            FailureMsgText.Text = string.Empty;
            FailureMsgText.Visible = false;
            FailureMsg.Visible = false;
        }

        protected void Fill_ScreenCBL(ref CheckBoxList checkBoxList)
        {
            checkBoxList.DataTextField = "ScreenName";
            checkBoxList.DataValueField = "ID";
            checkBoxList.DataSource = AllScreens;
            checkBoxList.DataBind();
        }

        protected int Modify_RoleScreenMappings(DIMSContainerDBEFDLL.RoleMaster Role, ref CheckBoxList checkBoxList)
        {
            List<RoleScreenMapping> NewRoleScreenMappings = new List<RoleScreenMapping>();

            foreach (ListItem li in checkBoxList.Items)
            {
                if (li.Selected)
                {
                    int ScreenID = Int32.Parse(li.Value.ToString());
                    RoleScreenMapping rsm = new RoleScreenMapping();
                    //rsm.RoleMaster = Role;
                    rsm.RoleID = Role.ID;
                    //rsm.ScreenMaster = AllScreens.Find(s => s.ID.Equals(ScreenID));
                    rsm.ScreenID = Int32.Parse(li.Value);
                    NewRoleScreenMappings.Add(rsm);
                }
            }

            dcre.RoleScreenMappings.AddRange(NewRoleScreenMappings);

            if (dcre.SaveChanges() > 0)
            {
                Task task = new Task(new Action(() => {
                    while (!(RoleScreenMappingDict.AddOrUpdate(Role, NewRoleScreenMappings, (a, b) => NewRoleScreenMappings).Count > 0))
                        RoleScreenMappingDict.AddOrUpdate(Role, NewRoleScreenMappings, (a, b) => NewRoleScreenMappings);
                }), TaskCreationOptions.AttachedToParent);

                task.Start();
                task.Wait();
            }

            return NewRoleScreenMappings.Count();
        }

        protected void NewRoleBtnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (Page.IsValid)
                {
                    DIMSContainerDBEFDLL.RoleMaster roleMaster = new DIMSContainerDBEFDLL.RoleMaster();
                    roleMaster.RoleName = NewRoleName.Text;
                    dcre.RoleMasters.Add(roleMaster);
                    if (dcre.SaveChanges() > 0)
                    {
                        DIMSContainerDBEFDLL.RoleMaster RecentRole = dcre.RoleMasters.OrderByDescending(a => a.ID).Select(b => b).First();

                        if (Modify_RoleScreenMappings(RecentRole, ref ScreenAuthorizedFor) > 0)
                        {
                            ClearMessages();
                            SuccessMsgText.Text = "Role created successfully";
                            SuccessMsgText.Visible = true;
                            SuccessMsg.Visible = true;
                        }
                    }

                    Fill_RolesDGV();
                }
            }
            catch (Exception ex)
            {
                ClearMessages();
                FailureMsgText.Text = "Can not create roll, contact system admin";
                FailureMsgText.Visible = true;
                FailureMsg.Visible = true;
            }
        }

        protected void RolesDGV_RowEditing(object sender, GridViewEditEventArgs e)
        {
            RolesDGV.EditIndex = e.NewEditIndex;

            int RowIndex = e.NewEditIndex;
            int RoleID = Int32.Parse(RolesDGV.DataKeys[RowIndex].Value.ToString());
            Fill_RolesDGV(true, e.NewEditIndex);
        }

        protected void Delete_RoleScreenMappings(int RoleID, bool isEdition = false)
        {
            if (isEdition)
            {
                dcre.RoleScreenMappings.RemoveRange(dcre.RoleScreenMappings.Where(a => a.RoleID.Equals(RoleID)).AsEnumerable());
                dcre.SaveChanges();
            }
            //dcre.RoleScreenMappings.RemoveRange(dcre.RoleScreenMappings.Where(a => a.RoleID.Equals(RoleID)).AsEnumerable());

            //if (dcre.SaveChanges() > 0)
            //{
                Task task = new Task(new Action(() => {
                    List<RoleScreenMapping> rsm = new List<RoleScreenMapping>();
                    while (!RoleScreenMappingDict.TryRemove(RoleScreenMappingDict.Keys.Where(a => a.ID.Equals(RoleID)).Select(b => b).First(), out rsm))
                        RoleScreenMappingDict.TryRemove(RoleScreenMappingDict.Keys.Where(a => a.ID.Equals(RoleID)).Select(b => b).First(), out rsm);
                }), TaskCreationOptions.AttachedToParent);

                task.Start();
                task.Wait();
            //}
        }

        private void Fill_RolesDGV(bool isEdition = false, int EditIndex = -1)
        {
            if (RoleScreenMappingDict != null && RoleScreenMappingDict.Count > 0)
            {
                RolesDGV.DataSource = RoleScreenMappingDict.Keys.ToList<DIMSContainerDBEFDLL.RoleMaster>();
                RolesDGV.DataBind();

                foreach (GridViewRow gvr in RolesDGV.Rows)
                {
                    int RowIndex = gvr.RowIndex;
                    int RoleID = Int32.Parse(RolesDGV.DataKeys[RowIndex].Value.ToString());
                    List<DIMSContainerDBEFDLL.RoleScreenMapping> rsm = RoleScreenMappingDict.Where(a => a.Key.ID.Equals(RoleID)).Select(b => b.Value).First();
                    CheckBoxList cbl1 = null;

                    if (isEdition && RowIndex.Equals(EditIndex))
                        cbl1 = (CheckBoxList)RolesDGV.Rows[RowIndex].FindControl("ScreenAccessCBLEdit");
                    else
                        cbl1 = (CheckBoxList)RolesDGV.Rows[RowIndex].FindControl("ScreenAccessCBLPrev");

                    Check_ScreenCBL(rsm, ref cbl1);

                }
            }
        }

        protected void RolesDGV_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                int RoleID = 0;

                if (Int32.TryParse(RolesDGV.DataKeys[e.RowIndex].Value.ToString(), out RoleID))
                {
                   DIMSContainerDBEFDLL.RoleMaster roleMaster = RoleScreenMappingDict.Where(a => a.Key.ID.Equals(RoleID)).Select(b => b.Key).First();
                    this.Delete_RoleScreenMappings(RoleID, true);
                    CheckBoxList EditedScreens = (CheckBoxList)RolesDGV.Rows[e.RowIndex].FindControl("ScreenAccessCBLEdit");
                    this.Modify_RoleScreenMappings(roleMaster, ref EditedScreens);
                    RolesDGV.EditIndex = -1;
                    Fill_RolesDGV();
                }
            }
            catch (Exception ex)
            {
                FailureMsgText.Text = "Can not update role, try again later";
                FailureMsgText.Visible = true;
                FailureMsg.Visible = true;
            }
        }

        protected void RolesDGV_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            ClearMessages();
            SuccessMsgText.Text = "Role updated successfully";
            SuccessMsgText.Visible = true;
            SuccessMsg.Visible = true;
        }

        protected void RolesDGV_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int RoleID = 0;
                if (Int32.TryParse(RolesDGV.DataKeys[e.RowIndex].Value.ToString(), out RoleID))
                {
                    Delete_RoleScreenMappings(RoleID);
                    dcre.RoleMasters.Remove(dcre.RoleMasters.Where(a => a.ID.Equals(RoleID)).Select(b => b).First());
                    dcre.SaveChanges();
                    Fill_RolesDGV();
                }
            }
            catch (Exception ex)
            {
                ClearMessages();
                FailureMsgText.Text = "Can not delete row, try again later";
                FailureMsgText.Visible = true;
                FailureMsg.Visible = true;
            }  
        }

        protected void RolesDGV_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            ClearMessages();
            SuccessMsgText.Text = "Role edition cancelled successfully";
            SuccessMsgText.Visible = true;
            RolesDGV.EditIndex = -1;
            Fill_RolesDGV();
            SuccessMsg.Visible = true;
        }

        protected void RolesDGV_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            ClearMessages();
            SuccessMsgText.Text = "Role deleted successfully";
            SuccessMsgText.Visible = true;
            SuccessMsg.Visible = true;
        }

        protected void Check_ScreenCBL(List<DIMSContainerDBEFDLL.RoleScreenMapping> rsm, ref CheckBoxList ScreenCBL)
        {
            foreach (ListItem li in ScreenCBL.Items)
            {
                foreach (DIMSContainerDBEFDLL.RoleScreenMapping RoleScreenMap in rsm)
                {
                    if (Int32.Parse(li.Value).Equals(RoleScreenMap.ScreenID))
                        li.Selected = true;
                }
            }
        }

        public void SetMessage(string Message)
        {
            this.ErrorMsg = Message;
        }

        public string GetMessage()
        {
            return ErrorMsg;
        }
    }

    class CBLColumn : ITemplate
    {
        CheckBoxList checkBoxList = null;

        public CBLColumn( ref CheckBoxList checkBoxList, string checkBoxListID)
        {
            this.checkBoxList = checkBoxList;
            checkBoxList.ID = checkBoxListID;
        }
        public void InstantiateIn(System.Web.UI.Control container)
        {
            container.Controls.Add(checkBoxList);
        }
    }
}