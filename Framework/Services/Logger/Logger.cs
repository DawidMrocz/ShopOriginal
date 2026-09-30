using Framework.Attribiutes.Dependency;
using log4net;
using log4net.Core;

namespace Framework.Services.Logger
{
    [DependencyInjection(typeof(ILogger))]
    public class Logger : ILogger
    {
        private log4net.Core.ILogger _logger;
        public Logger() => _logger = LogManager.GetLogger(GetType()).Logger;

        public void Log(Type type, Level logType, string message, Exception ex)
        {
            _logger.Log(type, logType, message, ex);
        }
    }
}
