using DIMSContainerDBEFDLL;
using DIMSContainerDBEFDLL.EntityProxies;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UlaWebAgsWF
{
    public partial class DamageImages_temp : System.Web.UI.Page
    {
        protected DIMContainerDB_Revised_DevEntities dcde;
        protected ContainerTransactionProxy ct = null;
        protected DamageTransactionProxy dt = null;
        protected string DmgImgDirLoc = null;
        protected int TransactionID = 0;
        protected List<DamageTypeMasterProxy> dtm = null;
        private string ErrorMsg = string.Empty;
        private ServerUtilities utilities = null;
        private static Logger logger = LogManager.GetLogger("TransactionPreviewLogger", typeof(DamageImages_temp));

        protected void Page_Load(object sender, EventArgs e)
        {
            dcde = new DIMContainerDB_Revised_DevEntities();
            utilities = new ServerUtilities();

            try
            {
                logger.Info(new LogMessageGenerator(() => {
                    return "Starting to load transaction preview page";
                }));

                if (!IsPostBack)
                {
                    Response.Cache.SetExpires(System.DateTime.UtcNow.AddMinutes(-1));
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.Cache.SetNoStore();

                    this.Page.Title = "Container Transaction";
                    Label IsPreview = new Label();

                    RolID.Text = ((UserMasterProxy)HttpContext.Current.Session["LoggedInUser"]).DesignationID.ToString();

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["TransactionID"] as string) && Int32.TryParse(HttpContext.Current.Session["TransactionID"].ToString(), out TransactionID))
                    {
                        ct = (ContainerTransactionProxy)dcde.ContainerTransactions.Where(s => s.TransID == TransactionID).First();

                        if (ct != null)
                        {
                            ViewState["CurrentContainerTransactionProxyID"] = ct.TransID;
                            Cont_Num.Text = !string.IsNullOrEmpty(ct.ContainerCode) ? ct.ContainerCode : "EMPTY TRAILER";
                            Cont_Type.Text = ct.ContainerTypeID == (int)ServerUtilities.ContainerTypes.SIZE_20_FT ? "20FT" : ct.ContainerTypeID == (int)ServerUtilities.ContainerTypes.SIZE_40_FT ? "40FT" : "N/A";
                            Cont_Num.Visible = true;
                            lbl_Cont_Num.Visible = true;
                            Cont_Type.Visible = true;
                            lbl_Cont_Type.Visible = true;

                            DmgImgDirLoc = ct.DIRLocation.ToString();

                            if (!string.IsNullOrEmpty(DmgImgDirLoc) && (Directory.Exists(DmgImgDirLoc) && Directory.GetFiles(DmgImgDirLoc).Length > 0))
                            {
                                logger.Info(new LogMessageGenerator(() => {
                                    return "Generating container images from application server";
                                }));

                                FillContainersImgBtns((int)ct.ContainerTypeID == 1 ? ServerUtilities.ContainerTypes.SIZE_20_FT : ServerUtilities.ContainerTypes.SIZE_40_FT, Int32.Parse(ct.LaneID.ToString()), ct.TransID.ToString());
                                FillDamageOptionsCB();

                                if (((!string.IsNullOrEmpty(ct.ContainerDmgd.ToString())) && ct.ContainerDmgd == true && !string.IsNullOrEmpty(ct.DmgDtlsID.ToString())) || ((!string.IsNullOrEmpty(ct.Displayed.ToString()) && ct.Displayed) || (!string.IsNullOrEmpty(ct.CancelStatus.ToString()) && (bool)ct.CancelStatus)))
                                {
                                    List<DIMSContainerDBEFDLL.EntityProxies.DamageTransactionDetailProxy> DamageDetails = new List<DIMSContainerDBEFDLL.EntityProxies.DamageTransactionDetailProxy>();

                                    if ((!string.IsNullOrEmpty(ct.ContainerDmgd.ToString())) && ct.ContainerDmgd == true && !string.IsNullOrEmpty(ct.DmgDtlsID.ToString()))
                                    {
                                        dt = (DamageTransactionProxy)dcde.DamageTransactions.Single(s => s.DmgDtlsID == ct.DmgDtlsID);
                                        DamageDetails = (List<DamageTransactionDetailProxy>)dcde.DamageTransactions.Where(a => a.DmgDtlsID.Equals(dt.DmgDtlsID)).Select(b => b);
                                        Cont_Dmgd.Text = "Yes";
                                        Cont_Dmgd.Visible = true;
                                        lbl_Cont_Dmgd.Visible = true;

                                        FillDamageContents(DamageDetails);

                                        logger.Info(new LogMessageGenerator(() => {
                                            return "Damaged Container with Damage details ID: "+dt.DmgDtlsID;
                                        }));
                                    }

                                    CB_Damaged.Visible = false;
                                    isPreview.Text = "1";
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(ct.ContainerCode))
                                        CB_Damaged.Enabled = false;
                                    else
                                        CB_Damaged.Enabled = true;

                                    CB_Damaged.Visible = true;
                                    CB_Damaged.Enabled = true;
                                    isPreview.Text = "0";
                                }
                            }
                            else
                            {
                                noPreviewTxt.Text = "No container images available";


                                logger.Info(new LogMessageGenerator(() => {
                                    return "Container images not available for Container: " + ct.ContainerCode + ", Transaction ID: " + ct.TransID;
                                }));

                                noPreviewTxt.Visible = true;
                            }
                        }
                        else
                        {
                            HttpContext.Current.Session["ErrorMsg"] = "Invalid transaction request, contact system admin";


                            logger.Info(new LogMessageGenerator(() => {
                                return "No container transaction found in database with TransactionID:" + TransactionID;
                            }));

                            Response.Redirect(Request.UrlReferrer.ToString());
                        }
                    }
                    else
                    {
                        HttpContext.Current.Session["ErrorMsg"] = "Invalid transaction ID, contact system admin";

                        logger.Info(new LogMessageGenerator(() => {
                            return "Transaction ID is not valid, contact system administrator";
                        }));

                        Response.Redirect(Request.UrlReferrer.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Damage_Opt_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox DamagedCB = (CheckBox)Mainliteral.FindControl("CB_Damaged");

            foreach (Control c in All(Mainliteral.Controls).AsEnumerable().Where(a => a.ID.Contains("Panel")))
            {
                if (DamagedCB.Checked)
                {
                    ((Panel)c).Visible = true;
                }
                else
                {
                    ((Panel)c).Visible = false;
                }
            }
        }

        protected void FillContainersImgBtns(ServerUtilities.ContainerTypes ContainerType = ServerUtilities.ContainerTypes.NONE, int LaneID = 0, string TransID = "", DamageTransactionProxy dt = null)
        {
            try
            {
                if (ContainerType != ServerUtilities.ContainerTypes.NONE && LaneID > 0 && !string.IsNullOrEmpty(TransID))
                {
                    List<CameraPosition> CameraPositions = new List<CameraPosition>();
                    List<ContainerTrans> Trans = new List<ContainerTrans>();
                    
                    int TransactionID = Int32.Parse(TransID);
                    int ContPosNotVisible = 0;

                    if (ContainerType.Equals(ServerUtilities.ContainerTypes.SIZE_40_FT))
                    {
                        CameraPositions = (from cdtls in dcde.CameraDtlsTbls
                                           join
                                           cpms in dcde.CameraPositionMasters
                                           on
                                           cdtls.PositionID equals cpms.PositionID
                                           where
                                           cdtls.LaneID.Equals(LaneID)
                                           select new CameraPosition { CameraPositionID = cdtls.PositionID, CameraPositionName = cpms.PositionName }).ToList<CameraPosition>();
                    }

                    if (ContainerType.Equals(ServerUtilities.ContainerTypes.SIZE_20_FT))
                    {
                        Trans = (from ct in dcde.ContainerTransactions
                                 where
                                 (from ct1 in dcde.ContainerTransactions
                                  where
                                  ct1.TransID == TransactionID
                                  select ct1.TrailerTransID).Contains(ct.TrailerTransID)
                                 select new ContainerTrans { TransID = ct.TransID, SequenceOfContainers = (Int32)ct.SequnceOfContan }).ToList<ContainerTrans>();

                        if (Trans.Count >= 2)
                        {
                            foreach (var item in Trans)
                            {
                                if (item.SequenceOfContainers == 1 && item.TransID == Int32.Parse(TransID))
                                    ContPosNotVisible = 2;

                                else if (item.SequenceOfContainers == 2 && item.TransID == Int32.Parse(TransID))
                                    ContPosNotVisible = 1;

                                CameraPositions = (from cd in dcde.CameraDtlsTbls
                                                   join
                                                   cm in dcde.CameraPositionMasters
                                                   on
                                                   cd.PositionID equals cm.PositionID
                                                   where
                                                   cd.LaneID == LaneID && cm.ContainerVisible != ContPosNotVisible && cd.Active
                                                   select new CameraPosition { CameraPositionID = cd.PositionID, CameraPositionName = cm.PositionName }).ToList<CameraPosition>();
                            }
                        }
                        else if (Trans.Count > 0)
                        {
                            CameraPositions = (from cdtls in dcde.CameraDtlsTbls
                                               join
                                               cpms in dcde.CameraPositionMasters
                                               on
                                               cdtls.PositionID equals cpms.PositionID
                                               where
                                               cdtls.LaneID.Equals(LaneID) && cdtls.Active
                                               select new CameraPosition { CameraPositionID = cdtls.PositionID, CameraPositionName = cpms.PositionName }).ToList<CameraPosition>();
                        }
                    }

                    if (CameraPositions.Count > 0)
                    {
                        foreach (CameraPosition pos in CameraPositions)
                        {
                            Mainliteral.Controls.Add(new LiteralControl("<div style='float:left; margin: auto; height: auto; width: auto'>"));
                            ImageButton ImgButton = new ImageButton();
                            ImgButton.ID = "imgFromDB" + pos.CameraPositionID;
                            ImgButton.Height = new Unit(350, UnitType.Pixel);
                            ImgButton.Width = new Unit(390, UnitType.Pixel);
                            ImgButton.CssClass = "img-responsive";
                            ImgButton.Visible = true;

                            if (File.Exists(DmgImgDirLoc + pos.CameraPositionID + ".jpg"))
                            {
                                FileStream fileStream = new FileStream(DmgImgDirLoc + pos.CameraPositionID + ".jpg", FileMode.Open, FileAccess.Read);
                                BinaryReader binaryReader = new BinaryReader(fileStream);
                                ImgButton.ImageUrl = "data:image/jpeg;base64," + Convert.ToBase64String(binaryReader.ReadBytes((Int32)fileStream.Length));
                                binaryReader.Close();
                                fileStream.Close();
                            }

                            Mainliteral.Controls.Add(ImgButton);
                            Mainliteral.Controls.Add(new LiteralControl("</div>"));
                            Mainliteral.Controls.Add(new LiteralControl("<br />"));
                            Mainliteral.Controls.Add(new LiteralControl("<div class='text-center'>"));
                            Label PosName = new Label();
                            PosName.CssClass = "h4";
                            PosName.Text = pos.CameraPositionName;
                            PosName.Visible = true;
                            Mainliteral.Controls.Add(PosName);
                            Mainliteral.Controls.Add(new LiteralControl("</div>"));
                            Mainliteral.Controls.Add(new LiteralControl("<br />"));
                            Panel DmgPanel = new Panel();
                            DmgPanel.ID = "PanelDmgOps" + pos.CameraPositionID;
                            DmgPanel.CssClass = "panel-default";
                            DmgPanel.Attributes.Add("float", "left");
                            DmgPanel.Attributes.Add("margin-top", "20px");
                            DmgPanel.Width = new Unit(390, UnitType.Pixel);
                            DmgPanel.Visible = true;
                            TextBox DmgCmnt1 = new TextBox();
                            DmgCmnt1.ID = "DmgImgCmnt" + pos.CameraPositionID;
                            DmgCmnt1.CssClass = "form-control";
                            DmgCmnt1.Attributes.Add("placeholder", "Enter damage comments...");
                            DmgPanel.Controls.Add(DmgCmnt1);
                            CheckBoxList DmgChkList1 = new CheckBoxList();
                            DmgChkList1.ID = "DmgImgCBL" + pos.CameraPositionID;
                            DmgChkList1.CssClass = "form-control";
                            DmgPanel.Controls.Add(DmgChkList1);
                            Label CamPosID = new Label();
                            CamPosID.ID = "CamPosIDHidden"+pos.CameraPositionID;
                            CamPosID.Text = string.Empty;
                            CamPosID.Visible = false;
                            DmgPanel.Controls.Add(CamPosID);
                            DmgPanel.Controls.Add(new LiteralControl("<br />"));
                            DmgPanel.Controls.Add(new LiteralControl("<br />"));
                            DmgPanel.Visible = false;
                            Mainliteral.Controls.Add(DmgPanel);
                            Mainliteral.Controls.Add(new LiteralControl("</div>"));
  
                        }

                        Mainliteral.Controls.Add(new LiteralControl("<div style='float:left; margin: auto; height: auto; width: auto'>"));
                        Panel CommonDmgPanel = new Panel();
                        CommonDmgPanel.ID = "PanelCommonDetails";
                        CommonDmgPanel.CssClass = "panel-default";
                        CommonDmgPanel.Attributes.Add("float", "left");
                        CommonDmgPanel.Attributes.Add("margin-top", "20px");
                        CommonDmgPanel.Width = new Unit(390, UnitType.Pixel);
                        CommonDmgPanel.Visible = true;
                        TextBox DmgCmnt = new TextBox();
                        DmgCmnt.ID = "CommonRemark";
                        DmgCmnt.CssClass = "form-control";
                        DmgCmnt.Attributes.Add("placeholder", "Enter damage comments...");
                        CommonDmgPanel.Controls.Add(DmgCmnt);
                        CheckBoxList DmgChkList = new CheckBoxList();
                        DmgChkList.ID = "CommonDmgCBL";
                        DmgChkList.CssClass = "form-control";
                        CommonDmgPanel.Controls.Add(DmgChkList);
                        CommonDmgPanel.Controls.Add(new LiteralControl("<br />"));
                        CommonDmgPanel.Controls.Add(new LiteralControl("<br />"));
                        CommonDmgPanel.Visible = false;
                        Mainliteral.Controls.Add(CommonDmgPanel);
                        Mainliteral.Controls.Add(new LiteralControl("</div>"));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void FillDamageContents(List<DIMSContainerDBEFDLL.EntityProxies.DamageTransactionDetailProxy> dtd)
        {
            foreach (DamageTransactionDetailProxy damage in dtd)
            {
                TextBox TargetTB = null;
                CheckBoxList TargetCBL = null;

                if (damage.IsCommonRemark)
                {
                    TargetTB = (TextBox)Mainliteral.FindControl("CommonRemark");
                    TargetCBL = (CheckBoxList)Mainliteral.FindControl("CommonDmgCBL");
                }
                else
                {
                    TargetTB = (TextBox)(Mainliteral.FindControl("DmgImgCmnt" + damage.CamPosID));
                    TargetCBL = (CheckBoxList)(Mainliteral.FindControl("DmgImgCBL" + damage.CameraPositionMaster));
                }

                TargetTB.Text = damage.DamageRemark;
                TargetTB.Visible = true;
                TargetTB.Enabled = false;

                foreach (string DmgType in damage.DamageTypes.Split(new char[] { ',' }))
                {
                    foreach (ListItem li in TargetCBL.Items)
                    {
                        if (li.Text.Equals(DmgType))
                        {
                            li.Selected = true;
                            li.Enabled = false;
                        }
                    }
                }

                TargetCBL.Parent.Visible = true;
            }
        }

        protected void FillDamageOptionsCB()
        {
            try
            {
                List<DamageTypeMaster> DmgTypes = dcde.DamageTypeMasters.ToList<DamageTypeMaster>();

                foreach (Control cbl in this.All((ControlCollection)this.Master.FindControl("MainContent").Controls).Where(s => s.GetType().ToString().Equals(typeof(CheckBoxList).ToString())).Select(s => s).ToList())
                {
                    CheckBoxList CurrentCbl = cbl as CheckBoxList;
                    CurrentCbl.DataTextField = "DmgTypeName";
                    CurrentCbl.DataValueField = "DmgTypeid";
                    CurrentCbl.DataSource = DmgTypes;
                    CurrentCbl.DataBind();
                    CurrentCbl.RepeatDirection = RepeatDirection.Vertical;
                }
            }
            catch (Exception ex)
            { }
        }

        public IEnumerable<Control> All(ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control.HasControls())
                {
                    foreach (Control child in control.Controls)
                    {
                        yield return child;
                    }

                    yield return control;
                }
                else
                {
                    yield return control;
                }
            }
        }

        protected void SubmitConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                int TransactionID = Int32.Parse(ViewState["CurrentContainerTransactionProxyID"].ToString());

                ContainerTransactionProxy CurrentTransaction = (ContainerTransactionProxy)dcde.ContainerTransactions.Where(a => a.TransID.Equals(TransactionID)).FirstOrDefault();

                logger.Info(new LogMessageGenerator(() => {
                    return "Submitting verified transaction: " + CurrentTransaction.ToString();
                }));

                if (CB_Damaged.Checked)
                {
                    dcde.DamageTransactions.Add(new DamageTransactionProxy());
                    dcde.SaveChanges();

                    DamageTransactionProxy RecentDamageTransactionProxy = (DamageTransactionProxy)dcde.DamageTransactions.OrderByDescending(a => a.DmgDtlsID).FirstOrDefault();

                    List<Control> DmgRemarksList = All(Mainliteral.Controls).AsEnumerable().Where(a => a.ID.Contains("PanelDmgOps") && a.GetType().Equals(typeof(Panel))).Select(a => a).ToList();
                    List<DamageTransactionDetailProxy> DmgDetails = new List<DamageTransactionDetailProxy>();


                    logger.Info(new LogMessageGenerator(() => {
                        return "\nDamaged transaction: " + RecentDamageTransactionProxy.ToString() + "\n"+DmgDetails.ToString();
                    }));

                    foreach (Control control in DmgRemarksList)
                    {
                        Panel CurrentPanel = (Panel)control;

                        DamageTransactionDetailProxy dtd = new DamageTransactionDetailProxy();
                        dtd.DmgDtlsID = RecentDamageTransactionProxy.DmgDtlsID;
                        dtd.DamageRemark = ((TextBox)All(CurrentPanel.Controls).AsEnumerable().Where(a => a.ID.Contains("DmgImgCmnt")).First()).Text;

                        CheckBoxList DmgTypesCBL = (CheckBoxList)All(CurrentPanel.Controls).AsEnumerable().Where(a => a.ID.Contains("DmgImgCBL")).First();

                        dtd.DamageTypes = string.Empty;

                        foreach (ListItem li in DmgTypesCBL.Items)
                        {
                            if (li.Selected)
                            {
                                if (string.IsNullOrEmpty(dtd.DamageTypes))
                                    dtd.DamageTypes += li.Text;
                                else
                                    dtd.DamageTypes += "," + li.Text;
                            }
                        }

                        dtd.CamPosID = Int32.Parse(((Label)All(CurrentPanel.Controls).AsEnumerable().Where(a => a.ID.Contains("CamPosIDHidden")).First()).Text);
                        dtd.IsCommonRemark = false;
                        DmgDetails.Add(dtd);
                    }

                    DamageTransactionDetailProxy CommonDmgTransDetail = new DamageTransactionDetailProxy();
                    Panel CommonDmgDetailsPnl = (Panel)Mainliteral.FindControl("PanelCommonDetails");
                    CommonDmgTransDetail.DmgDtlsID = RecentDamageTransactionProxy.DmgDtlsID;
                    CommonDmgTransDetail.DamageRemark = ((TextBox)CommonDmgDetailsPnl.FindControl("CommonRemark")).Text;
                    CommonDmgTransDetail.DamageTypes = string.Empty;

                    CheckBoxList CommonDmgTypes = (CheckBoxList)CommonDmgDetailsPnl.FindControl("CommonDmgCBL");

                    foreach (ListItem li in CommonDmgTypes.Items)
                    {
                        if (li.Selected)
                        {
                            if (string.IsNullOrEmpty(CommonDmgTransDetail.DamageTypes))
                                CommonDmgTransDetail.DamageTypes += li.Text;
                            else
                                CommonDmgTransDetail.DamageTypes += "," + li.Text;
                        }
                    }

                    CommonDmgTransDetail.CamPosID = null;
                    CommonDmgTransDetail.IsCommonRemark = true;

                    DmgDetails.Add(CommonDmgTransDetail);
                    CurrentTransaction.DmgDtlsID = RecentDamageTransactionProxy.DmgDtlsID;
                    CurrentTransaction.ContainerDmgd = true;
                    dcde.DamageTransactionDetails.AddRange(DmgDetails);
                }
                else
                {
                    CurrentTransaction.DmgDtlsID = null;
                    CurrentTransaction.ContainerDmgd = false;
                }

                CurrentTransaction.Displayed = true;
                CurrentTransaction.CancelStatus = false;

                if (dcde.SaveChanges() > 1)
                {
                    HttpContext.Current.Session["SuccessMsg"] = "Transaction saved successfully";

                    logger.Info(new LogMessageGenerator(() => {
                        return "Transaction saved successfully, redirecting to dashboard";
                    }));

                    Response.Redirect("Default.aspx");
                }
                else
                {
                    logger.Info(new LogMessageGenerator(() => {
                        return "Transaction can not be submitted at this moment, try again later";
                    }));

                    FailureMsgTxt.Text = "Transaction can not be saved at this moment, try again later";
                    FailureMsgTxt.Visible = true;
                    FailureMsg.Visible = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void SubmitCancel_Click(object sender, EventArgs e)
        {
            try
            {
                ContainerTransactionProxy TransToBeCancelled = null;

                if (ct != null)
                {
                    TransToBeCancelled = (ContainerTransactionProxy)dcde.ContainerTransactions.Where(a => a.TransID.Equals(ct.TransID)).SingleOrDefault();

                    logger.Info(new LogMessageGenerator(() => {
                        return "Cancelling transaction:\n"+TransToBeCancelled.ToString();
                    }));
                }
                else
                {
                    int PersistedTransID = Int32.Parse(ViewState["CurrentContainerTransactionProxyID"].ToString());
                    TransToBeCancelled = (ContainerTransactionProxy)dcde.ContainerTransactions.Where(a => a.TransID.Equals(PersistedTransID)).SingleOrDefault();
                }

                TransToBeCancelled.Displayed = false;
                TransToBeCancelled.CancelStatus = true;
                TransToBeCancelled.ContainerDmgd = null;
                TransToBeCancelled.DmgDtlsID = 0;

                if (dcde.SaveChanges() > 0)
                {
                    HttpContext.Current.Session["SuccessMsg"] = "Transaction cancelled successfully";

                    logger.Info(new LogMessageGenerator(() => {
                        return "Transaction cancelled successfully, redirecting to dashboard";
                    }));

                    Response.Redirect("Default.aspx", true);
                }
                else
                {
                    FailureMsgTxt.Text = "Transaction can not be canceled at this moment, try again later";

                    logger.Info(new LogMessageGenerator(() => {
                        return "Transaction can not be cancelled at this moment, try again later";
                    }));

                    FailureMsgTxt.Visible = true;
                    FailureMsg.Visible = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void btnSubmitAllCnf_Click(object sender, EventArgs e)
        {
            try
            {
                logger.Info(new LogMessageGenerator(() => {
                    return "Clearing all the previous transactions";
                }));

                List<ContainerTransactionProxy> UnclearedTrans = (List<ContainerTransactionProxy>)dcde.ContainerTransactions.Where(a => !a.Displayed && !(bool)a.CancelStatus).Select(b => b);

                foreach (ContainerTransactionProxy ct in UnclearedTrans)
                {
                    ct.Displayed = true;
                }

                if (dcde.SaveChanges() > 0)
                {
                    HttpContext.Current.Session["SuccessMsg"] = "Transaction saved successfully";

                    logger.Info(new LogMessageGenerator(() => {
                        return "All the previous transactions cleared successfully";
                    }));

                    Response.Redirect("Default.aspx", true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void BtnBack_Click(object sender, EventArgs e)
        {
            logger.Info(new LogMessageGenerator(() => {
                return "Redirecting to dashboard";
            }));

            Response.Redirect("Default.aspx", true);
        }

        protected void btnDownloadImgs_Click(object sender, EventArgs e)
        {
            BackgroundWorker ImageDownloadBW = new BackgroundWorker();
            ImageDownloadBW.DoWork += ImageDownloadBW_DoWork;
            ImageDownloadBW.WorkerReportsProgress = false;

            ImageDownloadBW.RunWorkerAsync();
        }
        
        private void ImageDownloadBW_DoWork(object sender, DoWorkEventArgs e)
        {
            if (ViewState["CurrentContainerTransactionProxyID"] != null)
            {
                logger.Info(new LogMessageGenerator(() => {
                    return "Creating request for container image download for Transaction ID: " + ct.TransID + " from system IP: " + HttpContext.Current.Session["SysIP"].ToString();
                }));
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Page.ResolveUrl("~/ImageDownloadHandler.ashx"));
                request.Headers.Add("TransID", ViewState["CurrentContainerTransactionProxyID"].ToString());

                if (HttpContext.Current.Session["PositionIDs"] != null)
                {
                    request.Headers.Add("PositionIDs", HttpContext.Current.Session["PositionIDs"].ToString());
                    HttpContext.Current.Session.Remove("PositionIDs");
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.Headers.HasKeys())
                {
                    if (response.Headers["Download"].ToString().Equals("Successful"))
                    {
                        logger.Info(new LogMessageGenerator(() => {
                            return "Downloaded image successfully for transaction ID: " + ct.TransID;
                        }));

                        SuccessMsgTxt.Text = "Images download successful";
                        SuccessMsg.Visible = true;
                    }
                    else
                    {
                        logger.Info(new LogMessageGenerator(() => {
                            return "No images found for transaction ID: " + ct.TransID;
                        }));

                        FailureMsgTxt.Text = "No images found";
                        FailureMsg.Visible = true;
                    }
                }
            }
        }
    }
}