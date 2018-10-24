using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Elasticsearch;

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

            WriteHeader("USING: Serilog.WriteTo.Console(new RenderedCompactJsonFormatter())");

            Log.Logger = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console(new RenderedCompactJsonFormatter())
                .CreateLogger();

            RunTest(builder => builder.AddSerilog());

            WriteHeader("USING: Serilog.WriteTo.Console(new ElasticsearchJsonFormatter())");

            Log.Logger = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console(new ElasticsearchJsonFormatter())
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


                try
                {
                    logger.LogInformation(
                        "Cloning state for Job Id: {0}:{3}-> From parent Target Id: {1}{3}-> To child Target Id: {2}",
                        "context.JobId",
                        "parentState.TargetId",
                        "state.TargetId",
                        Environment.NewLine);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error");
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
