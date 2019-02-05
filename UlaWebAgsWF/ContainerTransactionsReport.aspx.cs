using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DIMSContainerDBEFDLL.EntityProxies;
using DIMSContainerDBEFDLL;

namespace UlaWebAgsWF
{
    public partial class ContainerTransactionProxysReport : System.Web.UI.Page, IWebAGSClass
    {
        private DIMContainerDB_Revised_DevEntities dcde = null;
        private DataTable ContainerTransDGVDT = null;
        private string ErrorMsg = string.Empty;
        private List<sp_GetScreensFromRoleID_Result> UserAccessibleScreens = null;
        private UserMasterProxy LoggedInUser = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            dcde = new DIMContainerDB_Revised_DevEntities();

            if (!IsPostBack)
            {
                Response.Cache.SetExpires(System.DateTime.UtcNow.AddMinutes(-1));
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();

                this.Page.Title = "Transactions History";
                HttpContext.Current.Session["IsOnlyPreviousDayDataRequired"] = true;
                FillContainerTransactionProxyGV(FilterContainerTransDGVData(true));
            }
        }

        protected List<ContainerTransactionProxy> FilterContainerTransDGVData(bool IsOnlyPrevDayDataRequired)
        {
            bool Damaged = false;

            if (Int32.Parse(dmgtypedd.SelectedValue) > 0)
            {
                HttpContext.Current.Session["Damaged"] = dmgtypedd.SelectedValue.ToString() == "1" ? "true" : "false";
            }

            if (!string.IsNullOrEmpty(ContCode.Text))
            {
                HttpContext.Current.Session["ContainerCode"] = ContCode.Text;
            }

            if (!string.IsNullOrEmpty(FromDate.Text))
            {
                HttpContext.Current.Session["FromDate"] = FromDate.Text;
            }

            if (!string.IsNullOrEmpty(ToDate.Text))
            {
                HttpContext.Current.Session["ToDate"] = ToDate.Text;
            }

            if (!string.IsNullOrEmpty(ContainerType.Text))
            {
                HttpContext.Current.Session["ContainerType"] = ContainerType.Text;
            }

            if (!string.IsNullOrEmpty(LaneDD.SelectedValue))
            {
                HttpContext.Current.Session["LaneID"] = LaneDD.SelectedValue;
            }

            if (HttpContext.Current.Session["ContainerTrans"] != null)
            {
                List<ContainerTransactionProxy> ContainerTrans = new List<ContainerTransactionProxy>();
                ContainerTrans = HttpContext.Current.Session["ContainerTrans"] as List<ContainerTransactionProxy>;

                if (ContainerTrans.Count > 0)
                    return (List<ContainerTransactionProxy>)HttpContext.Current.Session["ContainerTrans"];
            }

            var FilterQuery = from ct in dcde.ContainerTransactions select ct;

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Damaged"] as string))
            {
                Damaged = Boolean.Parse(HttpContext.Current.Session["Damaged"].ToString());
                FilterQuery = FilterQuery.Where(a => a.ContainerDmgd == Damaged);
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["ContainerCode"] as string))
            {
                string ContainerCode = HttpContext.Current.Session["ContainerCode"] as string;
                FilterQuery = FilterQuery.Where(a => a.ContainerCode.Contains(ContainerCode));
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["ContainerType"] as string))
            {
                string ContainerType = HttpContext.Current.Session["ContainerType"] as string;
                FilterQuery = FilterQuery.Where(a => a.ContainerType.Contains(ContainerType));
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["FromDate"] as string))
            {
                DateTime FromDateTemp = DateTime.UtcNow;

                if (DateTime.TryParse(HttpContext.Current.Session["FromDate"] as string, out FromDateTemp))
                {
                    FilterQuery = FilterQuery.Where(a => a.TransactionTime.Value.CompareTo(FromDateTemp) >= 0);
                }
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["ToDate"] as string))
            {
                DateTime ToDateTemp = DateTime.UtcNow;

                if (DateTime.TryParse(HttpContext.Current.Session["ToDate"] as string, out ToDateTemp))
                {
                    FilterQuery = FilterQuery.Where(a => a.TransactionTime.Value.CompareTo(ToDateTemp) <= 0);
                }
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["LaneID"] as string))
            {
                int LaneID = Int32.Parse(HttpContext.Current.Session["LaneID"] as string);
                FilterQuery = FilterQuery.Where(a => a.LaneID == LaneID);
            }

            if (IsOnlyPrevDayDataRequired)
            {
                DateTime DayEarlier = DateTime.UtcNow.AddHours(-24);
                FilterQuery = FilterQuery.Where(a => a.TransactionTime > DayEarlier);
            }

            FilterQuery = FilterQuery.OrderByDescending(a => a.TransID);

            List<ContainerTransactionProxy> FilteredTrans = (List<ContainerTransactionProxy>)FilterQuery;
            HttpContext.Current.Session["ContainerTrans"] = FilteredTrans;

            return FilteredTrans;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            FillContainerTransactionProxyGV(FilterContainerTransDGVData(false));

            HttpContext.Current.Session["IsOnlyPreviousDayDataRequired"] = false;

            HttpContext.Current.Session.Remove("Damaged");
            HttpContext.Current.Session.Remove("ContainerCode");
            HttpContext.Current.Session.Remove("ContainerType");
            HttpContext.Current.Session.Remove("FromDate");
            HttpContext.Current.Session.Remove("ToDate");
            HttpContext.Current.Session.Remove("LaneID");

            HttpContext.Current.Session.Remove("ContainerTrans");
        }

        protected void FillContainerTransactionProxyGV(List<ContainerTransactionProxy> ContTrans)
        {
            string damaged = HttpContext.Current.Session["Damaged"] as string;
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Damaged"] as string))
            {
                dmgtypedd.SelectedValue = Boolean.Parse(HttpContext.Current.Session["Damaged"].ToString()) ? "1" : "2";
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["ContainerCode"] as string))
            {
                ContCode.Text = HttpContext.Current.Session["ContainerCode"].ToString();
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["FromDate"] as string))
            {
                FromDate.Text = Convert.ToDateTime(HttpContext.Current.Session["FromDate"].ToString()).ToString("dd-MM-yyyy");
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["ToDate"] as string))
            {
                ToDate.Text = Convert.ToDateTime(HttpContext.Current.Session["ToDate"].ToString()).ToString("dd-MM-yyyy");
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["ContainerType"] as string))
            {
                ContainerType.Text = HttpContext.Current.Session["ContainerType"].ToString();
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["LaneID"] as string))
            {
                LaneDD.SelectedValue = HttpContext.Current.Session["LaneID"].ToString();
            }

            ContainerTransDGVDT = new DataTable();
            ContainerTransDGVDT.Columns.Add("T. ID", typeof(string));
            ContainerTransDGVDT.Columns.Add("T. Time", typeof(DateTime));
            ContainerTransDGVDT.Columns.Add("Lane ID", typeof(string));
            ContainerTransDGVDT.Columns.Add("Container Damaged?", typeof(bool));
            ContainerTransDGVDT.Columns.Add("Container Size", typeof(string));
            ContainerTransDGVDT.Columns.Add("Container Code", typeof(string));

            if (ContTrans.Count > 0)
            {
                foreach (ContainerTransactionProxy ct in ContTrans)
                {
                    DataRow dr = ContainerTransDGVDT.NewRow();
                    dr["T. ID"] = ct.TransID.ToString();
                    dr["Container Code"] = !string.IsNullOrEmpty(ct.ContainerCode) ? ct.ContainerCode.ToString() : "EMPTY";
                    dr["Container Size"] = (ct.ContainerTypeID == 1 && !string.IsNullOrEmpty(ct.ContainerCode)) ? "20FT" : ct.ContainerTypeID == 2 ? "40FT" : "N/A";
                    dr["T. Time"] = ct.TransactionTime != null ? (object)DateTime.Parse(ct.TransactionTime.ToString()) : DBNull.Value;
                    dr["Lane ID"] = ct.LaneID;
                    dr["Container Damaged?"] = ct.ContainerDmgd != null ? (object)Boolean.Parse(ct.ContainerDmgd.ToString()) : DBNull.Value;

                    ContainerTransDGVDT.Rows.Add(dr);

                }

                ContainerTransDGV.AutoGenerateColumns = true;
                ContainerTransDGV.DataSource = ContainerTransDGVDT;
                ContainerTransDGV.DataBind();
                ContainerTransDGV.Visible = true;
                lbl_Error.Visible = false;
            }
            else
            {
                ContainerTransDGV.Visible = false;
                lbl_Error.Text = "No transaction records found.";
                lbl_Error.Visible = true;
            }
        }

        protected void ContainerTransDGV_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ContainerTransDGV.PageIndex = e.NewPageIndex;
            FillContainerTransactionProxyGV(FilterContainerTransDGVData((!string.IsNullOrEmpty(HttpContext.Current.Session["IsOnlyPreviousDayDataRequired"] as string) && Boolean.Parse(HttpContext.Current.Session["IsOnlyPreviousDayDataRequired"] as string)) ? true : false));
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
}