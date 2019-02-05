using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ozeki.Camera;
using Ozeki.Media;
using DIMSContainerDBEFDLL;
using DIMSContainerDBEFDLL.EntityProxies;
using System.Security.Cryptography;
using System.Text;

namespace UlaWebAgsWF
{
    public partial class CamerasMaster : System.Web.UI.Page, IWebAGSClass
    {
        private DIMContainerDB_Revised_DevEntities dcde = null;
        private List<StreamerManager> StreamerManagerList = null;
        int ListeningPort = 6000;
        private string ErrorMsg = string.Empty;
        private List<sp_GetScreensFromRoleID_Result> UserAccessibleScreens = null;
        private UserMaster LoggedInUser = null;


        protected void Page_Load(object sender, EventArgs e)
        {
            dcde = new DIMContainerDB_Revised_DevEntities();

            StreamerManagerList = new List<StreamerManager>();

            for (int i = 1; i <= 7; i++)
            {
                System.Web.UI.WebControls.Panel panel = (System.Web.UI.WebControls.Panel)(this.Page.Master.FindControl("MainContent").FindControl("CamViewPnl" + i));
                panel.Visible = false;

                Label lbl = (Label)(this.Page.Master.FindControl("MainContent").FindControl("CamPosLbl" + i));
                lbl.Text = "";
                lbl.Visible = false;
            }

            if (HttpContext.Current.Session["RoleID"] != null && HttpContext.Current.Session["RoleID"].ToString() == "1")
            {
                CreateNewCamLinkPnl.Visible = true;
            }
            else
            {
                CreateNewCamLinkPnl.Visible = false;
            }

            if (dcde.CameraPositionMasters.ToList<CameraPositionMaster>().Count > 0 && dcde.LaneMasters.ToList<LaneMaster>().Count > 0)
            {
                CreateNewCamLink.Visible = true;
            }
            else
            {
                CreateNewCamLink.Visible = false;
            }

            if (!IsPostBack)
            {
                Response.Cache.SetExpires(System.DateTime.UtcNow.AddMinutes(-1));
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();

                this.Page.Title = "Camera Master";
                Fill_SelectLaneDDL();
            }
        }

        protected void Fill_SelectLaneDDL()
        {
            DataTable LanesDT = new DataTable();

            List<LaneMaster> list = dcde.LaneMasters.ToList<LaneMaster>();

            LanesDT.Columns.Add("LaneID", typeof(string));
            LanesDT.Columns.Add("LaneName", typeof(string));

            foreach (LaneMaster lane in list)
            {
                DataRow dr = LanesDT.NewRow();
                dr["LaneID"] = lane.LaneID.ToString();
                dr["LaneName"] = lane.LaneName.ToString();

                LanesDT.Rows.Add(dr);
            }

            SelectLaneDDL.DataValueField = "LaneID";
            SelectLaneDDL.DataTextField = "LaneName";
            SelectLaneDDL.DataSource = LanesDT.DefaultView;
            SelectLaneDDL.DataBind();
        }

        protected void BtnLaneDataSubmit_Click(object sender, EventArgs e)
        {
            int LaneID = Int32.Parse(SelectLaneDDL.SelectedValue.ToString());

            List<LaneData> LaneDataList = (from cdt in dcde.CameraDtlsTbls
                                           join cpm in dcde.CameraPositionMasters
                                           on
                                           cdt.PositionID equals cpm.PositionID
                                           where
                                           cdt.LaneID == LaneID
                                           select new LaneData
                                           {
                                               PositionID = cdt.PositionID,
                                               PositionName = cpm.PositionName,
                                               CameraID = cdt.CameraID,
                                               CameraIP = cdt.CameraIP,
                                               LaneID = cdt.LaneID,
                                               isCameraActive = cdt.Active
                                           }).ToList<LaneData>();

            if (LaneDataList.Count > 0)
            {
                foreach (LaneData ld in LaneDataList)
                {
                    if (ld.isCameraActive)
                    {
                        StreamerManager sm = new StreamerManager(ListeningPort++, 1, ld.CameraIP);
                        sm.InitCam();
                        StreamerManagerList.Add(sm);

                        System.Web.UI.WebControls.Panel panel = this.Page.Master.FindControl("MainContent").FindControl("CamViewPnl" + ld.PositionID.ToString()) as System.Web.UI.WebControls.Panel;

                        panel.Visible = true;
                        System.Web.UI.WebControls.Image image = (System.Web.UI.WebControls.Image)this.Page.Master.FindControl("MainContent").FindControl("CamViewImg" + ld.PositionID);
                        image.ImageUrl = "http://localhost:" + sm.Streamer.ListenPort;
                        image.Visible = true;

                        Label lbl = (Label)(this.Page.Master.FindControl("MainContent").FindControl("CamPosLbl" + ld.PositionID.ToString()));
                        lbl.Text = ld.PositionName.ToString();
                        lbl.Visible = true;
                    }
                }
            }
        }

        protected void NewCamBtnSubmit_Click1(object sender, EventArgs e)
        {
            //Page.Validate();
            if (Page.IsValid)
            {
                CameraDtlsTblProxy cameraDetails = new CameraDtlsTblProxy();
                int PositionID = Int32.Parse(CamPosDD.SelectedValue);
                int LaneID = Int32.Parse(CamLaneDD.SelectedValue);

                CameraPositionMaster CameraPositionMaster = dcde.CameraPositionMasters.Where(s => s.PositionID == PositionID).Select(t => t).FirstOrDefault();
                LaneMaster laneMaster = dcde.LaneMasters.Where(s => s.LaneID == LaneID).Select(t => t).FirstOrDefault();

                cameraDetails.CameraIP = txtCamIP.Text;
                cameraDetails.CameraPositionMaster = CameraPositionMaster;
                cameraDetails.PositionID = PositionID;
                cameraDetails.LaneID = LaneID;
                cameraDetails.Active = IsCamActive.Checked;
                dcde.CameraDtlsTbls.Add(cameraDetails);
                dcde.SaveChanges();
            }
        }

        protected void NewPosCreateBtn_Click1(object sender, EventArgs e)
        {
            //Page.Validate();
            if (Page.IsValid)
            {
                CameraPositionMasterProxy CameraPositionMasterProxy = new CameraPositionMasterProxy();
                CameraPositionMasterProxy.CameraDtlsTbls = null;
                CameraPositionMasterProxy.PositionName = txtPosName.Text;
                CameraPositionMasterProxy.PositionDescription = txtPosDesc.Text;
                CameraPositionMasterProxy.ContainerVisible = Int32.Parse(txtContVis.Text);
                CameraPositionMasterProxy.ImageIndex = null;

                dcde.CameraPositionMasters.Add(CameraPositionMasterProxy);
                dcde.SaveChanges();
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

    public class LaneData {
        public int PositionID { get; set; }
        public string PositionName { get; set; }
        public int CameraID { get; set; }
        public string CameraIP { get; set; }
        public int LaneID { get; set; }
        public bool isCameraActive { get; set; }
    }

    internal class StreamerManager
    {
        public MJPEGStreamer Streamer = null;
        private MediaConnector Connector = null;
        private IPCamera Camera = null;
        private bool _initiated = false;

        public StreamerManager(int ListeningPort, int FrameInterval, string CameraIP)
        {
            Streamer = new MJPEGStreamer(new OzConf_MJPEGStreamServer(ListeningPort, FrameInterval));
            Connector = new MediaConnector();
            Camera = new IPCamera(CameraIP, "admin", "admin", CameraTransportType.TCP, true);
        }

        public void InitCam()
        {
            if (!_initiated)
                _initiated = true;
            else
                return;

            try
            {
                if (Camera != null && Streamer != null)
                {
                    Connector.Connect(Camera.VideoChannel, Streamer.VideoChannel);
                    Camera.Start();
                    Streamer.ClientConnected += Streamer_ClientConnected;
                    Streamer.ClientDisconnected += Streamer_ClientDisconnected;
                }

                Streamer.Start();
            }
            catch (Exception ex)
            {
                _initiated = false;
            }
        }

        private void Streamer_ClientDisconnected(object sender, OzFramework2.OzEventArgs<OzFramework2.OzBaseMJPEGStreamConnection> e)
        {
            e.Item.StopStreaming();
        }

        private void Streamer_ClientConnected(object sender, OzFramework2.OzEventArgs<OzFramework2.OzBaseMJPEGStreamConnection> e)
        {
            e.Item.StartStreaming();
        }
    }
}