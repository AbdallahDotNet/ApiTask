using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Helpers
{
    public class LoggerHelper
    {
        //log error in external file
        public static void LogError(Exception exception, NLog.ILogger logger, string serviceName)
        {
            var error = (string.IsNullOrEmpty(exception.InnerException?.Message)) ? exception.Message : exception.InnerException.Message;

            logger.Error($"error in service called {serviceName} error is {error}");
        }
    }
}
