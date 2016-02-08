using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.Hosting;

namespace Simple_Mail
{
    public enum EmailType
    {
        Default,
        Confirmation,
        Forgotten,
        Welcome,
        Error
    }
    public enum ApplicationType
    {
        Desktop,
        Web
    }
    public class Email
    {
        public string SITE_ADMIN { get; private set; }
        public string SUBJECT { get; private set; }
        public string BODY { get; set; }
        public string FROM { get; private set; }
        public string FROM_NAME { get; private set; }
        public string TO { get; private set; }
        public string TO_NAME { get; set; }
        public string USERNAME { get; private set; }
        public string PASSWORD { get; private set; }
        public int PORT { get; private set; }
        public bool USESSL { get; private set; }
        public string SERVER { get; private set; }
        public bool REQUIREAUTH { get; private set; }
        public string WEBSITE_NAME { get; private set; }
        public string HTML_DEFAULT { get; private set; }
        public string HTML_FORGOTEN { get; private set; }
        public string HTML_CONFIRMATION { get; private set; }
        public string HTML_WELCOME { get; private set; }
        public string HTML_SHARE_PATH { get; private set; }
        public string HTML_DEFAULT_PATH { get; private set; }
        public string HTML_FORGOTEN_PATH { get; private set; }
        public string HTML_CONFIRMATION_PATH { get; private set; }
        public string HTML_WELCOME_PATH { get; private set; }
        public string HTML_SHARE { get; private set; }


        public Email(ApplicationType type)
        {
            switch (type)
            {
                case ApplicationType.Desktop:
                    HTML_CONFIRMATION_PATH = ConfigurationManager.AppSettings["email-html-confirmation"];
                    HTML_WELCOME_PATH = ConfigurationManager.AppSettings["email-html-welcome"];
                    HTML_FORGOTEN_PATH = ConfigurationManager.AppSettings["email-html-forgotten"];
                    HTML_DEFAULT_PATH = ConfigurationManager.AppSettings["email-html-default"];
                    HTML_SHARE_PATH = ConfigurationManager.AppSettings["email-html-share"];
                    break;
                case ApplicationType.Web:
                    HTML_CONFIRMATION_PATH = HostingEnvironment.MapPath(ConfigurationManager.AppSettings["email-html-confirmation"]);
                    HTML_WELCOME_PATH = HostingEnvironment.MapPath(ConfigurationManager.AppSettings["email-html-welcome"]);
                    HTML_FORGOTEN_PATH = HostingEnvironment.MapPath(ConfigurationManager.AppSettings["email-html-forgotten"]);
                    HTML_DEFAULT_PATH = HostingEnvironment.MapPath(ConfigurationManager.AppSettings["email-html-default"]);
                    HTML_SHARE_PATH = HostingEnvironment.MapPath(ConfigurationManager.AppSettings["email-html-share"]);
                    break;
                default:
                    break;
            }
            

            HTML_CONFIRMATION = GetEmailHtml(HTML_CONFIRMATION_PATH);
            HTML_WELCOME = GetEmailHtml(HTML_DEFAULT_PATH);
            HTML_FORGOTEN = GetEmailHtml(HTML_FORGOTEN_PATH);
            HTML_DEFAULT = GetEmailHtml(HTML_WELCOME_PATH);
            HTML_SHARE = GetEmailHtml(HTML_SHARE_PATH);


            WEBSITE_NAME = ConfigurationManager.AppSettings["WebsiteName"];

            SITE_ADMIN = ConfigurationManager.AppSettings["siteadmin"];

            SERVER = ConfigurationManager.AppSettings["email-server"];
            PORT = int.Parse(ConfigurationManager.AppSettings["email-port"]);
            USESSL = bool.Parse(ConfigurationManager.AppSettings["useSSL"]);
            PASSWORD = ConfigurationManager.AppSettings["email-password"];
            USERNAME = ConfigurationManager.AppSettings["email-username"];
            REQUIREAUTH = bool.Parse(ConfigurationManager.AppSettings["email-require-auth"]);

            TO_NAME = "New " + ConfigurationManager.AppSettings["WebsiteName"] + " Member";
            TO = ConfigurationManager.AppSettings["email-to"];
            FROM_NAME = ConfigurationManager.AppSettings["email-from-name"];
            FROM = ConfigurationManager.AppSettings["email-from"];

            SUBJECT = ConfigurationManager.AppSettings["email-subject"];


        }

        private EmailType emailMessageType;
        private string emailMessageHtml;
        private System.Net.Mail.SmtpClient SmtpServer;


        private string GetEmailFilePath(EmailType type)
        {
            emailMessageType = type;

            switch (type)
            {
                case EmailType.Confirmation:
                    emailMessageHtml = HTML_CONFIRMATION;
                    break;
                case EmailType.Forgotten:
                    emailMessageHtml = HTML_FORGOTEN;
                    break;
                case EmailType.Welcome:
                    emailMessageHtml = HTML_WELCOME;
                    break;
                case EmailType.Default:
                    emailMessageHtml = HTML_DEFAULT;
                    break;
                default:
                    emailMessageHtml = "";
                    break;
            }
            return emailMessageHtml;
        }

        public bool SetupServer()
        {
            try
            {
                SmtpServer = new System.Net.Mail.SmtpClient();
                SmtpServer.Port = PORT;
                SmtpServer.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                if (REQUIREAUTH)
                {
                    SmtpServer.UseDefaultCredentials = false;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(USERNAME, PASSWORD);
                }
                else SmtpServer.UseDefaultCredentials = true;

                SmtpServer.Host = SERVER;
                SmtpServer.EnableSsl = USESSL;

                return true;
            }
            catch
            {

                return false;
            }
        }
        public KeyValuePair<bool, string> SendMail(EmailType type, bool custom = false, string body = "", string To = "", string ToName = "New Member")
        {

            if (SetupServer())
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();

                mail.Subject = SUBJECT;
                mail.From = new System.Net.Mail.MailAddress(FROM, FROM_NAME);
                mail.IsBodyHtml = true;

                if (ToName != "") TO_NAME = ToName;

                if (To == "") mail.To.Add(new System.Net.Mail.MailAddress(TO, TO_NAME));
                else mail.To.Add(new System.Net.Mail.MailAddress(To, TO_NAME));

                if (custom)
                    mail.Body = body;
                else
                {
                    BODY = GetEmailFilePath(type);
                    mail.Body = BODY;
                }

                try
                {
                    SmtpServer.Send(mail);
                    return new KeyValuePair<bool, string>(true, "Email sent succesfully");
                }
                catch (Exception ex)
                {
                    return new KeyValuePair<bool, string>(false, ex.Message);
                }
                finally
                {
                    SmtpServer.Dispose();
                }
            }
            else return new KeyValuePair<bool, string>(false, "Error Setting UP Email");

        }
        public void SetBody(string message)
        {
            BODY = message;
        }
        private string GetEmailHtml(string path)
        {
            return File.ReadAllText(path);
        }
    }

}
