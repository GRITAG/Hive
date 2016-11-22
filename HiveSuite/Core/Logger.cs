using NLog;

namespace HiveSuite.Core
{
    public class Logger
    {
        private static NLog.Logger Loging = LogManager.GetLogger("Hive");

        public Logger()
        {

        }

        public void Log(LogLevel level, string message)
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

            Loging.Log(logLevel, message);
        }
    }

    public enum LogLevel
    {
        Trace, Debug, Info, Warn, Error, Fatal
    }


}
