using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

namespace LoggingSample
{
    class Program
    {
        private static void Main()
        {
            WriteHeader("USING: Microsoft.Extensions.Logging.ConsoleLoggerExtensions.AddConsole()");

            RunTest(builder => 
                builder.AddConsole(
                    configure => configure.IncludeScopes = true));

            WriteHeader("USING: Serilog.WriteTo.Console()");

            Log.Logger = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Properties}{NewLine}{Exception}")
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
            services.AddLogging(configureLogging);

            using (var serviceProvider = services.BuildServiceProvider(true))
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

                using (logger.BeginMessage("message123", "correlation123"))
                {
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
        }

        private static void ThrowCustomException()
        {
            var ex = new CustomException("Custom exception message");
            ex.Data["DataProperty"] = "my property value";
            throw ex;
        }
    }
}
