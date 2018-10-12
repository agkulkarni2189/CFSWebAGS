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

namespace UlaWebAgsWF
{
    public partial class ScreensMaster : System.Web.UI.Page
    {
        private DIMContainerDB_RevisedEntities dcre = null;
        private static ConcurrentDictionary<int, ScreenMaster> ScreensDictionary = null;
        private CancellationToken ScreensTasksCT = new CancellationToken(false);
        TaskFactory<Task> ScreensTF = null;
        private List<sp_GetScreensFromRoleID_Result> UserAccessibleScreens = null;
        private DIMSContainerDBEFDLL.UserMaster LoggedInUser = null;

        protected async void Page_Load(object sender, EventArgs e)
        {
            await ClearMessages();
            dcre = new DIMContainerDB_RevisedEntities();
            UserAccessibleScreens = (List<sp_GetScreensFromRoleID_Result>)HttpContext.Current.Session["UserAccessibleScreens"];

            if (!IsPostBack)
            {
                ScreensTF = new TaskFactory<Task>(ScreensTasksCT, TaskCreationOptions.AttachedToParent, TaskContinuationOptions.AttachedToParent, null);
                List<ScreenMaster> Screens = dcre.ScreenMasters.ToList<ScreenMaster>();
                ScreensDictionary = new ConcurrentDictionary<int, ScreenMaster>();

                Screens = dcre.ScreenMasters.ToList<ScreenMaster>();

                foreach (ScreenMaster Screen in Screens)
                    ScreensDictionary.AddOrUpdate(Screen.ID, Screen, (a, b) => Screen);

                await Fill_ScreensDGV();
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
                                SuccessMsgText.Text = "New Screen Added Successfully";
                                SuccessMsgText.Visible = true;
                                SuccessMsg.Visible = true;
                            }
                            else
                            {
                                FailureMsgText.Text = "Can not add screen, contact system admin";
                                FailureMsgText.Visible = true;
                                FailureMsg.Visible = true;
                            }
                        }
                    }
                }), ScreensTasksCT);

                await Fill_ScreensDGV();
            }
            catch (Exception ex)
            {
                FailureMsgText.Text = "Can not add screen, contact system admin";
                FailureMsgText.Visible = true;
                FailureMsg.Visible = true;
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
                                ScreensDictionary.AddOrUpdate(ScreenID, OldScreen, (a, b) => NewScreen);
                            }
                        }
                        else
                        {
                            FailureMsgText.Text = "Selected screen is either deleted or not accessible, please try after some time.";
                            FailureMsgText.Visible = true;
                            FailureMsg.Visible = true;
                        }

                        ScreensDGV.EditIndex = -1;
                    }
                }), ScreensTasksCT);

                await Fill_ScreensDGV();
            }
            catch (Exception ex)
            {
                FailureMsgText.Text = "Can not update selected screen";
                FailureMsgText.Visible = true;
                FailureMsg.Visible = true;
            }
        }

        protected async void ScreensDGV_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            await Task.Run(new Action(() => {
                SuccessMsgText.Text = "Selected screen updated successfully";
                SuccessMsgText.Visible = true;
                SuccessMsg.Visible = true;
            }), ScreensTasksCT);
        }

        protected async void ScreensDGV_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                await Task.Run(new Action(() => {
                    int ScreenID = Int32.Parse(ScreensDGV.DataKeys[e.RowIndex].Value.ToString());
                    ScreenMaster ScreenToDelete = null;

                    if (ScreensDictionary.TryGetValue(ScreenID, out ScreenToDelete) && dcre != null)
                    {
                        dcre.ScreenMasters.Remove(dcre.ScreenMasters.Where(a => a.ID.Equals(ScreenToDelete.ID)).FirstOrDefault());

                        if (dcre.SaveChanges() > 0)
                        {
                            ScreenMaster DeletedScreen = null;

                            while (!ScreensDictionary.TryRemove(ScreenID, out DeletedScreen))
                                ScreensDictionary.TryRemove(ScreenID, out DeletedScreen);
                        }
                    }
                    else
                    {
                        FailureMsgText.Text = "Selected screen is either deleted or not accessible";
                        FailureMsgText.Visible = true;
                        FailureMsg.Visible = true;
                    }
                }), ScreensTasksCT);

                await Fill_ScreensDGV();
            }
            catch (Exception ex)
            {
                FailureMsgText.Text = "Can not delete selected screen";
                FailureMsgText.Visible = true;
                FailureMsg.Visible = true;
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

        protected async void ScreensDGV_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            await Task.Run(new Action(() => {
                SuccessMsgText.Text = "Selected screen deleted successfully";
                SuccessMsgText.Visible = true;
                SuccessMsg.Visible = true;
            }), ScreensTasksCT);
        }
    }
}