using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using DIMSContainerDBEFDLL;
using NLog;

namespace UlaWebAgsWF
{
    public partial class _Default : Page, IWebAGSClass
    {
        private static DIMContainerDB_Revised_DevEntities dcde = null;
        private string ErrorMsg = string.Empty;
        private static Logger logger = LogManager.GetLogger("DashboardLoggerEvents", typeof(_Default));

        protected void Page_Load(object sender, EventArgs e)
        {
            ClearMessages();

            if (HttpContext.Current.Session["ErrorMsg"] != null)
            {
                ErrorMsgLiteral.Text = HttpContext.Current.Session["ErrorMsg"].ToString();
                ErrorMsgLiteral.Visible = true;
                ErrorMsgPanel.Visible = true;
                HttpContext.Current.Session.Remove("ErrorMsg");
            }

            if (HttpContext.Current.Session["SuccessMsg"] != null)
            {
                SuccessMsgLiteral.Text = HttpContext.Current.Session["SuccessMsg"].ToString();
                SuccessMsgLiteral.Visible = true;
                SuccessMsgPanel.Visible = true;
                HttpContext.Current.Session.Remove("SuccessMsg");
            }

            dcde = new DIMContainerDB_Revised_DevEntities();

            if (!IsPostBack)
            {
                Response.Cache.SetExpires(System.DateTime.UtcNow.AddMinutes(-1));
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();

                List<sp_GetScreensFromRoleID_Result> UserAccessibleScreens = (List<sp_GetScreensFromRoleID_Result>)HttpContext.Current.Session["UserAccessibleScreens"];
                DIMSContainerDBEFDLL.EntityProxies.UserMasterProxy LoggedInUser = (DIMSContainerDBEFDLL.EntityProxies.UserMasterProxy)HttpContext.Current.Session["LoggedInUser"];

                bool IsUserAuthorizedToAccessTRansaction = false;
                bool IsTransactionPreviewEnabled = false;
                string CurrentSystemIP = HttpContext.Current.Session["SysIP"].ToString();

                if (UserAccessibleScreens != null)
                {
                    using (UserAuthorization UAuth = new UserAuthorization(ref LoggedInUser, ref UserAccessibleScreens))
                    {
                       IsUserAuthorizedToAccessTRansaction = UAuth.canUserAccessPage("DamageImages.aspx");
                    }
                }

                if (ConfigurationManager.AppSettings.Get("EnableContainerTransactionPreview") != null)
                {
                    IsTransactionPreviewEnabled = Boolean.Parse(ConfigurationManager.AppSettings.Get("EnableContainerTransactionPreview") as string);
                }

                //if (IsUserAuthorizedToAccessTRansaction && IsTransactionPreviewEnabled)
                //{
                //    TransCheckTimer.Enabled = true;
                //}
                //else
                //{
                //    TransCheckTimer.Enabled = false;

                //    if (!IsUserAuthorizedToAccessTRansaction)
                //    {
                //        logger.Info(new LogMessageGenerator(() =>
                //        {
                //            return "User not authorized to verify live transactions";
                //        }));
                //    }

                //    if(!IsTransactionPreviewEnabled)
                //    {
                //        logger.Info(new LogMessageGenerator(() =>
                //        {
                //            return "Live transaction verification is disabled for this system.";
                //        }));
                //    }
                //}

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
            logger.Info(new LogMessageGenerator(() => {
                return "Starting to build dashboard menu";
            }));

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

            logger.Info(new LogMessageGenerator(() => {
                return "Dashboard menu build successful";
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

        //protected void TransCheckTimer_Tick(object sender, EventArgs e)
        //{
        //    logger.Info(new LogMessageGenerator(() => {
        //        return "Checking for live transaciton in database";
        //    }));

        //    string TransactionID = string.Empty;

        //    string CurrentSysIP = HttpContext.Current.Session["SysIP"] as string;

        //    if (!string.IsNullOrEmpty(CurrentSysIP))
        //    {
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
        //                        lm.SystemIP.Equals(CurrentSysIP)
        //                        select
        //                        ct.TransID.ToString()).ToList<string>();

        //        if (LiveTransIDs.Count > 0)
        //        {
        //            foreach (string ct in LiveTransIDs)
        //            {
        //                TransactionID = LiveTransIDs[0];

        //                HttpContext.Current.Session["TransactionID"] = TransactionID;

        //                logger.Info(new LogMessageGenerator(() =>
        //                {
        //                    return "Live transaction found with Transaction ID: " + TransactionID + ", redirecting to transaction verification page";
        //                }));

        //                break;
        //            }

        //            HttpContext.Current.Response.Redirect("DamageImages.aspx");
        //        }
        //    }
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