using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace UlaWebAgsWF
{
    public class ServerUtilities
    {
        public enum ContainerTypes
        {
            NONE = -1,
            SIZE_20_FT,
            SIZE_40_FT
        }

        public string GetEncryptedMessage(string Message)
        {
            return Convert.ToBase64String(ProtectedData.Protect(Encoding.Unicode.GetBytes(Message), null, DataProtectionScope.LocalMachine));
        }

        public string GetDecryptedMessage(string Message)
        {
            return Encoding.Unicode.GetString(ProtectedData.Unprotect(Convert.FromBase64String(Message), null, DataProtectionScope.LocalMachine));
        }
    }
}