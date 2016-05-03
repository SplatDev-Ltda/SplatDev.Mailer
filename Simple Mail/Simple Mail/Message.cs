using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.Hosting;

namespace zuEuz.Smtp.Gun
{
    #region Enums
    /// <summary>
    /// Types of Emails that can be sent
    /// </summary>
    public enum MessageType
    {
        Default,
        Confirmation,
        Forgotten,
        Welcome,
        Error
    }
    /// <summary>
    /// Type of Application this module is being ran from
    /// </summary>
    public enum ApplicationType
    {
        Desktop,
        Web
    }
    #endregion

    #region Class
    /// <summary>
    /// Email Module Class
    /// </summary>
    public class Message
    {
        #region Public Members
        // <summary>
        /// The Email address of the Email Module Admin
        /// </summary>
        public string SITE_ADMIN { get; set; }
        /// <summary>
        /// Subject of the Email
        /// </summary>
        public string SUBJECT { get; set; }
        /// <summary>
        /// Body of the Email (default is html)
        /// </summary>
        public string BODY { get; set; }
        /// <summary>
        /// From Email Address (who's sending the email)
        /// </summary>
        public string FROM { get; set; }
        /// <summary>
        /// From Name (name of who is sending the email)
        /// </summary>
        public string FROM_NAME { get; set; }
        /// <summary>
        /// To Email Address (who is going to receive this email)
        /// </summary>
        public string TO { get; private set; }
        /// <summary>
        /// To Name (name of who is going to receive this email)
        /// </summary>
        public string TO_NAME { get; set; }
        /// <summary>
        /// Username of Smtp Service / Server
        /// </summary>
        public string USERNAME { get; private set; }
        /// <summary>
        /// Password for the smtp service / server
        /// </summary>
        public string PASSWORD { get; private set; }
        /// <summary>
        /// The port number for sending the email address (normal ports are 25, 587, 465)
        /// </summary>
        public int PORT { get; private set; }
        /// <summary>
        /// Whether to use SSL Encryption
        /// </summary>
        public bool USESSL { get; private set; }
        /// <summary>
        /// Uri of the Smtp Server
        /// </summary>
        public string SERVER { get; private set; }
        /// <summary>
        /// Whether the smtp server requires authentication
        /// </summary>
        public bool REQUIREAUTH { get; private set; }
        /// <summary>
        /// Name of the Website or App running the Module
        /// </summary>
        public string WEBSITE_NAME { get; set; }
        /// <summary>
        /// Default Html Body for the Email
        /// </summary>
        public string HTML_DEFAULT { get; private set; }
        /// <summary>
        /// Html body for the Forgotten Email
        /// </summary>
        public string HTML_FORGOTEN { get; private set; }
        /// <summary>
        /// Html Body for the Confirmation Email
        /// </summary>
        public string HTML_CONFIRMATION { get; private set; }
        /// <summary>
        /// Html Body for the Welcome Email
        /// </summary>
        public string HTML_WELCOME { get; private set; }
        /// <summary>
        /// Html for the Social Share
        /// </summary>
        public string HTML_SHARE_PATH { get; private set; }
        /// <summary>
        /// Path for the Default Html File
        /// </summary>
        public string HTML_DEFAULT_PATH { get; private set; }
        /// <summary>
        /// Path for the Forgotten Html File
        /// </summary>
        public string HTML_FORGOTEN_PATH { get; private set; }
        /// <summary>
        /// Path for the Confirmation Html File
        /// </summary>
        public string HTML_CONFIRMATION_PATH { get; private set; }
        /// <summary>
        /// Path for the Welcome Html File
        /// </summary>
        public string HTML_WELCOME_PATH { get; private set; }
        /// <summary>
        /// Path for the Social Share Html File
        /// </summary>
        public string HTML_SHARE { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Manually configures a Smtp Server Instance (when you do not want to use config files)
        /// </summary>
        /// <param name="smtpServer">The Uri for the smtp server</param>
        /// <param name="smtpPort">The Smtp Port</param>
        /// <param name="useAuth">Whether the Smtp Server Requires Authentication</param>
        /// <param name="username">The Smtp Username</param>
        /// <param name="password">The Smtp Password</param>
        /// <param name="useSSL">Whether to Use SSL Encryption</param>
        public Message(string smtpServer, int smtpPort, bool useAuth = false, string username = "", string password = "", bool useSSL = false)
        {
            SERVER = smtpServer;
            PORT = smtpPort;
            REQUIREAUTH = useAuth;
            USERNAME = username;
            PASSWORD = password;
            USESSL = useSSL;
        }
        /// <summary>
        /// Instanciates a new Email Module based on Config Files and Application Type (Web or Desktop)
        /// </summary>
        /// <param name="type"></param>
        public Message(ApplicationType type)
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

        #endregion

        #region Private Members

        private MessageType emailMessageType;
        private string emailMessageHtml;
        private System.Net.Mail.SmtpClient SmtpServer;
        #endregion

        #region Private Methods
        private string GetEmailHtml(string path)
        {
            return File.ReadAllText(path);
        }

        private string GetEmailFilePath(MessageType type)
        {
            emailMessageType = type;

            switch (type)
            {
                case MessageType.Confirmation:
                    emailMessageHtml = HTML_CONFIRMATION;
                    break;
                case MessageType.Forgotten:
                    emailMessageHtml = HTML_FORGOTEN;
                    break;
                case MessageType.Welcome:
                    emailMessageHtml = HTML_WELCOME;
                    break;
                case MessageType.Default:
                    emailMessageHtml = HTML_DEFAULT;
                    break;
                default:
                    emailMessageHtml = "";
                    break;
            }
            return emailMessageHtml;
        }

        private bool SetupServer()
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

        #endregion

        #region Public Methods
        /// <summary>
        /// Attempts to send an email
        /// </summary>
        /// <param name="type">The Type of Email Being Sent</param>
        /// <param name="custom">Whether to Customize the email</param>
        /// <param name="body">If customized, set the html body for the email</param>
        /// <param name="To">If customized, defines the TO field</param>
        /// <param name="ToName">If customized, defines the To name</param>
        /// <returns>Returns a KeyValuePair<bool,string> with success (true/false) and a message from the smtp server</returns>
        public KeyValuePair<bool, string> SendMail(MessageType type, bool custom = false, string body = "", string To = "", string ToName = "New Member")
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
        /// <summary>
        /// Manually Sets the Body for the Email Message
        /// </summary>
        /// <param name="message">The Html Body for the Email</param>
        public void SetBody(string message)
        {
            BODY = message;
        }
        #endregion

    } 
    #endregion
}
