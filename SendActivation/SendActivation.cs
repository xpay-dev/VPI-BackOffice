using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace SendActivation
{
    public class SendActivation
    {

        public void SendActivationCode(string mailFrom, int port, string host, string user, string pass, string userName, string password, string merchantName, string msg, string emailAddress, string path, string path1)
        {
            string htmlBody = "<html><body><h1></h1><br><img style=\"float:right; margin-right:20px;\" src=\"cid:Veritas\"></body></html>"
                + "<br /><br /><br /><br /><br /><br /><br />"
                + msg + "<br /><br /><br /><br />"
                + "<b>Real-time Access to Sales Reports</b><br /><br />"
                + "Just login to http://veritaspay.com/Backoffice/ and key in your username and password<br /><br />"
                + "Username:" + "<b>" + userName + "</b>" + "<br />"
                + "Password:" + "<b>" + password + "</b>" + "<br /><br /><br />"
                + "If you need any assistance, please feel free to contact us at swipesupport@paymaya.com<br /><br /><br />"
                + "*This email contains important and confidential information about your Veritas Swipe<br />"
                + "device, please save this message for future reference.<br /><br /><br /><br /><br />"
                + "Thank You,<br />Veritas Team<br />"
                + "<img src=\"cid:VeritasFooter\">";

            AlternateView avHtml = AlternateView.CreateAlternateViewFromString
                (htmlBody, null, MediaTypeNames.Text.Html);

            // Create a LinkedResource object for each embedded image
            LinkedResource pic1 = new LinkedResource(path, MediaTypeNames.Image.Jpeg);
            pic1.ContentId = "Veritas";
            LinkedResource pic2 = new LinkedResource(path1, MediaTypeNames.Image.Jpeg);
            pic2.ContentId = "VeritasFooter";
            avHtml.LinkedResources.Add(pic1);
            avHtml.LinkedResources.Add(pic2);

            // Add the alternate views instead of using MailMessage.Body
            MailMessage m = new MailMessage();
            m.AlternateViews.Add(avHtml);

            // Address and send the message
            m.From = new MailAddress(mailFrom, "VeritasPay");
            m.To.Add(new MailAddress(emailAddress, ""));
            m.Subject = "Welcome to VeritasPay";
            SmtpClient client = new SmtpClient();
            client.EnableSsl = false;
            client.Port = port;
            client.Host = host;
            client.Timeout = 90000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(user, pass);

            try
            {
                client.Send(m);
            }
            catch (Exception ex)
            {
                Exception e = new Exception("Timeout");
                e.Data["TimeOut"] = "Timeout";
                throw e;
            }
        }

        public void SendActivationCodeBcc(string mailFrom, int port, string host, string user, string pass, string userName, string password, string merchantName, string msg, string emailAddress, string path, string path1)
        {
            int pCount = password.Count();
            string hashPassword = "";

            for (int i = 0; i < pCount; i++)
            {
                hashPassword += "*";
            }

            //string emailBcc = "mar.lazaro@paymaya.com";
            //string emailBcc1 = "aribarrola@smartemoneyinc.com";
            //string emailBcc2 = "aileen.ibarrola@paymaya.com";
            //string emailBcc3 = "MBMallare@smartemoneyinc.com";
            //string emailBcc4 = "criselle.mallare@paymaya.com";
            //string emailBcc5 = "janna.gasilao@hybridpayasia.com";

            string htmlBody = "<html><body><h1></h1><br><img style=\"float:right; margin-right:20px;\" src=\"cid:Veritas\"></body></html>"
                + "<br /><br /><br /><br /><br /><br /><br />"
                + msg + "<br /><br /><br /><br />"
                + "<b>Real-time Access to Sales Reports</b><br /><br />"
                + "Just login to http://veritaspay.com/Backoffice/ and key in your username and password<br /><br />"
                + "Username:" + "<b>" + userName + "</b>" + "<br />"
                + "Password:" + "<b>" + hashPassword + "</b>" + "<br /><br /><br />"
                + "If you need any assistance, please feel free to contact us at swipesupport@paymaya.com<br /><br /><br />"
                + "*This email contains important and confidential information about your Veritas Swipe<br />"
                + "device, please save this message for future reference.<br /><br /><br /><br /><br />"
                + "Thank You,<br />Veritas Team<br />"
                + "<img src=\"cid:VeritasFooter\">";

            AlternateView avHtml = AlternateView.CreateAlternateViewFromString
                (htmlBody, null, MediaTypeNames.Text.Html);

            // Create a LinkedResource object for each embedded image
            LinkedResource pic1 = new LinkedResource(path, MediaTypeNames.Image.Jpeg);
            pic1.ContentId = "Veritas";
            LinkedResource pic2 = new LinkedResource(path1, MediaTypeNames.Image.Jpeg);
            pic2.ContentId = "VeritasFooter";
            avHtml.LinkedResources.Add(pic1);
            avHtml.LinkedResources.Add(pic2);

            // Add the alternate views instead of using MailMessage.Body
            MailMessage m = new MailMessage();
            m.AlternateViews.Add(avHtml);

            // Address and send the message
            m.From = new MailAddress(mailFrom, "VeritasPay");
            m.To.Add(new MailAddress(emailAddress, ""));
            //m.Bcc.Add(emailBcc);
            //m.Bcc.Add(emailBcc1);
            //m.Bcc.Add(emailBcc2);
            //m.Bcc.Add(emailBcc3);
            //m.Bcc.Add(emailBcc4);
            //m.Bcc.Add(emailBcc5);
            m.Subject = "Welcome to VeritasPay";
            SmtpClient client = new SmtpClient();
            client.EnableSsl = false;
            client.Port = port;
            client.Host = host;
            client.Timeout = 90000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(user, pass);

            try
            {
                client.Send(m);
            }
            catch (Exception ex)
            {
                Exception e = new Exception("Timeout");
                e.Data["TimeOut"] = "Timeout";
                throw e;
            }
        }

        public void SendActivationCode(string mailFrom, int port, string host, string user, string pass, string msg, string emailAddress)
        {
            SmtpClient client = new SmtpClient();
            client.EnableSsl = false;
            client.Port = port;
            client.Host = host;
            client.Timeout = 90000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(user, pass);

            MailMessage mm = new MailMessage(mailFrom, emailAddress, "Welcome Merchant", msg);
            mm.IsBodyHtml = true;
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            try
            {
                client.Send(mm);
            }
            catch (Exception ex)
            {
                Exception e = new Exception("Timeout");
                e.Data["TimeOut"] = "Timeout";
                throw e;
            }
        }

        public bool SendActivationCodeRequest(string mailFrom, int port, string host, string user, string pass, string msg)
        {

            SmtpClient client = new SmtpClient();
            client.EnableSsl = false;
            client.Port = port;
            client.Host = host;
            client.Timeout = 90000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(user, pass);

            MailMessage mm = new MailMessage(mailFrom, "admin@veritaspay.com", "Request for Activation Code", msg);
            mm.IsBodyHtml = true;
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            try
            {
                client.Send(mm);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}
