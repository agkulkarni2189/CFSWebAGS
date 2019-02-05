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
using NLog;

namespace UlaWebAgsWF
{
    public partial class LanesMaster : System.Web.UI.Page, IWebAGSClass
    {
        DIMContainerDB_Revised_DevEntities dcde = null;
        private string ErrorMsg = string.Empty;
        private static ConcurrentDictionary<int, LaneMaster> LanesDictionary = null;
        private static Logger logger = LogManager.GetLogger("LaneMasterLogger", typeof(LaneMaster));

        protected void Page_Load(object sender, EventArgs e)
        {
            ClearMessages();
  
            dcde = new DIMContainerDB_Revised_DevEntities();

            logger.Trace(new LogMessageGenerator(() => {
                return "Loading lanes master from system: " + HttpContext.Current.Session["SysIP"] + " accessed by " + ((DIMSContainerDBEFDLL.EntityProxies.UserMasterProxy)HttpContext.Current.Session["LoggedInUser"]).UserName;
            }));

            if (!IsPostBack)
            {
                Response.Cache.SetExpires(System.DateTime.UtcNow.AddMinutes(-1));
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();

                this.Page.Title = "Lane Master";
                LanesDGV.RowEditing += LanesDGV_RowEditing;
                LanesDGV.RowUpdating += LanesDGV_RowUpdating;
                LanesDGV.RowCancelingEdit += LanesDGV_RowCancelingEdit;

                LanesDictionary = new ConcurrentDictionary<int, LaneMaster>();
                List<LaneMaster> Lanes = dcde.LaneMasters.ToList<LaneMaster>();
                foreach (LaneMaster lane in Lanes)
                    LanesDictionary.TryAdd(lane.LaneID, lane);

                Fill_LanesDGV();
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

        protected void LanesDGV_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            LanesDGV.EditIndex = -1;
            Fill_LanesDGV();
        }

        protected void LanesDGV_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            LaneMaster Lane = null;

            try
            {
                GridViewRow LaneGridViewRow = LanesDGV.Rows[e.RowIndex];
                int LaneID = Int32.Parse(LanesDGV.DataKeys[e.RowIndex].Value.ToString());

                Lane = dcde.LaneMasters.Where(a => a.LaneID == LaneID).First();

                logger.Trace(new LogMessageGenerator(() => {
                    return "Updating lane: "+Lane.LaneName;
                }));

                Lane.LaneName = ((TextBox)LaneGridViewRow.FindControl("LaneNameTBX")).Text;
                //Lane.SystemIP = ((TextBox)LaneGridViewRow.FindControl("LaneSystemIPTBX")).Text;

                if (dcde.SaveChanges() > 0)
                {
                    Task task = new Task(new Action(() => {
                        while (!LanesDictionary.TryUpdate(Lane.LaneID, Lane, LanesDictionary.Where(a => a.Key.Equals(LaneID)).Select(b => b.Value).First()))
                            LanesDictionary.TryUpdate(Lane.LaneID, Lane, LanesDictionary.Where(a => a.Key.Equals(LaneID)).Select(b => b.Value).First());
                    }), TaskCreationOptions.AttachedToParent);

                    task.Start();
                    task.Wait();
                }
                LanesDGV.EditIndex = -1;
                Fill_LanesDGV();
                ClearMessages();
                //SuccessMsgText.Text = "Lane updated successfully";
                //SuccessMsg.Visible = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void Fill_LanesDGV()
        {
            if (LanesDictionary != null && LanesDictionary.Count > 0)
            {
                List<LaneMaster> Lanes = LanesDictionary.Values.ToList<LaneMaster>();

                LanesDGV.AutoGenerateColumns = false;
                LanesDGV.EmptyDataText = "No Lanes Registered";
                LanesDGV.DataSource = Lanes;
                LanesDGV.DataBind();
            }
        }

        protected void LanesDGV_RowEditing(object sender, GridViewEditEventArgs e)
        {
            LanesDGV.EditIndex = e.NewEditIndex;
            Fill_LanesDGV();
        }

        protected void LanesDGV_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int LaneID = Int32.Parse(LanesDGV.DataKeys[e.RowIndex].Value.ToString());
                List<LaneMaster> Lanes = dcde.LaneMasters.Where(b => b.LaneID == LaneID).Select(c => c).ToList<LaneMaster>();

                logger.Trace(new LogMessageGenerator(() => {
                    return "Deleting lane: " + Lanes[0].LaneName;
                }));

                if (Lanes.Count > 0)
                {
                    dcde.LaneMasters.Remove(Lanes.First());
                    if(dcde.SaveChanges() > 0)
                    {
                        LaneMaster RemovedLane = new LaneMaster();

                        Task task = new Task(new Action(() => {
                            while (!LanesDictionary.TryRemove(Lanes.First().LaneID, out RemovedLane))
                                LanesDictionary.TryRemove(Lanes.First().LaneID, out RemovedLane);
                        }), TaskCreationOptions.AttachedToParent);

                        task.Start();
                        task.Wait();
                    }
                    Fill_LanesDGV();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void LanesDGV_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            ClearMessages();
            SuccessMsgText.Text = "Lane updated successfully";
            SuccessMsg.Visible = true;

            logger.Trace(new LogMessageGenerator(() => {
                return "Lane updated successfully";
            }));
        }

        protected void LanesDGV_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            ClearMessages();
            SuccessMsgText.Text = "Lane deleted successfully";
            SuccessMsg.Visible = true;

            logger.Trace(new LogMessageGenerator(() => {
                return "Lane deleted successfully";
            }));
        }

        protected void NewLaneBtnSubmit_Click(object sender, EventArgs e)
        {

            logger.Trace(new LogMessageGenerator(() => {
                return "Creating new lane";
            }));

            LaneMaster Lane = new LaneMaster();

            Lane.LaneName = NewLaneName.Text;
            //Lane.SystemIP = LaneSystemIP.Text;

            NewLaneName.Text = string.Empty;
            LaneSystemIP.Text = string.Empty;

            dcde.LaneMasters.Add(Lane);
            if (dcde.SaveChanges() > 0)
            {
                Task task = new Task(new Action(() => {
                    while (!LanesDictionary.TryAdd(Lane.LaneID, Lane))
                        LanesDictionary.TryAdd(Lane.LaneID, Lane);
                }), TaskCreationOptions.AttachedToParent);

                task.Start();
                task.Wait();
            }
            Fill_LanesDGV();
            ClearMessages();
            SuccessMsgText.Text = "Lane added successfully";
            SuccessMsg.Visible = true;


            logger.Trace(new LogMessageGenerator(() => {
                return "Lane " + Lane.LaneName + " created successfully";
            }));
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
}