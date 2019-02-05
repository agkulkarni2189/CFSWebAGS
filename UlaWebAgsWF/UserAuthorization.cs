using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using AutoMapper;
using DIMSContainerDBEFDLL;
using DIMSContainerDBEFDLL.EntityProxies;

namespace UlaWebAgsWF
{
    public class UserAuthorization:IDisposable
    { 
        private DIMContainerDB_Revised_DevEntities dcre = null;
        private DIMSContainerDBEFDLL.EntityProxies.UserMasterProxy UserToBeAuthorized = null;
        private List<sp_GetScreensFromRoleID_Result> UserAuthorizedScreenList = null;
        private DeviceMasterProxy DeviceToVerify = null;

        public UserAuthorization(ref DIMSContainerDBEFDLL.EntityProxies.UserMasterProxy user, ref List<sp_GetScreensFromRoleID_Result> ScreensList)
        {
            dcre = new DIMContainerDB_Revised_DevEntities();
            this.UserToBeAuthorized = user;
            UserAuthorizedScreenList = ScreensList;
        }

        public UserAuthorization(ref UserMasterProxy UserToAuthorize, ref DeviceMasterProxy DeviceToVerify)
        {
            this.UserToBeAuthorized = UserToAuthorize;
            this.DeviceToVerify = DeviceToVerify;
        }

        public bool canUserAccessPage(string PageToBeChecked)
        {
            PageToBeChecked = PageToBeChecked.TrimStart(new char[] { '/' });

            if(UserAuthorizedScreenList != null && UserToBeAuthorized != null && UserAuthorizedScreenList.Where(a => a.ScreenUrl.Equals(PageToBeChecked)).Any())
                return true;
            else
                return false;
        }

        public bool canUserAccessHomePage(ref DIMSContainerDBEFDLL.EntityProxies.UserMasterProxy user)
        {
            if (UserAuthorizedScreenList != null && UserToBeAuthorized != null && user != null && UserToBeAuthorized.UserId.Equals(user.UserId) && UserAuthorizedScreenList.Where(a => a.ScreenUrl.Equals("Default.aspx")).Any())
                return true;
            return
                false;
        }

        public bool canUserAccessDevice()
        {
            bool UserCanAccessDevice = false;

            if (UserToBeAuthorized != null && DeviceToVerify != null)
            {
                if (UserToBeAuthorized.DesignationID > 0 && DeviceToVerify.DeviceTypeID > 0)
                {
                    int LocationsCount = UserToBeAuthorized
                                        .DesignationMaster
                                        .RoleDesignationMappingMasters
                                        .Where(a => a.DesignationID
                                        .Equals(UserToBeAuthorized.DesignationID))
                                        .FirstOrDefault()
                                        .RoleMaster
                                        .LocationTypeRoleMappingMasters
                                        .Select(b => b.LocationTypeMaster)
                                        .Select(c => c.LocationMasters)
                                        .Intersect(DeviceToVerify
                                        .DeviceTypeMaster
                                        .LocationTypeDeviceTypeMappingMasters
                                        .Select(a => a.LocationTypeMaster.LocationMasters))
                                        .Count();

                    if (LocationsCount > 0)
                    {
                        UserCanAccessDevice = true;
                    }
                }
            }

            return UserCanAccessDevice;
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