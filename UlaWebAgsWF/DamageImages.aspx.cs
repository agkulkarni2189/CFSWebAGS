using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using DIMSContainerDBEFDLL;
using DIMSContainerDBEFDLL.EntityProxies;

namespace UlaWebAgsWF
{
    public partial class DamageImages : System.Web.UI.Page, IWebAGSClass
    {
        protected DIMContainerDB_Revised_DevEntities dcde;
        protected ContainerTransactionProxy ct = null;
        protected DamageTransactionProxy dt = null;
        protected string DmgImgDirLoc = null;
        protected int TransactionID = 0;
        protected List<DamageTypeMasterProxy> dtm = null;
        private string ErrorMsg = string.Empty;
        private ServerUtilities utilities = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            dcde = new DIMContainerDB_Revised_DevEntities();
            utilities = new ServerUtilities();

            if (!IsPostBack)
            {
                Response.Cache.SetExpires(System.DateTime.UtcNow.AddMinutes(-1));
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();

                this.Page.Title = "Container Transaction";
                RolID.Text = ((UserMasterProxy)HttpContext.Current.Session["LoggedInUser"]).DesignationID.ToString();
                FillDamageOptionsCB();
            }

            try
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["TransactionID"] as string) && Int32.TryParse(Request.QueryString["TransactionID"].ToString(), out TransactionID))
                {
                    ct = (ContainerTransactionProxy)dcde.ContainerTransactions.Where(s => s.TransID == TransactionID).First();

                    if (ct != null)
                    {
                        Cont_Num.Text = !string.IsNullOrEmpty(ct.ContainerCode) ? ct.ContainerCode : "EMPTY TRAILER";
                        Cont_Type.Text = ct.ContainerTypeID == (int)ServerUtilities.ContainerTypes.SIZE_20_FT ? "20FT" : ct.ContainerTypeID == (int)ServerUtilities.ContainerTypes.SIZE_40_FT ? "40FT" : "N/A";
                        Cont_Num.Visible = true;
                        lbl_Cont_Num.Visible = true;
                        Cont_Type.Visible = true;
                        lbl_Cont_Type.Visible = true;
                        DmgImgDirLoc = ct.DIRLocation.ToString();

                        if (!string.IsNullOrEmpty(DmgImgDirLoc) && (Directory.Exists(DmgImgDirLoc) && Directory.GetFiles(DmgImgDirLoc).Length > 0))
                        {
                            FillContainersImgBtns((int)ct.ContainerTypeID == 1 ? ServerUtilities.ContainerTypes.SIZE_20_FT : ServerUtilities.ContainerTypes.SIZE_40_FT, Int32.Parse(ct.LaneID.ToString()), ct.TransID.ToString());

                            if (((!string.IsNullOrEmpty(ct.ContainerDmgd.ToString())) && ct.ContainerDmgd == true && !string.IsNullOrEmpty(ct.DmgDtlsID.ToString())) || ((!string.IsNullOrEmpty(ct.Displayed.ToString()) && ct.Displayed) || (!string.IsNullOrEmpty(ct.CancelStatus.ToString()) && (bool)ct.CancelStatus)))
                            {
                                if ((!string.IsNullOrEmpty(ct.ContainerDmgd.ToString())) && ct.ContainerDmgd == true && !string.IsNullOrEmpty(ct.DmgDtlsID.ToString()))
                                {
                                    dt = (DIMSContainerDBEFDLL.EntityProxies.DamageTransactionProxy)dcde.DamageTransactions.Single(s => s.DmgDtlsID == ct.DmgDtlsID);
                                    Cont_Dmgd.Text = "Yes";
                                    Cont_Dmgd.Visible = true;
                                    lbl_Cont_Dmgd.Visible = true;

                                    FillDamageContents(DmgImgCmnt1, 1, dt.RemarkCam1, dt.DmgdTypeCam1.Split(new char[] { ',' }));
                                    FillDamageContents(DmgImgCmnt2, 2, dt.RemarkCam2, dt.DmgdTypeCam2.Split(new char[] { ',' }));
                                    FillDamageContents(DmgImgCmnt3, 3, dt.RemarkCam3, dt.DmgdTypeCam3.Split(new char[] { ',' }));
                                    FillDamageContents(DmgImgCmnt4, 4, dt.RemarkCam4, dt.DmgdTypeCam4.Split(new char[] { ',' }));
                                    FillDamageContents(DmgImgCmnt5, 5, dt.RemarkCam5, dt.DmgdTypeCam5.Split(new char[] { ',' }));
                                    FillDamageContents(DmgImgCmnt6, 6, dt.RemarkCam6, dt.DmgdTypeCam6.Split(new char[] { ',' }));
                                    FillDamageContents(DmgImgCmnt7, 7, dt.RemarkCam7, dt.DmgdTypeCam7.Split(new char[] { ',' }));
                                    FillDamageContents(DmgImgCmnt8, 8, dt.CommonRemark, dt.CommonDmgTypes.Split(new char[] { ',' }));

                                    PopDamageDetails(true, true);
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
                                isPreview.Text = "0";
                            }
                        }
                        else
                        {
                            noPreviewTxt.Text = "No container images available";
                            noPreviewTxt.Visible = true;
                        }
                    }
                    else
                    {
                        HttpContext.Current.Session["ErrorMsg"] = "Invalid transaction request, contact system admin";
                        Response.Redirect(Request.UrlReferrer.ToString());
                    }
                }
                else
                {
                    HttpContext.Current.Session["ErrorMsg"] = "Invalid transaction ID, contact system admin";
                    Response.Redirect(Request.UrlReferrer.ToString());
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

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void FillDamageContents(TextBox tb, int ListBoxNum, string Remark, string[] DmgTypes)
        {
            try
            {
                if (!string.IsNullOrEmpty(Remark))
                    tb.Text = Remark;
                else
                    tb.Text = "No remarks entered";

                tb.Enabled = false;

                if (DmgTypes.Length > 0)
                {
                    CheckBoxList CurrentLB = (CheckBoxList)this.Master.FindControl("MainContent").FindControl("DmgImgCBL" + ListBoxNum);

                    foreach (string DmgType in DmgTypes)
                    {
                        foreach (ListItem li in CurrentLB.Items)
                        {
                            if (li.Text == DmgType.Trim())
                                li.Selected = true;

                            li.Enabled = false;
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        protected void PopDamageDetails(bool ToBeDisplayed, bool ContDmgd = false)
        {
            try
            {
                if (ToBeDisplayed)
                {
                    foreach (Control c in All((ControlCollection)this.Master.FindControl("MainContent").Controls).Where(s => s.GetType().ToString().Equals(typeof(Panel).ToString())))
                    {
                        if (c.ID.ToString().TrimEnd(c.ID.ToString()[c.ID.ToString().Length - 1]).Equals("DmgOps"))
                        {
                            Panel pnl = (Panel)c;

                            foreach (Control imgbtn in All((ControlCollection)this.Master.FindControl("MainContent").Controls).Where(s => s.GetType().ToString().Equals(typeof(ImageButton).ToString()) && s.ID.ToString().TrimEnd(s.ID.ToString()[s.ID.ToString().Length - 1]).Equals("imgFromDB")).Select(s => s).ToList())
                            {
                                ImageButton imageButton = (ImageButton)imgbtn;

                                if (pnl.ID.ToCharArray()[pnl.ID.ToCharArray().Length - 1].Equals(imageButton.ID.ToCharArray()[imageButton.ID.ToCharArray().Length - 1]) && imageButton.Visible && !string.IsNullOrEmpty(imageButton.ImageUrl))
                                {
                                    pnl.Visible = true;
                                }
                            }
                        }
                    }

                    if (!ContDmgd)
                    {
                        CmnRemPnl.Visible = false;
                        CommonRemark.Visible = false;
                    }
                    else
                    {
                        CmnRemPnl.Visible = true;
                        CommonRemark.Text = "Common Remarks";
                        CommonRemark.Visible = true;
                    }
                }
                else
                {
                    foreach (Control c in All((ControlCollection)this.Master.FindControl("MainContent").Controls).Where(s => s.GetType().ToString().Equals(typeof(Panel).ToString())))
                    {
                        if (c.ID.ToString().TrimEnd(c.ID.ToString()[c.ID.ToString().Length - 1]).Equals("DmgOps"))
                        {
                            Panel pnl = (Panel)c;

                            pnl.Visible = false;
                        }
                    }

                    CmnRemPnl.Visible = false;
                    CommonRemark.Text = "";
                    CommonRemark.Visible = false;
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected void CB_Damaged_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_Damaged.Checked)
            {
                PopDamageDetails(true, true);
            }
            else
            {
                PopDamageDetails(false);
            }
        }

        protected void FillContainersImgBtns(ServerUtilities.ContainerTypes ContainerType, int LaneID, string TransID)
        {
            try
            {
                List<CameraPosition> CameraPositions = new List<CameraPosition>();
                List<ContainerTrans> Trans = new List<ContainerTrans>();
                int TransactionID = Int32.Parse(TransID);
                StringBuilder PositionIDStringBuilder = new StringBuilder();

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
                        string name = "imgFromDB" + pos.CameraPositionID;

                        ImageButton target = (ImageButton)this.Master.FindControl("MainContent").FindControl("imgFromDB" + pos.CameraPositionID);
                        Label label = (Label)this.Master.FindControl("MainContent").FindControl("Pos" + pos.CameraPositionID);

                        if (File.Exists(DmgImgDirLoc + pos.CameraPositionID + ".jpg"))
                        {
                            FileStream fileStream = new FileStream(DmgImgDirLoc + pos.CameraPositionID + ".jpg", FileMode.Open, FileAccess.Read);
                            BinaryReader binaryReader = new BinaryReader(fileStream);
                            target.ImageUrl = "data:image/jpeg;base64," + Convert.ToBase64String(binaryReader.ReadBytes((Int32)fileStream.Length));
                            label.Text = pos.CameraPositionName;
                            label.Visible = true;
                            binaryReader.Close();
                            fileStream.Close();
                            target.Visible = true;

                            if (PositionIDStringBuilder.Length <= 0)
                                PositionIDStringBuilder.Append(pos.CameraPositionID.ToString());
                            else
                                PositionIDStringBuilder.Append("," + pos.CameraPositionID.ToString());
                        }
                    }

                    if (PositionIDStringBuilder.Length > 0)
                        HttpContext.Current.Session["PositionIDs"] = PositionIDStringBuilder.ToString();
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            List<CheckBoxList> CheckBoxLists = new List<CheckBoxList>();
            List<TextBox> TextBoxes = new List<TextBox>();

            for (int i = 1; i <= 7; i++)
            {
                CheckBoxList cbl = new CheckBoxList();
                cbl = this.Page.Master.FindControl("MainContent").FindControl("DmgImgCBL" + i) as CheckBoxList;

                TextBox tb = new TextBox();
                tb = this.Page.Master.FindControl("MainContent").FindControl("DmgImgCmnt1" + i) as TextBox;

                CheckBoxLists.Add(cbl);
                TextBoxes.Add(tb);
            }

            try
            {
                if (CB_Damaged.Visible && CB_Damaged.Checked)
                {
                    string[] DmgRemarks = new string[TextBoxes.Count];
                    string[] DmgTypes = new string[CheckBoxLists.Count];

                    for (int i = 0; i < TextBoxes.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(TextBoxes[i].Text))
                            DmgRemarks[i] = TextBoxes[i].Text;
                    }

                    for (int i = 0; i < CheckBoxLists.Count; i++)
                    {
                        foreach (ListItem li in CheckBoxLists[i].Items)
                        {
                            if (li.Selected)
                            {
                                if (!string.IsNullOrEmpty(DmgTypes[i]))
                                    DmgTypes[i] += "," + li.Text;
                                else
                                    DmgTypes[i] += li.Text;
                            }
                        }
                    }

                    DamageTransactionProxy NewDmgTrans = new DamageTransactionProxy();
                    NewDmgTrans.RemarkCam1 = DmgRemarks[1];
                    NewDmgTrans.RemarkCam2 = DmgRemarks[2];
                    NewDmgTrans.RemarkCam3 = DmgRemarks[3];
                    NewDmgTrans.RemarkCam4 = DmgRemarks[4];
                    NewDmgTrans.RemarkCam5 = DmgRemarks[5];
                    NewDmgTrans.RemarkCam6 = DmgRemarks[6];
                    NewDmgTrans.RemarkCam7 = DmgRemarks[7];
                    NewDmgTrans.CommonRemark = DmgRemarks[8];

                    NewDmgTrans.DmgdTypeCam1 = DmgTypes[1];
                    NewDmgTrans.DmgdTypeCam2 = DmgTypes[2];
                    NewDmgTrans.DmgdTypeCam3 = DmgTypes[3];
                    NewDmgTrans.DmgdTypeCam4 = DmgTypes[4];
                    NewDmgTrans.DmgdTypeCam5 = DmgTypes[5];
                    NewDmgTrans.DmgdTypeCam6 = DmgTypes[6];
                    NewDmgTrans.DmgdTypeCam7 = DmgTypes[7];
                    NewDmgTrans.CommonDmgTypes = DmgTypes[8];

                    dcde.DamageTransactions.Add(NewDmgTrans);
                    dcde.SaveChanges();

                    DamageTransactionProxy DamageTransactionProxy = (DamageTransactionProxy)dcde.DamageTransactions.OrderByDescending(s => s.DmgDtlsID).FirstOrDefault();

                    ct.DmgDtlsID = DamageTransactionProxy.DmgDtlsID;
                    ct.ContainerDmgd = true;
                    dcde.SaveChanges();
                }
                else
                {
                    ct.Displayed = true;
                    ct.ContainerDmgd = false;
                    ct.CancelStatus = false;
                    ct.DmgDtlsID = 0;

                    dcde.SaveChanges();
                }

                Session["TransactionID"] = null;
                Response.Redirect("Default.aspx");
            }
            catch (Exception ex) { }
        }

        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            try
            {
                ct.Displayed = true;
                ct.CancelStatus = true;
                ct.ContainerDmgd = null;
                ct.DmgDtlsID = 0;

                dcde.SaveChanges();
                Response.Redirect("Default.aspx");
            }
            catch (Exception ex)
            {

            }
        }

        protected void btnSubmitAllCnf_Click(object sender, EventArgs e)
        {
            try
            {
                List<ContainerTransactionProxy> UnclearedTrans = (List<ContainerTransactionProxy>)dcde.ContainerTransactions.Where(a => !a.Displayed && !(bool)a.CancelStatus).Select(b => b);

                foreach (ContainerTransactionProxy ct in UnclearedTrans)
                {
                    ct.Displayed = true;
                }

                dcde.SaveChanges();
                Response.Redirect("Default.aspx");
            }
            catch (Exception ex)
            {

            }
        }

        protected void btnDownloadImgs_Click(object sender, EventArgs e)
        {
            if (ct != null)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Page.ResolveUrl("~/ImageDownloadHandler.ashx"));
                request.Headers.Add("TransID", ct.TransID.ToString());

                if (HttpContext.Current.Session["PositionIDs"] != null)
                {
                    request.Headers.Add("PositionIDs", HttpContext.Current.Session["PositionIDs"].ToString());
                    HttpContext.Current.Session.Remove("PositionIDs");
                }
                 
                HttpWebResponse response  = (HttpWebResponse)request.GetResponse();

                if (response.Headers.HasKeys())
                {
                    if (response.Headers["Download"].ToString().Equals("Successful"))
                    {
                        SuccessMsgTxt.Text = "Images download successful";
                        SuccessMsg.Visible = true;
                    }
                    else
                    {
                        FailureMsgTxt.Text = "No images found";
                        FailureMsg.Visible = true;
                    }
                }
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

    class CameraPosition {
        public int TransID { get; set; }
        public int CameraPositionID { get; set; }
        public string CameraPositionName { get; set; }
    }

    class ContainerTrans {
        public int TransID { get; set; }
        public int SequenceOfContainers { get; set; }
        public int TrailerTransID { get; set; }
    }
}