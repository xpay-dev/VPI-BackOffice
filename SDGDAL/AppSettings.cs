using System;
using System.Configuration;

namespace SDGDAL
{
    public class AppSettings
    {
        public static string SDGroupConnString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["SDGroupConnString"].ToString();
            }
        }

        public static int PasswordExpirationbyDays
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["PasswordExpirationbyDays"]);
            }
        }

        public static int MaxLoginTries
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["MaxLoginTries"]);
            }
        }

        public static int AccountLockedByMinutes
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["AccountLockedByMinutes"]);
            }
        }
    }
}