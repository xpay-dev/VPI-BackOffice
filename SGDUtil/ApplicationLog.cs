using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDGUtil
{
    public static class ApplicationLog
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="application">SDGAdmin, SDGBackOffice, SDGWebService</param>
        /// <param name="error">Action + Exception.Message</param>
        /// <param name="source">MethodName or ControllerName</param>
        /// <param name="stackTrace">Exception.StackTrace</param>
        /// <returns>ReferenceNumber</returns>
        public static string LogError(string application, string error, string source, string stackTrace, string notes = "")
        {
            string referenceNumber = "RN" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "EL";

            string dateToday = DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Year;
            string time = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;

            string subdir = "c:\\ApplicationLogs\\" + application + "\\" + dateToday;
            string filedir = subdir + "\\errorLog.txt";

            if (!Directory.Exists(subdir))
            {
                Directory.CreateDirectory(subdir);
            }

            if (!File.Exists(filedir))
            {
                File.Create(filedir).Dispose();
            }
            

            using (StreamWriter file = new StreamWriter(filedir, true))
            {
                StringBuilder errorLog = new StringBuilder();

                errorLog.AppendLine("Reference Number: " + referenceNumber);
                errorLog.AppendLine("Time: " + time);
                errorLog.AppendLine("Source: " + source);
                errorLog.AppendLine("Error: " + error);
                errorLog.AppendLine("Stack Trace: " + stackTrace);

                if (string.IsNullOrEmpty(notes))
                {
                    errorLog.AppendLine("Notes: " + notes);
                }

                errorLog.AppendLine("----------------------------------------------------------------");

                file.WriteLine(errorLog.ToString());
            }

            return referenceNumber;
        }

        public static string UserActivityLog(string application, string userActivity, string source, string notes = "")
        {
            string referenceNumber = "RN" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "EL";

            string dateToday = DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Year;
            string time = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;

            string subdir = "c:\\ApplicationLogs\\" + application + "\\" + dateToday;
            string filedir = subdir + "\\userActivityLog.txt";

            if (!Directory.Exists(subdir))
            {
                Directory.CreateDirectory(subdir);
            }

            if (!File.Exists(filedir))
            {
                File.Create(filedir).Dispose();
            }

            using (StreamWriter file = new StreamWriter(filedir, true))
            {
                StringBuilder activityLog = new StringBuilder();

                activityLog.AppendLine("Reference Number: " + referenceNumber);
                activityLog.AppendLine("Time: " + time);
                activityLog.AppendLine("Source: " + source);
                activityLog.AppendLine("User Activity: " + userActivity);

                if (string.IsNullOrEmpty(notes))
                {
                    activityLog.AppendLine("Notes: " + notes);
                }

                activityLog.AppendLine("----------------------------------------------------------------");

                file.WriteLine(activityLog.ToString());
            }

            return referenceNumber;
        }

        public static string UsersLoginInfo(string application, int accountId, int userId, bool accountStatus,  string IPAddress, string notes = "")
        {
            string referenceNumber = "RN" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "EL";

            string dateToday = DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Year;
            string time = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;

            string subdir = "c:\\ApplicationLogs\\" + application + "\\" + dateToday;
            string filedir = subdir + "\\UsersLoginInfo.txt";

            if (!Directory.Exists(subdir))
            {
                Directory.CreateDirectory(subdir);
            }

            if (!File.Exists(filedir))
            {
                File.Create(filedir).Dispose();
            }

            using (StreamWriter file = new StreamWriter(filedir, true))
            {
                StringBuilder activityLog = new StringBuilder();

                activityLog.AppendLine("Reference Number: " + referenceNumber);
                activityLog.AppendLine("Time: " + time);
                activityLog.AppendLine("Account ID: " + accountId);
                activityLog.AppendLine("User ID: " + userId);
                activityLog.AppendLine("IP Address: " + IPAddress);
                activityLog.AppendLine("Account Status: " + accountStatus);

                if (string.IsNullOrEmpty(notes))
                {
                    activityLog.AppendLine("Notes: " + notes);
                }

                activityLog.AppendLine("----------------------------------------------------------------");

                file.WriteLine(activityLog.ToString());
            }

            return referenceNumber;
        }

        public static string LogCardFormatError(string application, string enctracks, string ksn, string cardname, string cardExpiry, string track2, string notes, string mid, string transNumber)
        {
            string referenceNumber = "RN" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "EL";

            string dateToday = DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Year;
            string time = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;

            string subdir = "c:\\ApplicationLogs\\" + application + "\\" + dateToday;
            string filedir = subdir + "\\GatewayErrorLog.txt";

            if (!Directory.Exists(subdir))
            {
                Directory.CreateDirectory(subdir);
            }

            if (!File.Exists(filedir))
            {
                File.Create(filedir).Dispose();
            }

            using (StreamWriter file = new StreamWriter(filedir, true))
            {
                StringBuilder activityLog = new StringBuilder();

                activityLog.AppendLine("Reference Number: " + referenceNumber);
                activityLog.AppendLine("Time: " + time);
                activityLog.AppendLine("Entracks: " + enctracks);
                activityLog.AppendLine("Ksn: " + ksn);
                activityLog.AppendLine("Card Name: " + cardname);
                activityLog.AppendLine("Card Expiry: " + cardExpiry);
                activityLog.AppendLine("Track2: " + track2);
                activityLog.AppendLine("Notes: " + notes);
                activityLog.AppendLine("Mid:" + mid);
                activityLog.AppendLine("Transaction Number:" + transNumber);
                activityLog.AppendLine("----------------------------------------------------------------");

                file.WriteLine(activityLog.ToString());
            }

            return referenceNumber;
        }

        public static string LogCTPaymentError(string application, string action, string request, string response)
        {
            string referenceNumber = "RN" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "EL";

            string dateToday = DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Year;
            string time = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;

            string subdir = "c:\\ApplicationLogs\\" + application + "\\" + dateToday;
            string filedir = subdir + "\\CTPaymentErrorLog.txt";

            if (!Directory.Exists(subdir))
            {
                Directory.CreateDirectory(subdir);
            }

            if (!File.Exists(filedir))
            {
                File.Create(filedir).Dispose();
            }

            using (StreamWriter file = new StreamWriter(filedir, true))
            {
                StringBuilder activityLog = new StringBuilder();

                activityLog.AppendLine("Reference Number: " + referenceNumber);
                activityLog.AppendLine("Time: " + time);
                activityLog.AppendLine("Data Request: " + request);
                activityLog.AppendLine("Action: " + action);
                activityLog.AppendLine("Response" + response);
  
                activityLog.AppendLine("----------------------------------------------------------------");

                file.WriteLine(activityLog.ToString());
            }

            return referenceNumber;
        }

        public static string LogTransactionEMV(string application, string action, string request, string response)
        {
            string referenceNumber = "RN" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "EL";

            string dateToday = DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Year;
            string time = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;

            string subdir = "c:\\ApplicationLogs\\" + application + "\\" + dateToday;
            string filedir = subdir + "\\EMVTransactionErrorLog.txt";

            if (!Directory.Exists(subdir))
            {
                Directory.CreateDirectory(subdir);
            }

            if (!File.Exists(filedir))
            {
                File.Create(filedir).Dispose();
            }

            using (StreamWriter file = new StreamWriter(filedir, true))
            {
                StringBuilder activityLog = new StringBuilder();

                activityLog.AppendLine("Reference Number: " + referenceNumber);
                activityLog.AppendLine("Time: " + time);
                activityLog.AppendLine("Data Request: " + request);
                activityLog.AppendLine("Action: " + action);
                activityLog.AppendLine("Response" + response);

                activityLog.AppendLine("----------------------------------------------------------------");

                file.WriteLine(activityLog.ToString());
            }

            return referenceNumber;
        }
    }
}
