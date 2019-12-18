using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SITW.Services
{
    public static class EmailTemplatesService
    {
        public static string GetVerifyEmailHTML(string VerifyLink)
        {
            string template = "";
            using (var sr = new StreamReader(HttpContext.Current.Server.MapPath("\\Content\\Templates\\") + "VerifyEmail.txt"))
            {
                template = sr.ReadToEnd();
            }

            string messageBody = string.Format(template, VerifyLink);
            return messageBody;
        }
        public static string GetChangeEmailHTML(string Content)
        {
            string template = "";
            using (var sr = new StreamReader(HttpContext.Current.Server.MapPath("\\Content\\Templates\\") + "ChangeEmail.txt"))
            {
                template = sr.ReadToEnd();
            }

            string messageBody = string.Format(template, Content);
            return messageBody;
        }

        public static string GetAllUserEmailHTML(string Content)
        {
            string template = "";
            using (var sr = new StreamReader(HttpContext.Current.Server.MapPath("\\Content\\Templates\\") + "BackEmail.txt"))
            {
                template = sr.ReadToEnd();
            }

            string messageBody = string.Format(template, Content);
            return messageBody;
        }


        public static string GetLiveEmailHTML(string Content)
        {
            string template = "";
            using (var sr = new StreamReader(HttpContext.Current.Server.MapPath("\\Content\\Templates\\") + "LiveEmail.txt"))
            {
                template = sr.ReadToEnd();
            }

            string messageBody = string.Format(template, Content);
            return messageBody;
        }

    }
}