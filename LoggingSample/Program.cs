using System;
using System.Runtime.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

namespace LoggingSample
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteHeader("USING: Microsoft.Extensions.Logging.ConsoleLoggerExtensions.AddConsole()");

            RunTest(builder => builder.AddConsole());

            WriteHeader("USING: Serilog.WriteTo.Console()");

            Log.Logger = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console()
                .CreateLogger();

            RunTest(builder => builder.AddSerilog());

            WriteHeader("USING: Serilog.WriteTo.Console(new CompactJsonFormatter())");

            Log.Logger = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console(new CompactJsonFormatter())
                .CreateLogger();

            RunTest(builder => builder.AddSerilog());
        }

        private static void WriteHeader(string header)
        {
            Console.WriteLine("\r\n---- {0} ----\r\n", header);
        }

        private static void RunTest(Action<ILoggingBuilder> configureLogging)
        {
            var services = new ServiceCollection();
            services.AddLogging(builder => configureLogging(builder));

            using (var serviceProvider = services.BuildServiceProvider(true))
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

                try
                {
                    ThrowCustomException();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error. {MessageProperty}", "Sample property value");
                }
            }
        }

        private static void ThrowCustomException()
        {
            var ex = new CustomException("Custom exception message");
            ex.Data["DataProperty"] = "my property value";
            throw ex;
        }
    }

    public class CustomException : Exception
    {
        public CustomException()
        {
        }

        public CustomException(string message) : base(message)
        {
        }

        public CustomException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CustomException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public string MyProperty { get; set; } = "My custom property";

    }
}
