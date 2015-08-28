using System;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Configuration;
using NUnit.Core.Extensibility;
using NUnit.Core;


namespace ReportBuilder
{
    [NUnitAddin(Description = "Report Generator")]
    public class EventTracer : IAddin, EventListener
    {
        public bool Install(IExtensionHost host)
        {
            if (host == null)
                throw new ArgumentNullException("host");

            IExtensionPoint listeners = host.GetExtensionPoint("EventListeners");
            if (listeners == null)
                return false;

            listeners.Install(this);
            return true;
        }

        TestRecord testRecord = new TestRecord();
        StringBuilder sb = new StringBuilder();

        #region Nunit EventListener Members

        public void RunStarted(string name, int testCount)
        {
            testRecord.StartRun(name, testCount);
            string logFileName = GetLogFileName(testRecord);
            LogHeader(logFileName, testRecord);
        }

        public void SuiteStarted(TestName testName)
        {
            testRecord.StartTestSuite(testName);
            testRecord.TestSuiteName = testName.Name;
        }

        public void TestStarted(TestName testName)
        {
            testRecord.StartTest(testName);
        }

        public void TestFinished(TestResult result)
        {
            testRecord.CompleteTest(result);
            string logFileName = GetLogFileName(testRecord);
            LogTestResult(logFileName, testRecord);
            testRecord.TestTime = result.Time;
        }

        public void SuiteFinished(TestResult suiteResult)
        {

            if (--testRecord.level == 0)
            {
                testRecord.CompleteTestSuite(suiteResult);
            }
        }

        public void RunFinished(TestResult result)
        {
            testRecord.RunTime = result.Time;

            LogSummaryResult(testRecord);
            
        }

        public void RunFinished(Exception exception)
        {
            Debug.Write("UnhandledException " + exception); 
        }

        public void UnhandledException(Exception exception)
        {
            Debug.Write("UnhandledException " + exception);
        }

        public void TestOutput(TestOutput testOutput)
        {
            return;
        }

        #endregion


        private string GetLogFileName(TestRecord testRecord)
        {
            if (testRecord != null)
            {
                string path = ConfigurationManager.AppSettings["Report_Path"];
                string fileName = testRecord.RunDateTime.ToString("yyyyMMddHHmmss") + ".csv";
                string filePath = path + "_" + fileName;
                return filePath;
            }
            else
            {
                return "testresultlog.csv";
            }
        }

        private void LogHeader(string logFileName, TestRecord testRecord)
        {
            string message = testRecord.GetHeader();
            LogMessageToCSV(logFileName, message);
        }

        private void LogTestResult(string logFileName, TestRecord testRecord)
        {
            string message = testRecord.StoreTestResults();
            LogMessageToCSV(logFileName, message);
        }

        private void LogMessageToCSV(string logFileName, string message)
        {
            using (StreamWriter sw = File.AppendText(logFileName))
            {
                sw.WriteLine(message);
            }
        }

        private void LogSummaryResult(TestRecord testRecord)
        {

            string headerstyle = " style='font-family: Calibri, sans-serif;'";
            string summaryheader = "<p" + headerstyle + "> *** SUMMARY ***</p>";
            string summary = testRecord.StoreSummaryResults();
            string failures = testRecord.GetAllSuiteResults();
            string failureheading = "";

            if (failures != String.Empty)
                failureheading = "<p" + headerstyle + "> *** FAILURES / ERRORS *** </p>";

            string msg = summaryheader + summary + failureheading + failures;

            testRecord.SendMail(msg);
        }



    }
}
