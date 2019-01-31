using System;
using System.Configuration;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests
{
    [TestClass]
    public class JobLoggerTest
    {
        [TestMethod]
        public void LogMessage_Empty_SouldNotThrowException()
        {
            JobLogger.LogMessage("", 0);
        }

        [TestMethod]
        public void LogMessage_WithoutType_SouldThrowException()
        {
            try
            {
                JobLogger.LogMessage("Hello", 0);
            }
            catch (Exception e)
            {
                // Assert
                StringAssert.Contains(e.Message, LoggerResources.Error_LogTypeMustBeSpecified);
            }
        }

        [TestMethod]
        public void LogMessage_LogToFile()
        {
            var messageToLog = "Hello";

            JobLogger.LogMessage(messageToLog, LogType.Warning);

            var filePath = ConfigurationManager.AppSettings["LogFileDirectory"] + "LogFile" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            if (!File.Exists(filePath))
            {
                throw new Exception("File was not created");
            }
            else
            {
                var text = File.ReadAllText(filePath);

                if (!text.Contains(messageToLog))
                {
                    throw new Exception("Message was not logged");
                }
            }
        }
    }
}
