using Newtonsoft.Json;
using Simple_Mail;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Simple_Email_Website.Controllers
{
    public class EmailsController : Controller
    {
        private const string _success = "Success emailing {0}";
        private const string _failure = "Error emailing {0}, {1}";
        // GET: Emails
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult SendEmail(string Email, string Message)
        {
            Email _email = new Email(ApplicationType.Web);
            string defaultHtml = _email.HTML_CONFIRMATION;
            string body = defaultHtml
                .Replace("*|EMAIL_PREVIEW|*", "This is a Simple Email")
                .Replace("*|HEADER|*", "Simple Email Website")
                .Replace("*|FIRSTPARAGRAPH|*", "The message below has been customized")
                .Replace("*|SECONDPARAGRAPH|*", Message)
                .Replace("*|WEBSITE_URL|*", Request.Url.ToString())
                .Replace("*|WEBSITE_NAME|*", _email.WEBSITE_NAME)
                .Replace("*|CURRENT_YEAR|*", DateTime.Now.Year.ToString());


            KeyValuePair<bool, string> response = _email.SendMail(EmailType.Confirmation, true, body, Email);

            Response result = new Controllers.Response();
            result.Success = response.Key;

            if (response.Key)
                result.Message = string.Format(_success, Email);
            else {
                result.Message = string.Format(_failure, Email, response.Value);
            }
            string json = JsonConvert.SerializeObject(result);


            return Json(json, JsonRequestBehavior.AllowGet);
        }

    }
    public class Response
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}