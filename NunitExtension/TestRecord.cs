using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Reflection;



namespace ReportBuilder
{
    class TestRecord
    {
        public string FullName { get; set; }
        public string TestSuiteName { get; set; }
        public string TestName { get; set; }
        public string Status { get; set; }
        public string ComponentName { get; set; }
        public string AppBrowser { get; set; }
        public string AppName { get; set; }
        public int ResultState { get; set; }
        public string ErrorMessage { get; set; }
        public string CSVErrorMessage { get; set; }
        public string AssemblyName { get; set; }
        public string ComputerName { get; set; }
        public string BuildVersionNumber { get; set; }
        public DateTime RunDateTime { get; set; }
        public int Count { get; set; }
        public double TestTime { get; set; }
        public double RunTime { get; set; }
        private int TestsRun { get; set; }
        private int TestPassedCount { get; set; }
        private int TestFailureCount { get; set; }
        private int TestErrorCount { get; set; }
        private int TestIgnoreCount { get; set; }
        //public object ConfigurationManager { get; private set; }

        public int level;
        List<string> list_testerrors;
        List<string> list_alltesterrors;

        public string GetContainerName()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(AssemblyName);
            sb.Append("_");
            sb.Append(RunDateTime.ToString("yyyyMMddHHmmss"));
            sb.Append("_");
            sb.Append(Count);
            return sb.ToString();
        }

        internal void StartRun(string name, int testCount)
        {
            TestsRun = 0;
            TestPassedCount = 0;
            TestFailureCount = 0;
            TestErrorCount = 0;
            level = 0;

            RunDateTime = DateTime.UtcNow;
            Count = testCount;
            AssemblyName = GetAssemblyNameFromPath(name);
            BuildVersionNumber = GetAssemblyVersionFromPath(name);
            ComputerName = Environment.MachineName;

            list_alltesterrors = new List<string>();
        }

        private string GetAssemblyVersionFromPath(string name)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            string assemblyNameFromPath = GetAssemblyNameFromPath(name);
            foreach (Assembly a in assemblies)
            {
                string testAssemblyName = (a.GetName()).Name + ".dll";
                if (String.Compare(testAssemblyName, assemblyNameFromPath, true) == 0)
                {
                    return a.GetName().Version.ToString();
                }
            }
            return "0.0.0000.0";
        }

        private string GetAssemblyNameFromPath(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                FileInfo fi = new FileInfo(name);
                return fi.Name;
            }
            else
            {
                return "unknown";
            }
        }

        internal string GetHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Application");
            sb.Append(",");
            sb.Append("Browser");
            sb.Append(",");
            sb.Append("TestSuiteName");
            sb.Append(",");
            sb.Append("TestName");
            sb.Append(",");
            sb.Append("Status");
            sb.Append(",");
            sb.Append("Duration");
            sb.Append(",");
            sb.Append("ErrorMessage");
            return sb.ToString();
        }

        internal string GetSummaryHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("App.Browser");
            sb.Append(",");
            sb.Append("#Tests");
            sb.Append(",");
            sb.Append("#Passed");
            sb.Append(",");
            sb.Append("#Failed");
            sb.Append(",");
            sb.Append("#Errors");
            sb.Append(",");
            sb.Append("Time (Sec)");
            return sb.ToString();
        }

        internal string StoreTestResults()
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(AppName);
            sb.Append(",");
            sb.Append(AppBrowser);
            sb.Append(",");
            sb.Append(TestSuiteName);
            sb.Append(",");
            sb.Append(TestName);
            sb.Append(",");
            sb.Append(Status);
            sb.Append(",");
            sb.Append(TestTime.ToString("##.#####"));
            sb.Append(",");
            sb.Append(CSVErrorMessage.Replace("\n", ";"));
            return sb.ToString();
        }

        internal string GetAllSuiteResults()
        {

            if (list_testerrors.Count == 0)
            {
                return String.Empty;
            }
            else
            {
                StringBuilder sb = new StringBuilder();

                foreach (string sm in list_alltesterrors)
                {
                    sb.Append(sm).Append("</table></br>");
                }
                return sb.ToString();
            }
        }

        internal string StoreSummaryResults()
        {

            string tablestyle = " style='border:solid #4472C4 1.0pt; text-align:center; cellpadding=0px; border-collapse:collapse; font-family: Calibri, sans-serif; size: 11pt; '";
            string headerrowstyle = " style='background-color: #4472C4; color: #FFF; height:.2in; '";
            string datarowstyle = " style='color: #000; height:.2in; '";
            string tdstring = " width='156' style='width:116.85pt; border:solid #4472C4 1.0pt; text-align:center; padding:0in 5.4pt'";
            string tdcounts = " width='96' style='width:1.0in; border:solid #4472C4 1.0pt;'";
            string failcolor = "#F63438;";
            string errcolor = "#F63438;";
            if (TestFailureCount == 0)
            {
                failcolor = " #000'";
            }
            if (TestErrorCount == 0)
            {
                failcolor = " #000'";
            }
            string tdstylefail = " width='96' style='width:1.0in; border:solid #4472C4 1.0pt; color: " + failcolor + "'";
            string tdstyleerror = " width='96' style='width:1.0in; border:solid #4472C4 1.0pt; color: " + errcolor + "'";
            
            
            StringBuilder sb = new StringBuilder();

            //header
            sb.Append("<table" + tablestyle + ">");
            sb.Append("<tr" + headerrowstyle + ">");
            sb.Append("<td" + tdstring + ">");
            sb.Append("App.Browser</td>");
            sb.Append("<td" + tdcounts + ">");
            sb.Append("Tests Run</td>");
            sb.Append("<td" + tdcounts + ">");
            sb.Append("Passed</td>");
            sb.Append("<td" + tdcounts + ">");
            sb.Append("Failures</td>");
            sb.Append("<td" + tdcounts + ">");
            sb.Append("Errors</td>");
            sb.Append("<td" + tdstring + ">");
            sb.Append("Run Time (Sec)</td>");
            sb.Append("</tr>");
            //data
            sb.Append("<tr" + datarowstyle + ">");
            sb.Append("<td" + tdstring + ">");
            sb.Append(AppName + "." + AppBrowser + "</td>");
            sb.Append("<td" + tdcounts + ">");
            sb.Append(TestsRun + "</td>");
            sb.Append("<td" + tdcounts + ">");
            sb.Append(TestPassedCount + "</td>");
            sb.Append("<td" + tdstylefail + ">");
            sb.Append(TestFailureCount + "</td>");
            sb.Append("<td" + tdstyleerror + ">");
            sb.Append(TestErrorCount + "</td>");
            sb.Append("<td" + tdstring + ">");
            sb.Append(RunTime.ToString("##.#####") + "</td>");
            sb.Append("</tr></table>");
            return sb.ToString();
        }

        internal void StartTestSuite(NUnit.Core.TestName testName)
        {
            level = 0;

            if (level++ == 0)
            {
                list_testerrors = new List<string>();
            }
        }

        internal void CompleteTestSuite(NUnit.Core.TestResult result)
        {

            if (list_testerrors.Count == 0)
            {
                return;
            }
            else
            {
                string tablestyle = " width='800' style='max-width: 800px; display:block; border:solid #4472C4 1.0pt; text-align:left; cellpadding=0px; border-collapse:collapse; font-family: Calibri, sans-serif; size: 11pt; '";
                string headerrowstyle = " style='background-color: #4472C4; color: #FFF; height:.2in; '";
                string suiteheaderstyle = " width='800' style='padding: 5pt; max-width: 800px;'";
                string tdstyle1 = " style='padding:5pt 5pt 10pt 5pt'";

                StringBuilder sb = new StringBuilder();
                sb.Append("<table" + tablestyle + ">");
                sb.Append("<tr" + headerrowstyle + ">");
                sb.Append("<td" + suiteheaderstyle + ">SUITE: " + TestSuiteName + "</td></tr>");

                foreach (string msg in list_testerrors)
                {
                    sb.Append("<tr><td" + tdstyle1 + ">");
                    sb.Append(msg);
                    sb.Append("</td></tr>");
                }
                list_alltesterrors.Add(sb.ToString());
            }
        }

        internal void StartTest(NUnit.Core.TestName testName)
        {
            TestName = testName.Name;
        }

        internal void CompleteTest(NUnit.Core.TestResult result)
        {
            TestTime = result.Time;

            if (result.Executed)
            {

                TestsRun++;
                string message = "";
                string span = "<span style='color: #F63438;'>";
                CSVErrorMessage = "";
                ErrorMessage = "";

                if (result.IsSuccess)
                {
                    Status = "Pass";
                    TestPassedCount++;
                }
                if (result.IsFailure)
                {
                    TestFailureCount++;
                    Status = "Fail";

                    if (result.Message != null && result.Message != string.Empty)
                    {
                        message = result.Message.Replace(",", ";");
                        CSVErrorMessage = message.Replace("\r", ";");
                    }

                    ErrorMessage = "<b>Message: </b>" + message.Replace("\r", "");

                    list_testerrors.Add(string.Format("<b>TEST:</b> " + span + "{0}</span><br>{1}", TestName, ErrorMessage));


                }
                if (result.IsError)
                {
                    
                    TestErrorCount++;
                    Status = "Error";

                    if (result.Message != null && result.Message != string.Empty)
                    {
                        message = result.Message.Replace(",", ";");
                        CSVErrorMessage = message.Replace("\r", ";");
                    }

                    string stack = FormatStackTrace(result.StackTrace.ToString());

                    
                    ErrorMessage = "<b>Exception: </b>" + message + "<br><b>Stack: </b>" + stack;

                    list_testerrors.Add(string.Format("<b>TEST:</b> " + span + "{0}</span><br>{1}", TestName, ErrorMessage));
                }
            }
            else
            {
                Status = "Not Run2";
            }
        }

        internal void CompleteTestWith(Exception exception)
        {
            TestTime = 0;
            ResultState = 2; //Error
        }

        public void SendMail(string message)
        {

            string nomail = ConfigurationManager.AppSettings["LocalMachineName"];

            if (String.IsNullOrEmpty(nomail) == false)
            {
                string[] email = nomail.Split(';', ',');

                bool found = false;

                foreach (string lmn in email)
                {
                    if (Environment.MachineName.Contains(lmn))
                    {
                        found = true;
                        break;
                    }
                }

                if (found == false)
                {
                    MailResults.MailHeader mh = new MailResults.MailHeader();

                    string subject = "COMPLETED - Kurryer Test Automation for "+ AppName +" [" + AppBrowser + "] ";
                    string body = message;
                    MailResults.SendMessage(mh, subject, body);
                }
            }
        }

        public string FormatStackTrace(string stacktrace)
        {
            List<string> stkmssgs = new List<string>();
            
            if (stacktrace != null && stacktrace != string.Empty)
            {
                string[] trace = stacktrace.Split(Environment.NewLine.ToCharArray());
                foreach (string s in trace)
                {
                    if (s != string.Empty)
                    {
                        string msg = Regex.Replace(s.Trim(), @".* in (.*):line (.*)", "$1($2)");
                        stkmssgs.Add(string.Format("\n {0}", msg));
                    }
                }
               
            }

            StringBuilder sb = new StringBuilder();
            for(int i = 5; i < stkmssgs.Count; i++)
            sb.Append(stkmssgs[i].ToString());
            return sb.ToString();
        }
    }


}
