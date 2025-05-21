using System;
using System.IO;
using System.Windows.Forms;

namespace EmployeeManagement.Utilities
{
    public static class Logger
    {
        private static readonly string LogDirectory = Path.Combine(Application.StartupPath, "Logs");
        private static readonly object lockObject = new object();

        static Logger()
        {
            // Tạo thư mục Logs nếu chưa tồn tại
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }
        }

        public static void LogInfo(string message)
        {
            WriteLog("INFO", message);
        }

        public static void LogError(string message)
        {
            WriteLog("ERROR", message);
        }

        public static void LogWarning(string message)
        {
            WriteLog("WARNING", message);
        }

        public static void LogDebug(string message)
        {
            WriteLog("DEBUG", message);
        }

        private static void WriteLog(string level, string message)
        {
            try
            {
                lock (lockObject)
                {
                    var logFileName = $"Log_{DateTime.Now:yyyy-MM-dd}.txt";
                    var logFilePath = Path.Combine(LogDirectory, logFileName);

                    var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";

                    using (var writer = new StreamWriter(logFilePath, true))
                    {
                        writer.WriteLine(logEntry);
                    }
                }
            }
            catch
            {
             }
        }
    }
}