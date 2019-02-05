using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using DIMSContainerDBEFDLL;
using AutoMapper;
using DIMSContainerDBEFDLL.EntityProxies;

namespace UlaWebAgsWF
{
    public class UserAuthentication : IDisposable
    {
        DIMSContainerDBEFDLL.DIMContainerDB_Revised_DevEntities dcre = null;
        DIMSContainerDBEFDLL.EntityProxies.UserMasterProxy UserToAuthenticate = null;
        private ServerUtilities utilities = null;

        public UserAuthentication(ref DIMSContainerDBEFDLL.EntityProxies.UserMasterProxy user)
        {
            dcre = new DIMContainerDB_Revised_DevEntities();
            UserToAuthenticate = user;
            utilities = new ServerUtilities();
        }

        public DIMSContainerDBEFDLL.EntityProxies.UserMasterProxy GetAuthenticatedUser()
        {
            DIMSContainerDBEFDLL.EntityProxies.UserMasterProxy AuthenticatedUser = null;
            UserMasterProxy MatchedUserFromDB = null;

            if (dcre.UserMasters.Where(a => a.UserName.Equals(UserToAuthenticate.UserName)).Count() > 0)
                MatchedUserFromDB = Mapper.Map<UserMasterProxy>(dcre.UserMasters.Where(a => a.UserName.Equals(UserToAuthenticate.UserName)).First());

            if (UserToAuthenticate != null && MatchedUserFromDB != null)
            {
                MatchedUserFromDB.Password = utilities.GetDecryptedMessage(MatchedUserFromDB.Password);

                if (UserToAuthenticate.Equals(MatchedUserFromDB))
                {
                    AuthenticatedUser = MatchedUserFromDB;
                }
            }

            return AuthenticatedUser;
        }

        public bool isUserLoggedIn()
        {
            return UserToAuthenticate.IsLoggedin;
        }

        public bool isUserActive()
        {
            return UserToAuthenticate.IsActive;
        }

        public void Dispose()
        {
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