using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using DIMSContainerDBEFDLL;
namespace UlaWebAgsWF
{
    public class UserAuthentication : IDisposable
    {
        DIMSContainerDBEFDLL.DIMContainerDB_RevisedEntities dcre = null;
        DIMSContainerDBEFDLL.UserMaster UserToAuthenticate = null;
        private ServerUtilities utilities = null;

        public UserAuthentication(ref DIMSContainerDBEFDLL.UserMaster user)
        {
            dcre = new DIMContainerDB_RevisedEntities();
            UserToAuthenticate = user;
            utilities = new ServerUtilities();
        }

        public DIMSContainerDBEFDLL.UserMaster GetAuthenticatedUser()
        {
            DIMSContainerDBEFDLL.UserMaster AuthenticatedUser = null;

            List<UserMaster> users = dcre.UserMasters.Where(a => a.UserName.Equals(UserToAuthenticate.UserName)).Select(a => a).ToList<UserMaster>();
            foreach (DIMSContainerDBEFDLL.UserMaster user in users)
            {
                string Password = utilities.GetDecryptedMessage(user.Password);

                if (UserToAuthenticate != null && UserToAuthenticate.Password.Equals(Password))
                {
                    AuthenticatedUser = user;
                    break;
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