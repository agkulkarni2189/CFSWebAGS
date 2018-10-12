using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using DIMSContainerDBEFDLL;

namespace UlaWebAgsWF
{
    public class UserAuthorization:IDisposable
    { 
        private DIMContainerDB_RevisedEntities dcre = null;
        private DIMSContainerDBEFDLL.UserMaster UserToBeAuthorized = null;
        private List<sp_GetScreensFromRoleID_Result> UserAuthorizedScreenList = null;

        public UserAuthorization(ref DIMSContainerDBEFDLL.UserMaster user, ref List<sp_GetScreensFromRoleID_Result> ScreensList)
        {
            dcre = new DIMContainerDB_RevisedEntities();
            this.UserToBeAuthorized = user;
            UserAuthorizedScreenList = ScreensList;
        }

        public bool canUserAccessPage(string PageToBeChecked, ref UserMaster user)
        {
            PageToBeChecked = PageToBeChecked.TrimStart(new char[] { '/' });

            if(UserAuthorizedScreenList != null && UserToBeAuthorized != null && user != null && UserToBeAuthorized.UserId.Equals(user.UserId) && UserAuthorizedScreenList.Where(a => a.ScreenUrl.Equals(PageToBeChecked)).Any())
                return true;
            else
                return false;
        }

        public bool canUserAccessHomePage(ref UserMaster user)
        {
            if (UserAuthorizedScreenList != null && UserToBeAuthorized != null && user != null && UserToBeAuthorized.UserId.Equals(user.UserId) && UserAuthorizedScreenList.Where(a => a.ScreenUrl.Equals("Default.aspx")).Any())
                return true;
            return
                false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                dcre.Dispose();
            }
        }
    }
}