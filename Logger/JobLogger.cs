using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace Logger
{
    public static class JobLogger
    {
        private static readonly bool _logToFile = bool.Parse(ConfigurationManager.AppSettings["LogToFile"]);
        private static readonly bool _logToConsole = bool.Parse(ConfigurationManager.AppSettings["LogToConsole"]);
        private static readonly bool _logToDatabase = bool.Parse(ConfigurationManager.AppSettings["LogToDatabase"]);
        private static readonly bool _logMessage = bool.Parse(ConfigurationManager.AppSettings["LogMessage"]);
        private static readonly bool _logWarning = bool.Parse(ConfigurationManager.AppSettings["LogWarning"]);
        private static readonly bool _logError = bool.Parse(ConfigurationManager.AppSettings["LogError"]);

        public static void LogMessage(string message, LogType logType)
        {
            message.Trim();

            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            if (!_logToConsole && !_logToFile && !_logToDatabase)
            {
                throw new Exception(LoggerResources.Error_InvalidConfiguration);
            }
            if ((!_logError && !_logMessage && !_logWarning) || (logType == default(LogType)))
            {
                throw new Exception(LoggerResources.Error_LogTypeMustBeSpecified);
            }
            if ((logType == LogType.Message && !_logMessage) ||
               (logType == LogType.Warning && !_logWarning) ||
               (logType == LogType.Error && !_logError))
            {
                return;
            }

            if (_logToDatabase)
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Test"].ConnectionString))
                {
                    var command = new SqlCommand("Insert into Log Values('" + message + "', " + (int)logType + ", '" + DateTime.Now + "')", connection);
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            if (_logToFile)
            {
                var filePath = ConfigurationManager.AppSettings["LogFileDirectory"] + "LogFile" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                var messageToLog = DateTime.Now + ": " + logType + ": " + message;
                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, messageToLog);
                }
                else
                {
                    File.AppendAllText(filePath, Environment.NewLine + messageToLog);
                }
            }

            if (_logToConsole)
            {
                switch (logType)
                {
                    case LogType.Message:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    case LogType.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LogType.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                }
                Console.WriteLine(DateTime.Now + ": " + message);
            }
        }
    }
}