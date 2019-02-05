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
using DIMSContainerDBEFDLL.EntityProxies;
using Microsoft.AspNet.Identity;
using NLog;

namespace UlaWebAgsWF
{
    public partial class RoleMaster : System.Web.UI.Page, IWebAGSClass
    {
        DIMContainerDB_Revised_DevEntities dcre = null;
        private static ConcurrentDictionary<DIMSContainerDBEFDLL.RoleMaster, List<RoleScreenMapping>> RoleScreenMappingDict = null;
        private static List<ScreenMaster> AllScreens = null;
        private string ErrorMsg = string.Empty;
        private static Logger logger = LogManager.GetLogger("RoleMasterLogger", typeof(RoleMaster));

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                ClearMessages();
                dcre = new DIMContainerDB_Revised_DevEntities();

                if (!IsPostBack)
                {
                    Response.Cache.SetExpires(System.DateTime.UtcNow.AddMinutes(-1));
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.Cache.SetNoStore();

                    RoleScreenMappingDict = new ConcurrentDictionary<DIMSContainerDBEFDLL.RoleMaster, List<RoleScreenMapping>>();
                    AllScreens = dcre.ScreenMasters.ToList<ScreenMaster>();

                    dcre.RoleMasters.ToList<DIMSContainerDBEFDLL.RoleMaster>().ForEach((r) => {
                        List<RoleScreenMapping> RoleScreenMaps = new List<RoleScreenMapping>();

                        dcre.RoleScreenMappings.Where(a => a.RoleID.Equals(r.ID)).Select(b => b).ToList<RoleScreenMapping>().ForEach((rsm) =>
                        {
                            RoleScreenMaps.Add(rsm);
                        });

                        RoleScreenMappingDict.TryAdd(r, RoleScreenMaps);
                    });

                    Fill_ScreenCBL(ref ScreenAuthorizedFor);
                    Fill_RolesDGV();
                }
            }
            catch (Exception ex)
            {
                throw new HttpException(403, ex.Message, ex);
            }
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
                    rsm.RoleID = Role.ID;
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
                    roleMaster.IsSuperUser = CbxIsSU.Checked;

                    dcre.RoleMasters.Add(roleMaster);

                    if (dcre.SaveChanges() > 0)
                    {
                        DIMSContainerDBEFDLL.RoleMaster RecentRole = dcre.RoleMasters.OrderByDescending(a => a.ID).Select(b => b).First();

                        if (Modify_RoleScreenMappings(RecentRole, ref ScreenAuthorizedFor) > 0)
                        {
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
                throw new HttpException(403, "Can not create new role due to unexpected exception occured in process", ex);
            }
        }

        protected void RolesDGV_RowEditing(object sender, GridViewEditEventArgs e)
        {
            RolesDGV.EditIndex = e.NewEditIndex;
            Fill_RolesDGV(true, e.NewEditIndex);
        }

        protected void Delete_RoleScreenMappings(int RoleID, bool isEdition = false)
        {
            if (isEdition)
            {
                dcre.RoleScreenMappings.RemoveRange(dcre.RoleScreenMappings.Where(a => a.RoleID.Equals(RoleID)).AsEnumerable());
                dcre.SaveChanges();
            }
           
            Task task = new Task(new Action(() => {
                List<RoleScreenMapping> rsm = new List<RoleScreenMapping>();
                while (!RoleScreenMappingDict.TryRemove(RoleScreenMappingDict.Keys.Where(a => a.ID.Equals(RoleID)).Select(b => b).First(), out rsm))
                    RoleScreenMappingDict.TryRemove(RoleScreenMappingDict.Keys.Where(a => a.ID.Equals(RoleID)).Select(b => b).First(), out rsm);
            }), TaskCreationOptions.AttachedToParent);

            task.Start();
            task.Wait();
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
                    if (roleMaster.IsSuperUser != null && (bool)roleMaster.IsSuperUser)
                    {
                        FailureMsgText.Text = "Super user role can not be edited";
                        FailureMsgText.Visible = true;
                        FailureMsg.Visible = true;
                    }
                    else
                    {
                        this.Delete_RoleScreenMappings(RoleID, true);
                        CheckBoxList EditedScreens = (CheckBoxList)RolesDGV.Rows[e.RowIndex].FindControl("ScreenAccessCBLEdit");
                        this.Modify_RoleScreenMappings(roleMaster, ref EditedScreens);
                    }
                   
                    RolesDGV.EditIndex = -1;
                    Fill_RolesDGV();
                }
            }
            catch (Exception ex)
            {
                throw new HttpException(403, ex.Message, ex);
            }
        }

        protected void RolesDGV_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
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
                    DIMSContainerDBEFDLL.RoleMaster RoleToDelete = dcre.RoleMasters.Where(a => a.ID.Equals(RoleID)).Select(b => b).First();

                    if (RoleToDelete.IsSuperUser != null && (bool)RoleToDelete.IsSuperUser)
                    {
                        if (dcre.RoleMasters.Where(a => a.IsSuperUser == true).Count() > 1)
                        {
                            dcre.RoleMasters.Remove(RoleToDelete);
                            dcre.SaveChanges();
                            Delete_RoleScreenMappings(RoleID, false);
                        }
                        else
                        {
                            FailureMsgText.Text = "Can not delete this user, at least one super user must exist in the system";
                            FailureMsgText.Visible = true;
                            FailureMsg.Visible = true;
                        }
                    }
                    else
                    {
                        dcre.RoleMasters.Remove(RoleToDelete);
                        dcre.SaveChanges();
                        Delete_RoleScreenMappings(RoleID, false);
                    }
                    
                    Fill_RolesDGV();
                }
            }
            catch (Exception ex)
            {
                throw new HttpException(403, "Can not delete the role due to unexpected error occured in process", ex);
            }  
        }

        protected void RolesDGV_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            SuccessMsgText.Text = "Role edition cancelled successfully";
            SuccessMsgText.Visible = true;
            RolesDGV.EditIndex = -1;
            Fill_RolesDGV();
            SuccessMsg.Visible = true;
        }

        protected void RolesDGV_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            SuccessMsgText.Text = "Role deleted successfully";
            SuccessMsgText.Visible = true;
            SuccessMsg.Visible = true;
        }

        protected void Check_ScreenCBL(List<RoleScreenMapping> rsm, ref CheckBoxList ScreenCBL)
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