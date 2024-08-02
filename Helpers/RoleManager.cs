using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarnwareGUI.Helpers
{
    public static class RoleManager
    {
        private static readonly object lockObj = new object();
        private static string _role;
        private static string _user;

        public static string Role
        {
            get
            {
                lock (lockObj)
                {
                    return _role;
                }
            }
            set
            {
                lock (lockObj)
                {
                    _role = value;
                }
            }
        }

        public static string User
        {
            get
            {
                lock (lockObj)
                {
                    return _user;
                }
            }
            set
            {
                lock (lockObj)
                {
                    _user = value;
                }
            }
        }

        public static string GetUserRole()
        {
            return Role ?? "user"; // default to User role if login fails
        }

        public static string GetUser()
        {
            return User ?? ""; // default to empty if login fails
        }
    }
}
