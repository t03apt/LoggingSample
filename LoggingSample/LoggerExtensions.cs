using System;
using Microsoft.Extensions.Logging;

namespace LoggingSample
{
    public static class LoggerExtensions
    {
        public static IDisposable BeginMessage<T>(this ILogger<T> logger, string messageId, string correlationId)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            return logger.BeginScope(new LogHeader
            {
                {"MessageId", messageId},
                {"CorrelationId", correlationId}
            });
        }
    }
}