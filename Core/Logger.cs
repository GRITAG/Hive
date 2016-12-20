using NLog;

namespace HiveSuite.Core
{
    public class Logger
    {
        /// <summary>
        /// Static log object, this keeps all logging though the same object instance
        /// </summary>
        private static NLog.Logger Loging = LogManager.GetLogger("Hive");

        public Logger()
        {

        }

        /// <summary>
        /// Create and store a log entry
        /// </summary>
        /// <param name="level">the log level to use</param>
        /// <param name="message">string message to store in the log</param>
        public void Log(LogLevel level, string message)
        {
            Loging.Log(TranslateLogLevel(level), message);
        }

        public static NLog.LogLevel TranslateLogLevel(LogLevel level)
        {
            NLog.LogLevel logLevel = NLog.LogLevel.Off;

            switch (level)
            {
                case LogLevel.Trace:
                    logLevel = NLog.LogLevel.Trace;
                    break;
                case LogLevel.Debug:
                    logLevel = NLog.LogLevel.Debug;
                    break;
                case LogLevel.Info:
                    logLevel = NLog.LogLevel.Info;
                    break;
                case LogLevel.Warn:
                    logLevel = NLog.LogLevel.Warn;
                    break;
                case LogLevel.Error:
                    logLevel = NLog.LogLevel.Error;
                    break;
                case LogLevel.Fatal:
                    logLevel = NLog.LogLevel.Fatal;
                    break;
            }

            return logLevel;
        }
    }

    /// <summary>
    /// Levels of logging
    /// </summary>
    public enum LogLevel
    {
        Trace, Debug, Info, Warn, Error, Fatal
    }


}
