using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGBackOffice
{
    public class CurrentUser
    {
        public static int AccountId
        {
            get
            {
                return Convert.ToInt32(HttpContext.Current.Session["AccountId"]);
            }
            set
            {
                HttpContext.Current.Session["AccountId"] = value;
            }
        }

        public static int UserId
        {
            get
            {
                return Convert.ToInt32(HttpContext.Current.Session["UserId"]);
            }
            set
            {
                HttpContext.Current.Session["UserId"] = value;
            }
        }

        public static string DisplayName
        {
            get
            {
                return HttpContext.Current.Session["DisplayName"].ToString();
            }
            set
            {
                HttpContext.Current.Session["DisplayName"] = value;
            }
        }

        public static SDGBackOffice.Enums.ParentType ParentType
        {
            get
            {
                return (Enums.ParentType)HttpContext.Current.Session["ParentTypeId"];
            }
            set
            {
                HttpContext.Current.Session["ParentTypeId"] = value;
            }
        }

        public static int ParentId
        {
            get
            {
                return Convert.ToInt32(HttpContext.Current.Session["ParentId"]);
            }
            set
            {
                HttpContext.Current.Session["ParentId"] = value;
            }
        }

        public static string Avatar
        {
            get
            {
                return HttpContext.Current.Session["UserAvatar"] == null ? "" : HttpContext.Current.Session["UserAvatar"].ToString();
            }
            set
            {
                HttpContext.Current.Session["UserAvatar"] = value;
            }
        }

        public static string Role
        {
            get
            {
                return HttpContext.Current.Session["UserRole"].ToString();
            }
            set
            {
                HttpContext.Current.Session["UserRole"] = value;
            }
        }

        public static string Country
        {
            get
            {
                return HttpContext.Current.Session["Country"].ToString();
            }
            set
            {
                HttpContext.Current.Session["Country"] = value;
            }
        }
    }
}