using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using DIMSContainerDBEFDLL;

namespace UlaWebAgsWF
{
    public partial class _Default : Page, IWebAGSClass
    {
        private static DIMContainerDB_RevisedEntities dcde = null;
        private string ErrorMsg = string.Empty;

        public static Mutex mutex;

        protected void Page_Load(object sender, EventArgs e)
        {
            ClearMessages();

            if (HttpContext.Current.Session["ErrorMsg"] != null)
            {
                ErrorMsgLiteral.Text = HttpContext.Current.Session["ErrorMsg"].ToString();
                ErrorMsgPanel.Visible = true;
                HttpContext.Current.Session.Remove("ErrorMsg");
            }

            if (HttpContext.Current.Session["SuccessMsg"] != null)
            {
                ErrorMsgLiteral.Text = HttpContext.Current.Session["SuccessMsg"].ToString();
                ErrorMsgPanel.Visible = true;
                HttpContext.Current.Session.Remove("SuccessMsg");
            }

            dcde = new DIMContainerDB_RevisedEntities();

            if (!IsPostBack)
            {
                Response.Cache.SetExpires(System.DateTime.UtcNow.AddMinutes(-1));
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();

                this.Page.Title = "Dashboard";
                Fill_DashboardMenu();
                new DBUtility();
            }
        }

        private void ClearMessages()
        {
            ErrorMsgLiteral.Text = string.Empty;
            ErrorMsgLiteral.Visible = false;
            ErrorMsgPanel.Visible = false;
        }

        public void Fill_DashboardMenu()
        {
            if (HttpContext.Current.Session["UserAccessibleScreens"] != null)
            {
                PageLinksLiteral.Text += "<ul class='list-group'>";

                foreach (sp_GetScreensFromRoleID_Result uas in (List<sp_GetScreensFromRoleID_Result>)HttpContext.Current.Session["UserAccessibleScreens"])
                {
                    if(!uas.ScreenUrl.Equals("DamageImages.aspx"))
                        PageLinksLiteral.Text += "<li class='list-group-item'><a class='btn btn-link btn-large' runat='server' href='" + uas.ScreenUrl + "'>" + uas.ScreenName + "</a></li>";
                }

                PageLinksLiteral.Text += "</ul>";
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

        protected void TransCheckTimer_Tick(object sender, EventArgs e)
        {
            string TransactionID = string.Empty;
            List<sp_GetScreensFromRoleID_Result> UserAccessibleScreens = (List<sp_GetScreensFromRoleID_Result>)HttpContext.Current.Session["UserAccessibleScreens"];
            bool IsUserAuthorizedToAccessTRansaction = false;
            bool IsTransactionPreviewEnabled = false;
            string CurrentSysIP = HttpContext.Current.Session["SysIP"] as string;

            if (UserAccessibleScreens != null)
            {
                IsUserAuthorizedToAccessTRansaction = UserAccessibleScreens.Where(a => a.ScreenUrl.Equals("DamageImages.aspx")).Any();
            }

            if (HttpContext.Current.Session["IsTransactionPreviewEnabled"] != null)
            {
                IsTransactionPreviewEnabled = Boolean.Parse(HttpContext.Current.Session["IsTransactionPreviewEnabled"] as string);
            }

            if (!string.IsNullOrEmpty(CurrentSysIP) && IsUserAuthorizedToAccessTRansaction && IsTransactionPreviewEnabled)
            {
                List<string> LiveTransIDs = new List<string>();
                LiveTransIDs = (from
                                ct
                                in
                                    dcde.ContainerTransactions
                                join
                                lm
                                in
                                dcde.LaneMasters
                                on
                                ct.LaneID
                                equals
                                lm.LaneID
                                where
                                lm.SystemIP.Equals(CurrentSysIP)
                                select
                                ct.TransID.ToString()).ToList<string>();

                if (LiveTransIDs.Count > 0)
                {
                    foreach (string ct in LiveTransIDs)
                    {
                        TransactionID = LiveTransIDs[0];

                        HttpContext.Current.Session["TransactionID"] = TransactionID;
                        break;
                    }

                    HttpContext.Current.Response.Redirect("DamageImages.aspx");
                }
            }
        }

        //[WebMethod]
        //public static string Trans_Check_Timer_Tick()
        //{
        //    mutex.WaitOne();
        //    string TransactionID = string.Empty;

        //    if (HttpContext.Current.Session["SysIP"] != null && HttpContext.Current.Session["UserAccessibleScreens"] != null && ((List<sp_GetScreensFromRoleID_Result>)HttpContext.Current.Session["UserAccessibleScreens"]).Where(a => a.ScreenUrl.Equals("DamageImages.aspx")).Any())
        //    {
        //        string SysIP = HttpContext.Current.Session["SysIP"].ToString();

        //        List<string> LiveTransIDs = new List<string>();
        //        LiveTransIDs = (from
        //                        ct
        //                        in
        //                            dcde.ContainerTransactions
        //                        join
        //                        lm
        //                        in
        //                        dcde.LaneMasters
        //                        on
        //                        ct.LaneID
        //                        equals
        //                        lm.LaneID
        //                        where
        //                        lm.SystemIP.Equals(SysIP)
        //                        select
        //                        ct.TransID.ToString()).ToList<string>();

        //        if (LiveTransIDs.Count > 0)
        //        {
        //            foreach (string ct in LiveTransIDs)
        //            {
        //                TransactionID = LiveTransIDs[0];
        //                break;
        //            }
        //        }
        //    }
        //    mutex.ReleaseMutex();
        //    return TransactionID;
        //}
    }

    public class ObjectStateInfo
    {
        private HttpContext HttpContext;
        private AutoResetEvent AutoResetEvent;

        public ObjectStateInfo()
        { }

        public ObjectStateInfo(HttpContext ctx, AutoResetEvent ResetEvent) : this()
        {
            HttpContext = ctx;
            AutoResetEvent = ResetEvent;
        }

        public HttpContext GetHttpContext()
        {
            return HttpContext;
        }

        public AutoResetEvent GetResetEvent()
        {
            return AutoResetEvent;
        }
    }
}