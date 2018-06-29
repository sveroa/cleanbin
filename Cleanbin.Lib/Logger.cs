using System;
using log4net;
using log4net.Config;

namespace Cleanbin.Lib
{
    /// <summary>
    /// 
    /// </summary>
    public enum LogLevelL4N
    {
        DEBUG = 1,
        ERROR,
        FATAL,
        INFO,
        WARN
    }

    // ************************************************************************
    // Log4net Thread-Safe but not Process-Safe
    // http://hectorcorrea.com/Blog/Log4net-Thread-Safe-but-not-Process-Safe
    //
    // http://www.codeproject.com/KB/dotnet/MultipleLog4net.aspx
    // ************************************************************************

    public static class Logger
    {
        #region Members
        private static readonly ILog logger = LogManager.GetLogger(typeof(Logger));
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;

        #endregion

        #region Constructors
        static Logger()
        {
            XmlConfigurator.Configure();
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="log"></param>
        /// <param name="tStartTime"></param>
        public static void WriteLog(LogLevelL4N logLevel, String log, DateTime tStartTime)
        {
            var elapsed = DateTime.Now - tStartTime ;
            WriteLog(logLevel, log + " - Execution time: " + elapsed.TotalSeconds.ToString("#0.0###") + " sec(s)");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="log"></param>
        public static void WriteLog(LogLevelL4N logLevel, String log)
        {
            if (logLevel.Equals(LogLevelL4N.DEBUG))
            {
                logger.Debug(log);
            }

            else if (logLevel.Equals(LogLevelL4N.ERROR))
            {
                logger.Error(log);
            }

            else if (logLevel.Equals(LogLevelL4N.FATAL))
            {
                logger.Fatal(log);
            }

            else if (logLevel.Equals(LogLevelL4N.INFO))
            {
                logger.Info(log);
            }

            else if (logLevel.Equals(LogLevelL4N.WARN))
            {
                logger.Warn(log);
            }
        }
        #endregion
    }
}
