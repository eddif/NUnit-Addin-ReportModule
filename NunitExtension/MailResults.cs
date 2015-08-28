using System;
using System.Configuration;
using System.Net.Mail;

namespace ReportBuilder
{
    class MailResults
    {

        public static void SendMessage(MailHeader mh, string thissubject, string message)
        {
            string from = mh.EmailFromAddress;
            string to = mh.EmailToAddress;
            string cc = mh.EmailCcAddress;
            string bcc = mh.EmailBccAddress;
            string subject = thissubject;
            string smtp = mh.EmailSmtpServer;
            string displayname = mh.EmailSmtpServer;

            try
            {
                MailMessage msg = new MailMessage();

                msg.From = new MailAddress(from);

                if (String.IsNullOrEmpty(to) == false)
                {
                    string[] addrs = to.Split(';', ',');
                    foreach (string addr in addrs)
                    {
                        msg.To.Add(addr.Trim());
                    }
                }
                if (String.IsNullOrEmpty(cc) == false)
                {
                    string[] addrs = cc.Split(';', ',');
                    foreach (string addr in addrs)
                    {
                        msg.CC.Add(addr.Trim());
                    }
                }
                if (String.IsNullOrEmpty(bcc) == false)
                {
                    string[] addrs = bcc.Split(';', ',');
                    foreach (string addr in addrs)
                    {
                        msg.Bcc.Add(addr.Trim());
                    }
                }

                if (mh.Important)
                    msg.Priority = MailPriority.High;

                msg.Subject = subject;
                msg.Body = message;
                msg.IsBodyHtml = true;

                SmtpClient client = new SmtpClient(smtp);
                client.Port = 587;
                client.Send(msg);
            }
            catch (Exception e)
            {
                // Swallow exception
                Console.WriteLine(e);
            }
        }

        public class MailHeader
        {
            public MailHeader()
            {
                // always init with appsettings
                this.EmailToAddress = ConfigurationManager.AppSettings["EmailToAddress"];
                this.EmailFromAddress = ConfigurationManager.AppSettings["EmailFromAddress"];
                this.EmailCcAddress = ConfigurationManager.AppSettings["EmailCcAddress"];
                this.EmailBccAddress = ConfigurationManager.AppSettings["EmailBccAddress"];
                //this.EmailSubject = ConfigurationManager.AppSettings["EmailSubject"];
                this.EmailSmtpServer = ConfigurationManager.AppSettings["EmailSmtpServer"];
                this.EmailDisplayName = ConfigurationManager.AppSettings["EmailDisplayName"];
                this.Important = false;

            }

            public string EmailToAddress { get; set; }
            public string EmailFromAddress { get; set; }
            public string EmailCcAddress { get; set; }
            public string EmailBccAddress { get; set; }
            public string EmailSubject { get; set; }
            public string EmailSmtpServer { get; set; }
            public bool Important { get; set; }
            public string EmailDisplayName { get; set; }
        }


    }
}
