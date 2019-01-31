using System;

namespace Logger.Program
{
    class Program
    {
        static void Main(string[] args)
        {
            JobLogger.LogMessage("Hello", LogType.Message);
            JobLogger.LogMessage("Hello", LogType.Error);
            JobLogger.LogMessage("Hello", LogType.Warning);

            Console.ReadLine();
        }
    }
}
