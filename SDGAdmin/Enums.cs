using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGAdmin
{
    public class Enums
    {
        public enum PermissionType
        {
            Admin = 1,
            Manager = 2,
            Employee = 3,
            Developer = 4,
            SuperAdmin = 5
        }

        public enum ParentType
        {
            Partner = 1,
            Reseller = 2,
            Merchant = 3,
            MerchantLocation = 4,
            Admin = 5,
            Marketing = 6
        }
    }
}