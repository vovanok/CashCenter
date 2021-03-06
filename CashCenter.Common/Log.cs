﻿using log4net;
using System;
using System.IO;

namespace CashCenter.Common
{
    public class Log
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Log));

        static Log()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));
        }

        public static void Info(string message)
        {
            log.Info(message);
        }

        public static void Error(string message)
        {
            log.Error(message);
        }

        public static void Error(string message, Exception exception)
        {
            log.Error(message, exception);
        }
    }
}
