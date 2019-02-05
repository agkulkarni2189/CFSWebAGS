using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DIMSContainerDBEFDLL;
using NLog;

namespace UlaWebAgsWF
{
    public partial class ScreensMaster : System.Web.UI.Page
    {
        private DIMContainerDB_Revised_DevEntities dcre = null;
        private static ConcurrentDictionary<int, ScreenMaster> ScreensDictionary = null;
        private CancellationToken ScreensTasksCT = new CancellationToken(false);
        private List<sp_GetScreensFromRoleID_Result> UserAccessibleScreens = null;
        private static Logger logger = LogManager.GetLogger("ScreensMasterLogger", typeof(ScreensMaster));

        protected async void Page_Load(object sender, EventArgs e)
        {
            try
            {
                await ClearMessages();

                if (HttpContext.Current.Session["ScreensMasterSuccessMsg"] != null)
                {
                    SuccessMsgText.Text = HttpContext.Current.Session["ScreensMasterSuccessMsg"].ToString();
                    SuccessMsgText.Visible = true;
                    SuccessMsg.Visible = true;
                    HttpContext.Current.Session.Remove("ScreensMasterSuccessMsg");
                }

                if (HttpContext.Current.Session["ScreensMasterFailureMsg"] != null)
                {
                    FailureMsgText.Text = HttpContext.Current.Session["ScreensMasterFailureMsg"].ToString();
                    FailureMsgText.Visible = true;
                    FailureMsg.Visible = true;
                    HttpContext.Current.Session.Remove("ScreensMasterFailureMsg");
                }

                dcre = new DIMContainerDB_Revised_DevEntities();
                UserAccessibleScreens = (List<sp_GetScreensFromRoleID_Result>)HttpContext.Current.Session["UserAccessibleScreens"];

                if (!IsPostBack)
                {
                    logger.Trace(new LogMessageGenerator(() => {
                        return "Loading lanes master from system: " + HttpContext.Current.Session["SysIP"] + " accessed by " + ((DIMSContainerDBEFDLL.EntityProxies.UserMasterProxy)HttpContext.Current.Session["LoggedInUser"]).UserName;
                    }));

                    List<ScreenMaster> Screens = dcre.ScreenMasters.ToList<ScreenMaster>();
                    ScreensDictionary = new ConcurrentDictionary<int, ScreenMaster>();
                    Screens.ForEach((s) => {
                        ScreensDictionary.TryAdd(s.ID, s);
                    });

                    await Fill_ScreensDGV();
                }
            }
            catch (Exception ex)
            {
                throw new HttpException(403, ex.Message, ex);
            }
        }

        private async Task ClearMessages()
        {
            await Task.Run(new Action(() => {
                SuccessMsgText.Text = string.Empty;
                SuccessMsgText.Visible = false;
                FailureMsgText.Text = string.Empty;
                FailureMsgText.Visible = false;
                FailureMsg.Visible = false;
            }), ScreensTasksCT);
        }

        private async Task Fill_ScreensDGV()
        {
            await Task.Run(new Action(() => {
                if (ScreensDictionary != null && ScreensDictionary.Count > 0)
                {
                    ScreensDGV.AutoGenerateColumns = false;
                    ScreensDGV.DataSource = ScreensDictionary.Values.ToList<ScreenMaster>();
                    ScreensDGV.EmptyDataText = "No screens registered";
                    ScreensDGV.DataBind();
                }
            }), ScreensTasksCT);
        }

        protected async void NewScreenBtnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string SuccessMsg = null;
                string FailureMsg = null;
                await Task.Run(new Action(() =>
                {
                    if (Page.IsValid && dcre != null)
                    {
                        ScreenMaster NewScreen = new ScreenMaster();
                        NewScreen.ScreenName = NewScreenName.Text;
                        NewScreen.ScreenUrl = ScreenUrl.Text;

                        dcre.ScreenMasters.Add(NewScreen);
                        if (dcre.SaveChanges() > 0)
                        {
                            ScreenMaster RegisteredScreen = dcre.ScreenMasters.OrderByDescending(a => a.ID).Select(a => a).First();
                            if (ScreensDictionary.TryAdd(RegisteredScreen.ID, RegisteredScreen))
                            {
                                logger.Trace(new LogMessageGenerator(() => {
                                    return "New screen added successfully";
                                }));

                                SuccessMsg = "New Screen Added Successfully";
                            }
                            else
                            {
                                logger.Trace(new LogMessageGenerator(() => {
                                    return "Can not add screen this time, contact system admin";
                                }));

                                FailureMsg = "Can not add screen this time, contact system admin";
                            }
                        }
                    }
                }), ScreensTasksCT);

                HttpContext.Current.Session["ScreensMasterSuccessMsg"] = SuccessMsg;
                HttpContext.Current.Session["ScreensMasterFailureMsg"] = FailureMsg;
                //HttpContext.Current.Session["UserAccessibleScreens"] = dcre.sp_GetScreensFromRoleID(((DIMSContainerDBEFDLL.EntityProxies.UserMasterProxy)HttpContext.Current.Session["LoggedInUser"]).RoleID).ToList<sp_GetScreensFromRoleID_Result>();
                Response.Redirect(Request.RawUrl, false);
            }
            catch (Exception ex)
            {
                throw new HttpException(403, ex.Message, ex);
            }
        }

        protected async void ScreensDGV_RowEditing(object sender, GridViewEditEventArgs e)
        {
            await Task.Run(new Action(() => {
                ScreensDGV.EditIndex = e.NewEditIndex;
            }), ScreensTasksCT);

            await Fill_ScreensDGV();
        }

        protected async void ScreensDGV_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                string SuccessMsg = null, FailureMsg = null;

                await Task.Run(new Action(() =>
                {
                    if (Page.IsValid)
                    {
                        int ScreenID = Int32.Parse(ScreensDGV.DataKeys[e.RowIndex].Value.ToString());
                        ScreenMaster OldScreen = new ScreenMaster();
                        if (ScreensDictionary.TryGetValue(ScreenID, out OldScreen))
                        {
                            ScreenMaster NewScreen = dcre.ScreenMasters.Where(a => a.ID.Equals(ScreenID)).First();
                            NewScreen.ScreenName = ((TextBox)ScreensDGV.Rows[e.RowIndex].FindControl("ScreenNameTBX")).Text;
                            NewScreen.ScreenUrl = ((TextBox)ScreensDGV.Rows[e.RowIndex].FindControl("ScreenUrlTBX")).Text;

                            if (dcre.SaveChanges() > 0)
                            {
                                NewScreen.ID = ScreenID;
                                ScreensDictionary.TryUpdate(ScreenID, NewScreen, OldScreen);
                                SuccessMsg = "Screen " + NewScreen.ScreenName + " updated successfully";
                            }
                        }
                        else
                        {
                            logger.Trace(new LogMessageGenerator(() => {
                                return "Selected screen is either deleted already or not accessible, please try after some time.";
                            }));

                            FailureMsg = "Selected screen is either deleted already or not accessible, please try after some time.";
                        }

                        ScreensDGV.EditIndex = -1;
                    }
                }), ScreensTasksCT);


                HttpContext.Current.Session["ScreensMasterSuccessMsg"] = SuccessMsg;
                HttpContext.Current.Session["ScreensMasterFailureMsg"] = FailureMsg;
                //HttpContext.Current.Session["UserAccessibleScreens"] = dcre.sp_GetScreensFromRoleID(((DIMSContainerDBEFDLL.EntityProxies.UserMasterProxy)HttpContext.Current.Session["LoggedInUser"]).RoleID).ToList<sp_GetScreensFromRoleID_Result>();
                Response.Redirect(Request.RawUrl, false);
            }
            catch (Exception ex)
            {
                throw new HttpException(403, ex.Message, ex);
            }
        }

        protected async void ScreensDGV_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                string SuccessMsg = null, FailureMsg = null;
                await Task.Run(new Action(() => {
                    int ScreenID = Int32.Parse(ScreensDGV.DataKeys[e.RowIndex].Value.ToString());
                    ScreenMaster ScreenToDelete = null;

                    if (ScreensDictionary.TryGetValue(ScreenID, out ScreenToDelete) && dcre != null)
                    {
                        if (!ScreenToDelete.ScreenName.Equals("Home") && ScreensDictionary.Count > 1)
                        {
                            dcre.ScreenMasters.Remove(dcre.ScreenMasters.Where(a => a.ID.Equals(ScreenToDelete.ID)).FirstOrDefault());

                            if (dcre.SaveChanges() > 0)
                            {
                                ScreenMaster DeletedScreen = null;

                                while (!ScreensDictionary.TryRemove(ScreenID, out DeletedScreen))
                                    ScreensDictionary.TryRemove(ScreenID, out DeletedScreen);

                                SuccessMsg = "Screen " + DeletedScreen.ScreenName + " removed successfully";
                            }
                        }
                        else
                        {
                            FailureMsg = "Can not delete this screen, it is a home screen or last one in the system";
                        }
                    }
                    else
                    {
                        FailureMsg = "Selected screen is either deleted or not accessible";
                    }
                }), ScreensTasksCT);

                HttpContext.Current.Session["ScreensMasterSuccessMsg"] = SuccessMsg;
                HttpContext.Current.Session["ScreensMasterFailureMsg"] = FailureMsg;
                //HttpContext.Current.Session["UserAccessibleScreens"] = dcre.sp_GetScreensFromRoleID(((DIMSContainerDBEFDLL.EntityProxies.UserMasterProxy)HttpContext.Current.Session["LoggedInUser"]).RoleID).ToList<sp_GetScreensFromRoleID_Result>();
                Response.Redirect(Request.RawUrl, true);
            }
            catch (Exception ex)
            {
                throw new HttpException(403, ex.Message, ex);
            }
        }

        protected async void ScreensDGV_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            await Task.Run(new Action(() => {
                ScreensDGV.EditIndex = -1;

                SuccessMsgText.Text = "Screen edit cancelled successfully";
                SuccessMsgText.Visible = true;
                SuccessMsg.Visible = true;
            }), ScreensTasksCT);

            await Fill_ScreensDGV();
        }
    }
}